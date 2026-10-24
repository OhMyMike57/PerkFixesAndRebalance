using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Ship trades call OnShipOwnerChanged to pay Merchant Prince. Vanilla uses governor.CurrentSettlement.Town
    /// with no null check. A governor assigned to the port but not standing in a settlement crashes the daily tick.
    /// Skipping this method drops only that perk gold. The sale has already completed.
    /// </summary>
    [HarmonyPatch]
    internal static class MerchantPrinceGovernorPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                AccessTools.TypeByName("NavalDLC.CampaignBehaviors.ShipTradeCampaignBehavior"),
                "OnShipOwnerChanged");
        }

        private static bool Prepare() => TargetMethod() != null;

        private static bool Prefix(Ship ship, PartyBase oldOwner, ChangeShipOwnerAction.ShipOwnerChangeDetail details)
        {
            if (details != ChangeShipOwnerAction.ShipOwnerChangeDetail.ApplyByTrade)
            {
                return true;
            }

            Hero governor = null;
            if (oldOwner != null && oldOwner.IsSettlement)
            {
                governor = oldOwner.Settlement?.Town?.Governor;
            }
            else if (ship?.Owner != null && ship.Owner.IsSettlement)
            {
                governor = ship.Owner.Settlement?.Town?.Governor;
            }

            if (governor != null && (governor.CurrentSettlement == null || governor.CurrentSettlement.Town == null))
            {
                return false;
            }

            return true;
        }
    }
}
