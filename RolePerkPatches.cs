using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace PerkFixesAndRebalance
{
    internal static class RolePerks
    {
        internal static bool IsPartyRole(PartyRole role)
        {
            switch (role)
            {
                case PartyRole.Scout:
                case PartyRole.Engineer:
                case PartyRole.Quartermaster:
                case PartyRole.Surgeon:
                case PartyRole.FirstMate:
                case PartyRole.Navigator:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>The party role the perk has in either slot, or None.</summary>
        internal static PartyRole RoleOf(PerkObject perk)
        {
            if (IsPartyRole(perk.PrimaryRole))
            {
                return perk.PrimaryRole;
            }

            return IsPartyRole(perk.SecondaryRole) ? perk.SecondaryRole : PartyRole.None;
        }

        /// <summary>For code that reads a role hero's perk directly instead of through MobileParty.HasPerk.</summary>
        internal static bool RoleHeroHasPerk(Hero roleHero, PerkObject perk)
        {
            if (roleHero == null)
            {
                return false;
            }

            if (roleHero.GetPerkValue(perk))
            {
                return true;
            }

            Hero leader = roleHero.PartyBelongedTo?.LeaderHero;
            return PerkChanges.LeaderOrRoleHero && leader != null && leader != roleHero
                && RoleOf(perk) != PartyRole.None && leader.GetPerkValue(perk);
        }

        /// <summary>The skill that scales a role perk: the role hero's, or the higher of role hero and leader.</summary>
        internal static int Skill(MobileParty party, PartyRole role, SkillObject skill)
        {
            int value = party.GetEffectiveRoleHolder(role)?.GetSkillValue(skill) ?? 0;
            Hero leader = party.LeaderHero;
            if (PerkChanges.LeaderOrRoleHero && leader != null)
            {
                value = Math.Max(value, leader.GetSkillValue(skill));
            }

            return value;
        }

        internal static IEnumerable<CodeInstruction> ReplaceCalls(IEnumerable<CodeInstruction> instructions, MethodInfo original, MethodInfo replacement)
        {
            foreach (CodeInstruction code in instructions)
            {
                yield return code.Calls(original)
                    ? new CodeInstruction(OpCodes.Call, replacement).WithLabels(code.labels).WithBlocks(code.blocks)
                    : code;
            }
        }
    }

    /// <summary>
    /// Party role effects (scout, quartermaster, surgeon, engineer, first mate, navigator) also count when the party
    /// leader has the perk. Vanilla only falls back to the leader when the role is empty.
    /// </summary>
    [HarmonyPatch(typeof(MobileParty), nameof(MobileParty.HasPerk))]
    internal static class LeaderOrRoleHeroHasPerkPatch
    {
        private static void Postfix(MobileParty __instance, PerkObject perk, bool checkSecondaryRole, ref bool __result)
        {
            if (__result || !PerkChanges.LeaderOrRoleHero || perk == null
                || !RolePerks.IsPartyRole(checkSecondaryRole ? perk.SecondaryRole : perk.PrimaryRole))
            {
                return;
            }

            __result = __instance.LeaderHero?.GetPerkValue(perk) == true;
        }
    }

    /// <summary>
    /// Skill-scaled role capstones (Price of Loyalty, Uncanny Insight) are read from the role hero's character. With the
    /// leader-or-role-hero option, the perk can come from either and the higher of their two skills is used.
    /// </summary>
    [HarmonyPatch(typeof(PerkHelper), nameof(PerkHelper.AddEpicPerkBonusForCharacter))]
    internal static class LeaderOrRoleHeroEpicPerkPatch
    {
        private static bool Prefix(PerkObject perk, CharacterObject character, SkillObject skillType, bool applyPrimaryBonus,
            ref ExplainedNumber bonuses, int skillRequired, bool shouldApplyNavalMultiplier)
        {
            if (!PerkChanges.LeaderOrRoleHero || perk == null)
            {
                return true;
            }

            PartyRole role = applyPrimaryBonus ? perk.PrimaryRole : perk.SecondaryRole;
            MobileParty party = character?.HeroObject?.PartyBelongedTo;
            if (!RolePerks.IsPartyRole(role) || party == null)
            {
                return true;
            }

            if (party.HasPerk(perk, !applyPrimaryBonus))
            {
                int skill = Math.Max(character.GetSkillValue(skillType), RolePerks.Skill(party, role, skillType));
                if (skill > skillRequired)
                {
                    float value = (applyPrimaryBonus ? perk.PrimaryBonus : perk.SecondaryBonus) * (skill - skillRequired)
                        * (shouldApplyNavalMultiplier ? 0.5f : 1f);
                    EffectIncrementType type = applyPrimaryBonus ? perk.PrimaryIncrementType : perk.SecondaryIncrementType;
                    if (type == EffectIncrementType.AddFactor)
                    {
                        bonuses.AddFactor(value, perk.Name);
                    }
                    else
                    {
                        bonuses.Add(value, perk.Name);
                    }
                }
            }

            return false;
        }
    }

    /// <summary>Forest Kin is read straight from the effective scout.</summary>
    [HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), nameof(DefaultPartySpeedCalculatingModel.CalculateFinalSpeed))]
    internal static class ForestKinLeaderOrScoutPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
            RolePerks.ReplaceCalls(instructions,
                AccessTools.Method(typeof(Hero), nameof(Hero.GetPerkValue), new[] { typeof(PerkObject) }),
                AccessTools.Method(typeof(ForestKinLeaderOrScoutPatch), nameof(HasPerk)));

        private static bool HasPerk(Hero hero, PerkObject perk) =>
            perk == DefaultPerks.Scouting.ForestKin ? RolePerks.RoleHeroHasPerk(hero, perk) : hero.GetPerkValue(perk);
    }

    /// <summary>
    /// Recruit slots. Artisan Community's merchant slot is a quartermaster effect, but vanilla reads the recruiting hero.
    /// Engineering Guilds' artisan slot is read straight from the effective engineer.
    /// </summary>
    [HarmonyPatch(typeof(DefaultVolunteerModel), nameof(DefaultVolunteerModel.MaximumIndexHeroCanRecruitFromHero))]
    internal static class RecruitSlotRolePerksPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
            RolePerks.ReplaceCalls(instructions,
                AccessTools.Method(typeof(Hero), nameof(Hero.GetPerkValue), new[] { typeof(PerkObject) }),
                AccessTools.Method(typeof(RecruitSlotRolePerksPatch), nameof(HasPerk)));

        private static bool HasPerk(Hero hero, PerkObject perk)
        {
            if (perk == DefaultPerks.Trade.ArtisanCommunity && PerkChanges.ArtisanCommunityQuartermaster && hero?.PartyBelongedTo != null)
            {
                return hero.PartyBelongedTo.HasPerk(perk, true);
            }

            if (perk == DefaultPerks.Engineering.EngineeringGuilds)
            {
                return RolePerks.RoleHeroHasPerk(hero, perk);
            }

            return hero.GetPerkValue(perk);
        }
    }

    /// <summary>
    /// Rapid Development's buying discount checks the clan leader (primary role) before adding the quartermaster
    /// (secondary role) bonus, so vanilla needs both to have the perk. This makes the quartermaster enough.
    /// </summary>
    [HarmonyPatch(typeof(DefaultTradeItemPriceFactorModel), "GetTradePenalty")]
    internal static class RapidDevelopmentQuartermasterPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
            RolePerks.ReplaceCalls(instructions,
                AccessTools.Method(typeof(MobileParty), nameof(MobileParty.HasPerk)),
                AccessTools.Method(typeof(RapidDevelopmentQuartermasterPatch), nameof(HasPerk)));

        private static bool HasPerk(MobileParty party, PerkObject perk, bool checkSecondaryRole)
        {
            if (PerkChanges.RapidDevelopmentQuartermaster && perk == DefaultPerks.Trade.RapidDevelopment)
            {
                checkSecondaryRole = true;
            }

            return party.HasPerk(perk, checkSecondaryRole);
        }
    }

    /// <summary>
    /// Masterwork is an engineer perk, but vanilla scales it with the party leader's Engineering. This is the only skill
    /// read in GetSiegeEngineDamage.
    /// </summary>
    [HarmonyPatch(typeof(DefaultSiegeEventModel), nameof(DefaultSiegeEventModel.GetSiegeEngineDamage))]
    internal static class MasterworkEngineerSkillPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
            RolePerks.ReplaceCalls(instructions,
                AccessTools.Method(typeof(Hero), nameof(Hero.GetSkillValue), new[] { typeof(SkillObject) }),
                AccessTools.Method(typeof(MasterworkEngineerSkillPatch), nameof(GetSkillValue)));

        private static int GetSkillValue(Hero leader, SkillObject skill)
        {
            MobileParty party = leader?.PartyBelongedTo;
            if ((PerkChanges.MasterworkEngineerSkill || PerkChanges.LeaderOrRoleHero) && skill == DefaultSkills.Engineering
                && party != null && party.LeaderHero == leader)
            {
                return RolePerks.Skill(party, PartyRole.Engineer, skill);
            }

            return leader.GetSkillValue(skill);
        }
    }

    /// <summary>
    /// Port Authority / Blessings of the Sea and Stormrider / Master and Commander are perk pairs with the same +1 ship
    /// limit. When two heroes can supply a role (leader and role hero, or a role moved for only one of the pair), each
    /// pair still gives +1.
    /// </summary>
    [HarmonyPatch]
    internal static class ShipLimitPairPatch
    {
        private static MethodBase Target => FirstMate.NavalMethod("NavalDLC.GameComponents.NavalDLCShipDeploymentModel", "GetShipDeploymentLimit");

        private static bool Prepare() => Target != null;

        private static MethodBase TargetMethod() => Target;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
            RolePerks.ReplaceCalls(instructions,
                AccessTools.Method(typeof(PerkHelper), nameof(PerkHelper.AddPerkBonusForParty)),
                AccessTools.Method(typeof(ShipLimitPairPatch), nameof(AddPerkBonusForParty)));

        private static void AddPerkBonusForParty(PerkObject perk, MobileParty party, bool isPrimaryBonus, ref ExplainedNumber stat, bool shouldApplyNavalMultiplier)
        {
            if (Paired(perk, PerkChanges.PortAuthority, PerkChanges.BlessingsOfTheSea, party, isPrimaryBonus)
                || Paired(perk, PerkChanges.Stormrider, PerkChanges.MasterAndCommander, party, isPrimaryBonus))
            {
                return;
            }

            PerkHelper.AddPerkBonusForParty(perk, party, isPrimaryBonus, ref stat, shouldApplyNavalMultiplier);
        }

        private static bool Paired(PerkObject perk, PerkObject first, PerkObject second, MobileParty party, bool isPrimaryBonus) =>
            perk != null && perk == second && first != null && party != null && party.HasPerk(first, !isPrimaryBonus);
    }

    /// <summary>
    /// War Sails reads four first-mate effects through PerkHelper.AddPerkBonusFromCaptain, which only accepts captain
    /// perks: Streamlined Operations, Smooth Operator and Accuracy Training never apply, and Special Arrows (a captain
    /// perk whose secondary effect is first mate) adds its captain value (+5) to sail fire damage instead of +40%.
    /// These patches read the first mate of the acting agent's party instead.
    /// </summary>
    internal static class FirstMate
    {
        internal static MobileParty PartyOf(Agent agent)
        {
            return (agent?.Origin?.BattleCombatant as PartyBase)?.MobileParty;
        }

        /// <summary>The first-mate half of the perk, if the party's first mate (or, with that option, leader) has it.</summary>
        internal static bool TryGetBonus(MobileParty party, PerkObject perk, out float bonus, out EffectIncrementType type)
        {
            bonus = 0f;
            type = EffectIncrementType.Invalid;
            if (party == null || perk == null)
            {
                return false;
            }

            bool primary = perk.PrimaryRole == PartyRole.FirstMate;
            if (!primary && perk.SecondaryRole != PartyRole.FirstMate)
            {
                return false;
            }

            if (!party.HasPerk(perk, !primary))
            {
                return false;
            }

            bonus = primary ? perk.PrimaryBonus : perk.SecondaryBonus;
            type = primary ? perk.PrimaryIncrementType : perk.SecondaryIncrementType;
            return true;
        }

        /// <summary>Adds the bonus to a model result whose own ExplainedNumber started from <paramref name="baseValue"/>.</summary>
        internal static float Apply(float result, float baseValue, float bonus, EffectIncrementType type)
        {
            switch (type)
            {
                case EffectIncrementType.AddFactor:
                    return result + baseValue * bonus;
                case EffectIncrementType.Add:
                    return result + bonus;
                default:
                    return result;
            }
        }

        internal static MethodBase NavalMethod(string typeName, string method)
        {
            Type type = AccessTools.TypeByName(typeName);
            return type == null ? null : AccessTools.DeclaredMethod(type, method);
        }
    }

    internal static class BaseModelOf<T> where T : GameModel
    {
        private static readonly Func<MBGameModel<T>, T> Getter =
            AccessTools.MethodDelegate<Func<MBGameModel<T>, T>>(AccessTools.PropertyGetter(typeof(MBGameModel<T>), "BaseModel"));

        internal static T Get(object model) => model is MBGameModel<T> m ? Getter(m) : null;
    }

    [HarmonyPatch]
    internal static class StreamlinedOperationsFirstMatePatch
    {
        private static MethodBase Target => FirstMate.NavalMethod("NavalDLC.GameComponents.NavalMissionSiegeEngineCalculationModel", "CalculateReloadSpeed");

        private static bool Prepare() => Target != null;

        private static MethodBase TargetMethod() => Target;

        private static void Postfix(object __instance, Agent userAgent, float baseSpeed, ref float __result)
        {
            if (Mission.Current?.IsNavalBattle != true
                || !FirstMate.TryGetBonus(FirstMate.PartyOf(userAgent), PerkChanges.FirstMateStreamlinedOperations, out float bonus, out EffectIncrementType type))
            {
                return;
            }

            MissionSiegeEngineCalculationModel baseModel = BaseModelOf<MissionSiegeEngineCalculationModel>.Get(__instance);
            if (baseModel != null)
            {
                __result = FirstMate.Apply(__result, baseModel.CalculateReloadSpeed(userAgent, baseSpeed), bonus, type);
            }
        }
    }

    [HarmonyPatch]
    internal static class SmoothOperatorFirstMatePatch
    {
        private static MethodBase Target => FirstMate.NavalMethod("NavalDLC.GameComponents.NavalMissionSiegeEngineCalculationModel", "CalculateShipSiegeWeaponAmmoCount");

        private static bool Prepare() => Target != null;

        private static MethodBase TargetMethod() => Target;

        private static void Postfix(object shipOrigin, Agent captain, RangedSiegeWeapon weapon, ref int __result)
        {
            if (!(weapon is Ballista))
            {
                return;
            }

            MobileParty party = FirstMate.PartyOf(captain)
                ?? (Traverse.Create(shipOrigin).Property("Owner").GetValue() as PartyBase)?.MobileParty;
            if (FirstMate.TryGetBonus(party, PerkChanges.FirstMateSmoothOperator, out float bonus, out EffectIncrementType type))
            {
                __result = MathF.Ceiling(FirstMate.Apply(__result, weapon.AmmoCount, bonus, type));
            }
        }
    }

    [HarmonyPatch]
    internal static class AccuracyTrainingFirstMatePatch
    {
        private static MethodBase Target => FirstMate.NavalMethod("NavalDLC.GameComponents.NavalAgentApplyDamageModel", "ApplyDamageAmplifications");

        private static bool Prepare() => Target != null;

        private static MethodBase TargetMethod() => Target;

        private static void Postfix(object __instance, ref AttackInformation attackInformation, ref AttackCollisionData collisionData, float baseDamage, ref float __result)
        {
            PerkObject perk = PerkChanges.FirstMateAccuracyTraining;
            if (perk == null || Mission.Current?.IsNavalBattle != true
                || !(collisionData.AttackBlockedWithShield || collisionData.CollidedWithShieldOnBack))
            {
                return;
            }

            WeaponComponentData weapon = attackInformation.AttackerWeapon.CurrentUsageItem;
            SkillObject skill = weapon?.RelevantSkill;
            if (weapon == null || !weapon.IsConsumable
                || (skill != DefaultSkills.Bow && skill != DefaultSkills.Crossbow && skill != DefaultSkills.Throwing))
            {
                return;
            }

            Agent attacker = attackInformation.IsAttackerAgentMount ? attackInformation.AttackerAgent?.RiderAgent : attackInformation.AttackerAgent;
            if (!(attacker?.Character is CharacterObject)
                || !FirstMate.TryGetBonus(FirstMate.PartyOf(attacker), perk, out float bonus, out EffectIncrementType type))
            {
                return;
            }

            AgentApplyDamageModel baseModel = BaseModelOf<AgentApplyDamageModel>.Get(__instance);
            if (baseModel != null)
            {
                __result = FirstMate.Apply(__result, baseModel.ApplyDamageAmplifications(in attackInformation, in collisionData, baseDamage), bonus, type);
            }
        }
    }

    [HarmonyPatch]
    internal static class SpecialArrowsFirstMatePatch
    {
        // IShipOrigin isn't in a referenced assembly, so the base call goes through the abstract declaration by reflection.
        private static readonly MethodInfo SailFireDamage = AccessTools.Method(typeof(AgentApplyDamageModel), "CalculateSailFireDamage");

        private static MethodBase Target => FirstMate.NavalMethod("NavalDLC.GameComponents.NavalAgentApplyDamageModel", "CalculateSailFireDamage");

        private static bool Prepare() => Target != null;

        private static MethodBase TargetMethod() => Target;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo fromCaptain = AccessTools.Method(typeof(PerkHelper), nameof(PerkHelper.AddPerkBonusFromCaptain));
            MethodInfo replacement = AccessTools.Method(typeof(SpecialArrowsFirstMatePatch), nameof(AddPerkBonusFromCaptain));
            foreach (CodeInstruction code in instructions)
            {
                yield return code.Calls(fromCaptain)
                    ? new CodeInstruction(OpCodes.Call, replacement).WithLabels(code.labels).WithBlocks(code.blocks)
                    : code;
            }
        }

        private static void AddPerkBonusFromCaptain(PerkObject perk, CharacterObject captainCharacter, ref ExplainedNumber bonuses)
        {
            if (perk != null && perk == PerkChanges.FirstMateSpecialArrows)
            {
                return;
            }

            PerkHelper.AddPerkBonusFromCaptain(perk, captainCharacter, ref bonuses);
        }

        private static void Postfix(object __instance, Agent agent, object shipOrigin, float baseDamage, bool damageFromShipMachine, ref float __result)
        {
            if (damageFromShipMachine
                || !FirstMate.TryGetBonus(FirstMate.PartyOf(agent), PerkChanges.FirstMateSpecialArrows, out float bonus, out EffectIncrementType type))
            {
                return;
            }

            AgentApplyDamageModel baseModel = BaseModelOf<AgentApplyDamageModel>.Get(__instance);
            if (baseModel != null)
            {
                float baseValue = (float)SailFireDamage.Invoke(baseModel, new[] { agent, shipOrigin, baseDamage, (object)damageFromShipMachine });
                __result = FirstMate.Apply(__result, baseValue, bonus, type);
            }
        }
    }
}
