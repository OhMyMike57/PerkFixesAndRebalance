using System;
using HarmonyLib;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Vanilla OnCombatHit leaves explainedNumber at 1 for non-hero hits, so the commander gets Ceiling(0.02) = 1 Tactics
    /// XP per troop strike. Sim kills use 0.02 × GetXpReward on the affector party's leader. Top up fatal troop hits to
    /// that same basis. BattleAgentLogic supplies the affector party when Team.Leader (commander) is missing.
    /// </summary>
    internal static class TacticsXp
    {
        [ThreadStatic]
        private static PartyBase _affectorParty;

        internal static void SetAffectorParty(PartyBase party) => _affectorParty = party;

        internal static void ClearAffectorParty() => _affectorParty = null;

        internal static MobileParty PartyForKill(Hero commander)
        {
            if (_affectorParty != null && _affectorParty.IsMobile)
            {
                return _affectorParty.MobileParty;
            }

            return commander?.PartyBelongedTo;
        }
    }

    [HarmonyPatch(typeof(BattleAgentLogic), "EnemyHitReward")]
    internal static class TacticsXpAffectorPartyPatch
    {
        private static void Prefix(Agent affectorAgent)
        {
            TacticsXp.SetAffectorParty(affectorAgent?.Origin?.BattleCombatant as PartyBase);
        }

        private static void Postfix()
        {
            TacticsXp.ClearAffectorParty();
        }
    }

    [HarmonyPatch(typeof(DefaultSkillLevelingManager), nameof(DefaultSkillLevelingManager.OnCombatHit))]
    internal static class TacticsXpPatch
    {
        private static void Postfix(
            CharacterObject affectorCharacter,
            CharacterObject affectedCharacter,
            Hero commander,
            bool isTeamKill,
            bool isFatal)
        {
            if (isTeamKill || !isFatal || affectorCharacter == null || affectorCharacter.IsHero
                || affectedCharacter == null)
            {
                return;
            }

            // Same recipient as vanilla OnCombatHit / sim: commander party when present.
            bool vanillaGranted = commander != null && commander.PartyBelongedTo != null;
            MobileParty party = vanillaGranted
                ? commander.PartyBelongedTo
                : TacticsXp.PartyForKill(commander);
            if (party == null)
            {
                return;
            }

            // Match OnSimulationCombatKill: no Tactics from naval map events.
            if (party.MapEvent != null && party.MapEvent.IsNavalMapEvent)
            {
                return;
            }

            int reward = Campaign.Current.Models.PartyTrainingModel.GetXpReward(affectedCharacter);
            int desired = MathF.Ceiling(0.02f * reward);
            // Vanilla already applied Ceiling(0.02 * 1) = 1 when commander had a party.
            int extra = desired - (vanillaGranted ? 1 : 0);
            if (extra > 0)
            {
                SkillLevelingManager.OnTacticsUsed(party, extra);
            }
        }
    }
}
