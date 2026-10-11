using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Synthetic <see cref="TroopUsageFlags"/> bits for land vs sea captain scoring and UI.
    /// Vanilla formations never set these; PerkFixesAndRebalance ORs the active context into
    /// <c>GetCaptainRatingForTroopUsages</c>. Perks that apply on both land and sea omit both bits
    /// (same convention as omitting OnFoot/Mounted or Melee/Ranged when either side works).
    /// </summary>
    public static class TroopUsageBits
    {
        public const TroopUsageFlags Land = (TroopUsageFlags)2048;
        public const TroopUsageFlags Naval = (TroopUsageFlags)4096;

        public static TroopUsageFlags WithBattleContext(TroopUsageFlags flags, bool naval) =>
            flags | (naval ? Naval : Land);

        public static TroopUsageFlags WithBattleContext(TroopUsageFlags flags, Hero hero = null) =>
            WithBattleContext(flags, IsNavalCaptainContext(hero));

        /// <summary>
        /// True for naval missions, naval map events, or when the hero's party (else main party) is at sea.
        /// </summary>
        public static bool IsNavalCaptainContext(Hero hero = null)
        {
            Mission mission = Mission.Current;
            if (mission != null)
            {
                return mission.IsNavalBattle || mission.IsNavalRaidBattle;
            }

            if (Campaign.Current == null)
            {
                return false;
            }

            MobileParty party = hero?.PartyBelongedTo ?? MobileParty.MainParty;
            MapEvent mapEvent = party?.MapEvent;
            if (mapEvent != null && mapEvent.IsNavalMapEvent)
            {
                return true;
            }

            if (PlayerEncounter.Battle != null && PlayerEncounter.Battle.IsNavalMapEvent
                && (party == null || party == MobileParty.MainParty))
            {
                return true;
            }

            return party != null && party.IsCurrentlyAtSea;
        }
    }
}
