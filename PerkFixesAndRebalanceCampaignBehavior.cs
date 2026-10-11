using TaleWorlds.CampaignSystem;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Perk InitializeAll can run before MCM has loaded saved settings, so flags like Tactics battle effects
    /// would stick at defaults. Re-apply once the session is up.
    /// </summary>
    internal sealed class PerkFixesAndRebalanceCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private static void OnSessionLaunched(CampaignGameStarter starter) => PerkChanges.ReapplyAll();
    }
}
