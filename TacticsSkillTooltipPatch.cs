using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Extra Tactics skill-effect bullets for commanded-battle damage, matching vanilla
    /// Role: Description: ±{a0}% style and inserted next to the auto-resolve advantage line.
    /// </summary>
    [HarmonyPatch(typeof(SkillVM), nameof(SkillVM.CreateLists))]
    internal static class TacticsSkillTooltipPatch
    {
        private static void Postfix(SkillVM __instance)
        {
            if (!PerkChanges.TacticsBattleEffects || __instance.Skill != DefaultSkills.Tactics)
            {
                return;
            }

            Hero hero = Traverse.Create(__instance).Field("_heroItem").Property("Hero").GetValue<Hero>();
            int skillValue = hero?.GetSkillValue(DefaultSkills.Tactics) ?? __instance.Level;
            // Same display math as SkillHelper.GetEffectDescriptionForSkillLevel for AddFactor effects.
            float displayed = MathF.Abs(skillValue * TacticsBattle.SkillPerLevel * 100f);
            // Vanilla uses "0.0"; keep extra digits so 0.18 does not become 0.2.
            string a0 = displayed.ToString("0.##");
            TextObject role = GameTexts.FindText("role", PartyRole.PartyLeader.ToString());

            TextObject dealt = new TextObject(
                "{=PFx5_TacticsFightDealt}Damage dealt by troops in land battles you fight: +{a0}%");
            dealt.SetTextVariable("a0", a0);
            TextObject taken = new TextObject(
                "{=PFx5_TacticsFightTaken}Damage taken by troops in land battles you fight: -{a0}%");
            taken.SetTextVariable("a0", a0);

            // After the auto-resolve advantage bullet (index 0); flee stays after these.
            int insertAt = __instance.SkillEffects.Count > 0 ? 1 : 0;
            __instance.SkillEffects.Insert(insertAt, new BindingListStringItem($"• {role}: {dealt} "));
            __instance.SkillEffects.Insert(insertAt + 1, new BindingListStringItem($"• {role}: {taken} "));
        }
    }
}
