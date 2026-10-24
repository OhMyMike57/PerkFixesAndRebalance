using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Helpers;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Vanilla GetEffectiveSkill only adds the captain Wrapped Handles, Strong Grip, Clean Thrust and Counterweight skill
    /// bonuses in its "no horse in equipment" branch. This adds them for troops with a horse, so every troop gets them.
    /// </summary>
    [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), nameof(SandboxAgentStatCalculateModel.GetEffectiveSkill))]
    internal static class CaptainWeaponSkillsForAllTroopsPatch
    {
        private static void Postfix(Agent agent, SkillObject skill, ref int __result)
        {
            if (!(agent?.Character is CharacterObject character) || !character.HasMount())
            {
                return;
            }

            CharacterObject captain = agent.Formation?.Captain?.Character as CharacterObject;
            if (captain == null || captain == character)
            {
                return;
            }

            ExplainedNumber bonus = new ExplainedNumber(0f);
            if (skill == DefaultSkills.OneHanded && PerkChanges.WrappedHandlesAllTroops)
            {
                PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.WrappedHandles, captain, ref bonus);
            }
            else if (skill == DefaultSkills.TwoHanded && PerkChanges.StrongGripAllTroops)
            {
                PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.StrongGrip, captain, ref bonus);
            }
            else if (skill == DefaultSkills.Polearm)
            {
                if (PerkChanges.CleanThrustAllTroops)
                {
                    PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.CleanThrust, captain, ref bonus);
                }

                if (PerkChanges.CounterweightAllTroops)
                {
                    PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.CounterWeight, captain, ref bonus);
                }
            }
            else
            {
                return;
            }

            __result += (int)bonus.ResultNumber;
        }
    }

    /// <summary>
    /// Vanilla gives Flexible Fighter's Bow, Crossbow and Throwing skill to infantry only, and the melee skills to
    /// anyone flagged ranged (archers and horse archers). Cavalry are mounted and not ranged, so they get neither.
    /// </summary>
    [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), nameof(SandboxAgentStatCalculateModel.GetEffectiveSkill))]
    internal static class FlexibleFighterCavalryPatch
    {
        private static void Postfix(Agent agent, SkillObject skill, ref int __result)
        {
            if (!(agent?.Character is CharacterObject character))
            {
                return;
            }

            CharacterObject captain = agent.Formation?.Captain?.Character as CharacterObject;
            __result += FlexibleFighterCavalryBonus.For(character, captain, skill);
        }
    }

    /// <summary>
    /// RBM's auto-resolve copies the same infantry / ranged split in SimulationPerks.SkillOf.
    /// </summary>
    [HarmonyPatch]
    internal static class FlexibleFighterCavalrySimulationPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.TypeByName("RBMCampaign.SimulationPerks"), "SkillOf");
        }

        private static bool Prepare() => TargetMethod() != null;

        private static void Postfix(CharacterObject __0, SkillObject __1, CharacterObject __2, ref int __result)
        {
            __result += FlexibleFighterCavalryBonus.For(__0, __2, __1);
        }
    }

    /// <summary>
    /// Foot-only scoring misses cavalry. Any-scoring already includes them. Add cavalry formations when the skill
    /// option is on and horse-archer scoring is off.
    /// </summary>
    [HarmonyPatch(typeof(PerkHelper), nameof(PerkHelper.GetCaptainPerksForTroopUsages))]
    internal static class FlexibleFighterCavalryScorePatch
    {
        private static void Postfix(TroopUsageFlags troopUsageFlags, ref IEnumerable<PerkObject> __result)
        {
            if (!PerkChanges.FlexibleFighterCavalry)
            {
                return;
            }

            if (!troopUsageFlags.HasAllFlags(TroopUsageFlags.Mounted | TroopUsageFlags.Melee)
                || (troopUsageFlags & TroopUsageFlags.Ranged) != 0)
            {
                return;
            }

            PerkObject perk = DefaultPerks.Throwing.FlexibleFighter;
            if (__result is ICollection<PerkObject> perks)
            {
                if (!perks.Contains(perk))
                {
                    perks.Add(perk);
                }

                return;
            }

            List<PerkObject> list = __result.ToList();
            if (!list.Contains(perk))
            {
                list.Add(perk);
            }

            __result = list;
        }
    }

    internal static class FlexibleFighterCavalryBonus
    {
        public static int For(BasicCharacterObject character, CharacterObject captain, SkillObject skill)
        {
            if (!PerkChanges.FlexibleFighterCavalry || character == null || captain == null || captain == character)
            {
                return 0;
            }

            if (!character.IsMounted || character.IsRanged)
            {
                return 0;
            }

            if (skill != DefaultSkills.Bow && skill != DefaultSkills.Crossbow && skill != DefaultSkills.Throwing)
            {
                return 0;
            }

            ExplainedNumber bonus = new ExplainedNumber(0f);
            PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.FlexibleFighter, captain, ref bonus);
            return (int)bonus.ResultNumber;
        }
    }
}
