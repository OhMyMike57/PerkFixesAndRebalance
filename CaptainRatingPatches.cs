using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Inject Land or Naval into captain-rating queries so perks tagged with those bits only score in context.
    /// </summary>
    [HarmonyPatch(typeof(DefaultBattleCaptainModel), nameof(DefaultBattleCaptainModel.GetCaptainRatingForTroopUsages))]
    internal static class CaptainRatingPatch
    {
        private static void Prefix(Hero hero, ref TroopUsageFlags flag)
        {
            flag = TroopUsageBits.WithBattleContext(flag, hero);
        }
    }
}
