using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    public class SubModule : MBSubModuleBase
    {
        public const string HarmonyId = "com.PerkFixesAndRebalance";
        public const string ModuleId = "PerkFixesAndRebalance";

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // Pass assembly explicitly: parameterless PatchAll() can NRE under BLSE.
            new Harmony(HarmonyId).PatchAll(typeof(SubModule).Assembly);
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter campaignStarter)
            {
                campaignStarter.AddBehavior(new PerkFixesAndRebalanceCampaignBehavior());
            }
        }
    }
}
