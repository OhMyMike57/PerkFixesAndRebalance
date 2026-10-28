using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Helpers;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Fortitude Tonic's "+HP to other heroes in your party" belongs to the surgeon instead of the party leader.
    /// Every hero in the party except the surgeon gets it, including the player when a companion is surgeon.
    /// The personal +HP half is unchanged.
    /// </summary>
    [HarmonyPatch(typeof(DefaultCharacterStatsModel), nameof(DefaultCharacterStatsModel.MaxHitpoints))]
    internal static class FortitudeTonicSurgeonHitpointsPatch
    {
        // Vanilla pays every hero except the party leader; the recipients should be everyone except the surgeon.
        private static void Postfix(CharacterObject character, ref ExplainedNumber __result)
        {
            PerkObject perk = DefaultPerks.Medicine.FortitudeTonic;
            if (perk == null || perk.PrimaryRole != PartyRole.Surgeon || character == null || !character.IsHero)
            {
                return;
            }

            Hero hero = character.HeroObject;
            MobileParty party = hero?.PartyBelongedTo;
            Hero surgeon = party?.EffectiveSurgeon;
            // Same check vanilla uses for its payout, so the two only differ in who is left out.
            if (surgeon == null || surgeon == party.LeaderHero || !party.HasPerk(perk))
            {
                return;
            }

            if (hero == party.LeaderHero)
            {
                __result.Add(perk.PrimaryBonus, perk.Name);
            }
            else if (hero == surgeon)
            {
                __result.Add(-perk.PrimaryBonus, perk.Name);
            }
        }
    }

    /// <summary>
    /// Minister of Health as a surgeon perk: troop HP comes from the surgeon's perk and Medicine above 250, at the perk's
    /// configured HP per point. Vanilla reads the party leader's perk and skill.
    /// </summary>
    [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), nameof(SandboxAgentStatCalculateModel.GetEffectiveMaxHealth))]
    internal static class MinisterOfHealthSurgeonHitpointsPatch
    {
        private static void Postfix(Agent agent, ref float __result)
        {
            PerkObject perk = DefaultPerks.Medicine.MinisterOfHealth;
            if (perk == null || perk.PrimaryRole != PartyRole.Surgeon || agent == null || agent.IsHero || !agent.IsHuman)
            {
                return;
            }

            if (!(agent.Character is CharacterObject))
            {
                return;
            }

            MobileParty party = (agent.Origin?.BattleCombatant as PartyBase)?.MobileParty;
            Hero leader = party?.LeaderHero;
            if (leader == null)
            {
                return;
            }

            // Undo what vanilla already added from the leader, then add the surgeon's share.
            __result += SurgeonBonus(party, perk) - Bonus(leader.GetPerkValue(perk), leader.GetSkillValue(DefaultSkills.Medicine), perk);
        }

        /// <summary>From the surgeon, or with the leader-or-role-hero option either one at the higher Medicine.</summary>
        internal static int SurgeonBonus(MobileParty party, PerkObject perk) =>
            Bonus(party.HasPerk(perk), RolePerks.Skill(party, PartyRole.Surgeon, DefaultSkills.Medicine), perk);

        internal static int Bonus(bool hasPerk, int medicine, PerkObject perk)
        {
            if (!hasPerk)
            {
                return 0;
            }

            int above = MathF.Max(medicine - Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus, 0);
            int bonus = (int)(above * perk.PrimaryBonus);
            return bonus > 0 ? bonus : 0;
        }
    }

    /// <summary>
    /// Doctor's Oath adds the enemy surgeon's Medicine to your casualties' survival. With the leader-or-role-hero option,
    /// the leader can be the one with the perk, so use the higher Medicine of surgeon and leader, like other role skills.
    /// </summary>
    internal static class DoctorsOath
    {
        internal static bool TryGetMedicine(MobileParty enemyParty, out int medicine)
        {
            medicine = 0;
            if (!PerkChanges.LeaderOrRoleHero || enemyParty?.GetEffectiveRoleHolder(PartyRole.Surgeon) == null)
            {
                return false;
            }

            medicine = RolePerks.Skill(enemyParty, PartyRole.Surgeon, DefaultSkills.Medicine);
            return true;
        }

        internal static TextObject RoleText(ref ExplainedNumber explainedNumber) =>
            explainedNumber.IncludeDescriptions ? GameTexts.FindText("role", PartyRole.Surgeon.ToString()) : null;
    }

    [HarmonyPatch(typeof(DefaultPartyHealingModel), "AddDoctorsOathSkillBonusForParty")]
    internal static class DoctorsOathVanillaSkillPatch
    {
        private static bool Prefix(MobileParty enemyParty, ref ExplainedNumber explainedNumber)
        {
            if (!DoctorsOath.TryGetMedicine(enemyParty, out int medicine))
            {
                return true;
            }

            bool isPlayerEvent = enemyParty.MapEvent?.IsPlayerMapEvent ?? false;
            float value = DefaultSkillEffects.SurgeonSurvivalBonus.GetSkillEffectValue(medicine);
            explainedNumber.Add(value * (isPlayerEvent ? 1f : 0.1f), DoctorsOath.RoleText(ref explainedNumber));
            return false;
        }
    }

    /// <summary>
    /// Fought battles award Doctor's Oath surgery XP inside GetSurvivalChance, called from
    /// SandboxAgentDecideKilledOrUnconsciousModel when a troop is downed. RBM's prefix replaces that
    /// method and drops OnSurgeryApplied. Earlier hit and removal hooks never saw the striker: the blow
    /// is not fatal yet, and siege OnAgentRemoved often has a null affector. This model method is the
    /// one that runs for every downed troop. Mounted killers are resolved here; the model itself only
    /// passes a party when the affector is already human, so cavalry downs never reached GetSurvivalChance's XP.
    /// </summary>
    [HarmonyPatch(typeof(SandboxAgentDecideKilledOrUnconsciousModel), nameof(SandboxAgentDecideKilledOrUnconsciousModel.GetAgentStateProbability))]
    internal static class DoctorsOathCombatSurgeryXpPatch
    {
        private static void Postfix(Agent affectorAgent, Agent effectedAgent)
        {
            if (!DoctorsOathRbmSkillPatch.RbmActive() || Campaign.Current == null
                || effectedAgent == null || !effectedAgent.IsHuman
                || !(effectedAgent.Character is CharacterObject troop) || troop.IsHero)
            {
                return;
            }

            Agent killer = affectorAgent;
            if (killer != null && !killer.IsHuman)
            {
                killer = killer.RiderAgent;
            }

            if (killer == null || !killer.IsHuman || killer.Team == null || effectedAgent.Team == null
                || killer.Team.Side == effectedAgent.Team.Side
                || !(killer.Origin?.BattleCombatant is PartyBase partyBase) || !partyBase.IsMobile)
            {
                return;
            }

            MobileParty party = partyBase.MobileParty;
            if (party == null || !party.HasPerk(DefaultPerks.Medicine.DoctorsOath))
            {
                return;
            }

            // Vanilla awards 5 × tier here, before it knows killed vs wounded.
            SkillLevelingManager.OnSurgeryApplied(party, surgerySuccess: false, troop.Tier);
        }
    }

    /// <summary>
    /// RBM replaces GetSurvivalChance and keeps the Doctor's Oath survival bonus but drops OnSurgeryApplied.
    /// Fought battles are paid from <see cref="DoctorsOathCombatSurgeryXpPatch"/>. Auto-resolve never calls that
    /// model, so restore the side-effect here only while no mission is running.
    /// </summary>
    [HarmonyPatch]
    internal static class DoctorsOathRbmSkillPatch
    {
        private static MethodBase Target
        {
            get
            {
                Type type = AccessTools.TypeByName("RBMCombat.CampaignChanges+OverrideDefaultPartyHealingModel");
                return type == null ? null : AccessTools.Method(type, "PrefixGetAiWeight");
            }
        }

        private static bool Prepare()
        {
            OverridesSurvival = Target != null;
            return OverridesSurvival;
        }

        internal static bool OverridesSurvival { get; private set; }

        private static bool _resolved;

        /// <summary>
        /// Prepare runs at mod load, before RBM may be loaded, so look once more after all modules are up
        /// (<see cref="Resolve"/> at game start). The lookup scans every loaded type (about 70 ms), so it must never
        /// run per downed troop. Without RBM this is a plain bool read.
        /// </summary>
        internal static bool RbmActive()
        {
            if (!_resolved)
            {
                Resolve();
            }

            return OverridesSurvival;
        }

        internal static void Resolve()
        {
            _resolved = true;
            if (!OverridesSurvival)
            {
                OverridesSurvival = Target != null;
            }
        }

        private static MethodBase TargetMethod() => Target;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo doctorsOath = AccessTools.PropertyGetter(typeof(DefaultPerks.Medicine), nameof(DefaultPerks.Medicine.DoctorsOath));
            MethodInfo original = AccessTools.Method(typeof(SkillHelper), nameof(SkillHelper.AddSkillBonusForParty));
            MethodInfo replacement = AccessTools.Method(typeof(DoctorsOathRbmSkillPatch), nameof(AddSkillBonus));
            bool afterPerkCheck = false;
            foreach (CodeInstruction code in instructions)
            {
                if (code.Calls(doctorsOath))
                {
                    afterPerkCheck = true;
                }
                else if (afterPerkCheck && code.Calls(original))
                {
                    afterPerkCheck = false;
                    yield return new CodeInstruction(OpCodes.Call, replacement).WithLabels(code.labels).WithBlocks(code.blocks);
                    continue;
                }

                yield return code;
            }
        }

        private static void AddSkillBonus(SkillEffect effect, MobileParty enemyParty, ref ExplainedNumber explainedNumber)
        {
            if (effect != DefaultSkillEffects.SurgeonSurvivalBonus || effect.IncrementType != EffectIncrementType.Add
                || !DoctorsOath.TryGetMedicine(enemyParty, out int medicine))
            {
                SkillHelper.AddSkillBonusForParty(effect, enemyParty, ref explainedNumber);
                return;
            }

            explainedNumber.Add(effect.GetSkillEffectValue(medicine), DoctorsOath.RoleText(ref explainedNumber));
        }

        private static void Postfix(CharacterObject character, PartyBase enemyParty)
        {
            // Fought battles pay from GetAgentStateProbability. This covers auto-resolve, where that model is not used.
            if (Mission.Current != null || character == null
                || enemyParty?.MobileParty == null
                || !enemyParty.MobileParty.HasPerk(DefaultPerks.Medicine.DoctorsOath))
            {
                return;
            }

            SkillLevelingManager.OnSurgeryApplied(enemyParty.MobileParty, surgerySuccess: false, character.Tier);
        }
    }

    /// <summary>
    /// RBM's auto-resolve troop HP (SimulationTroopHitPoints.BuildCommandedHealth) adds Minister of Health from the party
    /// leader whenever its simulation perk system is on. Swap that for the surgeon's share, like the mission patch above.
    /// </summary>
    [HarmonyPatch]
    internal static class MinisterOfHealthSurgeonSimulationPatch
    {
        private static Func<bool> _simulationPerksEnabled;

        private static MethodBase Target
        {
            get
            {
                Type type = AccessTools.TypeByName("RBMCampaign.SimulationTroopHitPoints");
                return type == null ? null : AccessTools.Method(type, "BuildCommandedHealth");
            }
        }

        private static bool Prepare()
        {
            if (Target == null)
            {
                return false;
            }

            MethodInfo getter = AccessTools.PropertyGetter(AccessTools.TypeByName("RBMCampaign.SimulationPerks"), "Enabled");
            if (getter == null)
            {
                return false;
            }

            _simulationPerksEnabled = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), getter);
            return true;
        }

        private static MethodBase TargetMethod() => Target;

        private static void Postfix(CharacterObject troop, PartyBase party, ref ExplainedNumber __result)
        {
            PerkObject perk = DefaultPerks.Medicine.MinisterOfHealth;
            if (perk == null || perk.PrimaryRole != PartyRole.Surgeon || troop == null || troop.IsHero)
            {
                return;
            }

            MobileParty mobileParty = party?.MobileParty;
            Hero leader = mobileParty?.LeaderHero;
            if (leader == null || !_simulationPerksEnabled())
            {
                return;
            }

            int delta = MinisterOfHealthSurgeonHitpointsPatch.SurgeonBonus(mobileParty, perk)
                - MinisterOfHealthSurgeonHitpointsPatch.Bonus(leader.GetPerkValue(perk), leader.GetSkillValue(DefaultSkills.Medicine), perk);
            if (delta != 0)
            {
                __result.Add(delta, perk.Name);
            }
        }
    }
}
