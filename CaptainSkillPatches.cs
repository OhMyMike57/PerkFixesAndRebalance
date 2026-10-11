using HarmonyLib;
using Helpers;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
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
}
