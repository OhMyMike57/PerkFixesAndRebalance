using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Helpers;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// When a party has no army, treat its leader as the army commander for selected perks.
    /// Does not invent a commander for multi-party MapEvents that are not an army.
    /// </summary>
    internal static class ArmyCommander
    {
        internal static CharacterObject LeaderCharacter(MobileParty party)
        {
            if (party == null)
            {
                return null;
            }

            if (party.Army != null)
            {
                return party.Army.LeaderParty?.LeaderHero?.CharacterObject;
            }

            return PerkChanges.ArmyCommanderPerksWhenAlone
                ? party.LeaderHero?.CharacterObject
                : null;
        }

        internal static object MissionBehavior(string typeName)
        {
            Type type = AccessTools.TypeByName(typeName);
            if (type == null || Mission.Current == null)
            {
                return null;
            }

            MethodInfo open = AccessTools.Method(typeof(Mission), "GetMissionBehavior", Type.EmptyTypes);
            return open?.MakeGenericMethod(type).Invoke(Mission.Current, null);
        }

        /// <summary>
        /// PartyBase.FlagShip is Ships.MaxBy and throws when the party has no ships (every land-only party).
        /// </summary>
        internal static Ship FlagShipOrNull(PartyBase party)
        {
            return party?.Ships != null && party.Ships.Count > 0 ? party.FlagShip : null;
        }

        internal static bool AgentOnShip(Agent agent, Ship ship, bool bypassSteppedShipCheck)
        {
            object logic = MissionBehavior("NavalDLC.Missions.MissionLogics.NavalShipsLogic");
            if (logic == null || ship == null)
            {
                return false;
            }

            object allShips = Traverse.Create(logic).Property("AllShips").GetValue();
            if (!(allShips is System.Collections.IEnumerable ships))
            {
                return false;
            }

            foreach (object missionShip in ships)
            {
                object origin = Traverse.Create(missionShip).Property("ShipOrigin").GetValue();
                if (!ReferenceEquals(origin, ship))
                {
                    continue;
                }

                return Traverse.Create(missionShip).Method("GetIsAgentOnShip", agent, bypassSteppedShipCheck).GetValue<bool>();
            }

            return false;
        }
    }

    /// <summary>
    /// Vanilla ArmyCommander HasPerk is Army?.LeaderParty?.LeaderHero. When alone, use the party leader.
    /// </summary>
    [HarmonyPatch(typeof(MobileParty), nameof(MobileParty.HasPerk))]
    internal static class MobilePartyHasPerkArmyAlonePatch
    {
        private static bool Prefix(MobileParty __instance, PerkObject perk, bool checkSecondaryRole, ref bool __result)
        {
            if (!PerkChanges.ArmyCommanderPerksWhenAlone || perk == null || __instance.Army != null)
            {
                return true;
            }

            PartyRole role = checkSecondaryRole ? perk.SecondaryRole : perk.PrimaryRole;
            if (role != PartyRole.ArmyCommander)
            {
                return true;
            }

            __result = __instance.LeaderHero?.GetPerkValue(perk) ?? false;
            return false;
        }
    }

    /// <summary>
    /// Vanilla Mastery only runs for the army leader party. Party-leader mode also applies for lone / non-leader parties.
    /// </summary>
    [HarmonyPatch(typeof(DefaultCombatSimulationModel), "CalculateSimulationDamagePerkEffects")]
    internal static class TacticalMasteryPartyLeaderSimPatch
    {
        private static void Postfix(MobileParty strikerParty, ref ExplainedNumber effectiveDamage)
        {
            if (!PerkChanges.TacticalMasteryPartyLeader || strikerParty?.LeaderHero == null)
            {
                return;
            }

            // Army leader already got it from vanilla's army gate.
            if (strikerParty.Army != null && strikerParty.Army.LeaderParty == strikerParty)
            {
                return;
            }

            PerkHelper.AddEpicPerkBonusForCharacter(
                DefaultPerks.Tactics.TacticalMastery,
                strikerParty.LeaderHero.CharacterObject,
                DefaultSkills.Tactics,
                applyPrimaryBonus: true,
                ref effectiveDamage,
                Campaign.Current.Models.CharacterDevelopmentModel.MinSkillRequiredForEpicPerkBonus,
                strikerParty.IsCurrentlyAtSea);
        }
    }

    /// <summary>
    /// Great Leader start-of-battle morale reads Army.LeaderParty. When alone, use the party leader (same skip if the
    /// agent is that leader character).
    /// </summary>
    [HarmonyPatch(typeof(SandboxBattleMoraleModel), nameof(SandboxBattleMoraleModel.GetEffectiveInitialMorale))]
    internal static class GreatLeaderAloneMoralePatch
    {
        private static void Postfix(Agent agent, ref float __result)
        {
            if (!PerkChanges.ArmyCommanderPerksWhenAlone || agent?.Origin?.BattleCombatant is not PartyBase party
                || !party.IsMobile)
            {
                return;
            }

            MobileParty mobile = party.MobileParty;
            if (mobile?.Army != null || mobile.LeaderHero == null
                || !mobile.LeaderHero.GetPerkValue(DefaultPerks.Leadership.GreatLeader))
            {
                return;
            }

            if (agent.Character == mobile.LeaderHero.CharacterObject)
            {
                return;
            }

            __result += DefaultPerks.Leadership.GreatLeader.PrimaryBonus;
        }
    }

    /// <summary>
    /// Enemy of the Wood: same lookup as vanilla, but when the enemy party has no army use its party leader.
    /// </summary>
    [HarmonyPatch]
    internal static class EnemyOfTheWoodAlonePatch
    {
        private static MethodBase Target =>
            AccessTools.TypeByName("NavalDLC.GameComponents.NavalBattleMoraleModel") is Type t
                ? AccessTools.Method(t, "GetEnemyArmyLeaderCharacter")
                : null;

        private static bool Prepare() => Target != null;

        private static MethodBase TargetMethod() => Target;

        private static bool Prefix(object __instance, object shipOrigin, ref CharacterObject __result)
        {
            if (!PerkChanges.ArmyCommanderPerksWhenAlone)
            {
                return true;
            }

            object logic = Traverse.Create(__instance).Method("GetNavalShipsLogic").GetValue();
            if (logic == null || shipOrigin == null || Mission.Current == null)
            {
                return true;
            }

            MethodInfo find = AccessTools.Method(logic.GetType(), "FindAssignmentOfShipOrigin");
            if (find == null)
            {
                return true;
            }

            object[] args = { shipOrigin, null };
            find.Invoke(logic, args);
            object assignment = args[1];
            Formation formation = assignment == null
                ? null
                : Traverse.Create(assignment).Property("Formation").GetValue<Formation>();
            Agent witness = formation?.GetFirstUnit();
            if (witness?.Team == null)
            {
                __result = null;
                return false;
            }

            foreach (Team team in Mission.Current.Teams)
            {
                if (!team.IsEnemyOf(witness.Team) || team.ActiveAgents.Count <= 0)
                {
                    continue;
                }

                PartyBase enemyParty = team.ActiveAgents[0].Origin?.BattleCombatant as PartyBase;
                MobileParty mobile = enemyParty != null && enemyParty.IsMobile ? enemyParty.MobileParty : null;
                __result = ArmyCommander.LeaderCharacter(mobile);
                return false;
            }

            __result = null;
            return false;
        }
    }

    /// <summary>
    /// Commodore flagship figurehead for all crews: when alone, apply the flagship figurehead if the agent is not already
    /// on that ship (vanilla already applied the ship-they're-on bonus).
    /// </summary>
    [HarmonyPatch]
    internal static class CommodoreAloneFigureheadPatch
    {
        private static MethodBase Target =>
            AccessTools.TypeByName("NavalDLC.GameComponents.NavalAgentStatCalculateModel") is Type t
                ? AccessTools.Method(t, "InitializeAgentStatsAfterDeploymentFinished", new[] { typeof(Agent) })
                : null;

        private static MethodInfo ApplyFigureheadBonusesMethod =>
            AccessTools.TypeByName("NavalDLC.GameComponents.NavalAgentStatCalculateModel") is Type t
                ? AccessTools.Method(t, "ApplyFigureheadBonuses")
                : null;

        private static bool Prepare() => Target != null && ApplyFigureheadBonusesMethod != null;

        private static MethodBase TargetMethod() => Target;

        private static void Postfix(object __instance, Agent agent)
        {
            if (!PerkChanges.ArmyCommanderPerksWhenAlone || PerkChanges.Commodore == null || agent == null
                || !(agent.Origin?.BattleCombatant is PartyBase party) || !party.IsMobile)
            {
                return;
            }

            MobileParty mobile = party.MobileParty;
            if (mobile?.Army != null || mobile.LeaderHero == null
                || !mobile.LeaderHero.GetPerkValue(PerkChanges.Commodore))
            {
                return;
            }

            // Vanilla returns before figureheads when there is no NavalShipsLogic (land battles).
            if (ArmyCommander.MissionBehavior("NavalDLC.Missions.MissionLogics.NavalShipsLogic") == null)
            {
                return;
            }

            Ship flagShip = ArmyCommander.FlagShipOrNull(party);
            Figurehead figurehead = flagShip?.Figurehead;
            if (figurehead == null || ArmyCommander.AgentOnShip(agent, flagShip, bypassSteppedShipCheck: true))
            {
                return;
            }

            ApplyFigureheadBonusesMethod.Invoke(__instance, new object[] { agent, agent.AgentDrivenProperties, figurehead });
        }
    }

    /// <summary>
    /// Commodore Lion figurehead morale when alone (Naval GetEffectiveInitialMorale only checks army leader).
    /// Vanilla's per-ship loop already gave the Lion to agents standing on the flagship, so skip them.
    /// </summary>
    [HarmonyPatch]
    internal static class CommodoreAloneLionMoralePatch
    {
        private static MethodBase Target =>
            AccessTools.TypeByName("NavalDLC.GameComponents.NavalBattleMoraleModel") is Type t
                ? AccessTools.Method(t, "GetEffectiveInitialMorale", new[] { typeof(Agent), typeof(float) })
                : null;

        private static bool Prepare() => Target != null;

        private static MethodBase TargetMethod() => Target;

        private static void Postfix(Agent agent, ref float __result)
        {
            if (!PerkChanges.ArmyCommanderPerksWhenAlone || PerkChanges.Commodore == null || agent == null
                || !(agent.Origin?.BattleCombatant is PartyBase party) || !party.IsMobile)
            {
                return;
            }

            MobileParty mobile = party.MobileParty;
            if (mobile?.Army != null || mobile.LeaderHero == null
                || !mobile.LeaderHero.GetPerkValue(PerkChanges.Commodore))
            {
                return;
            }

            Ship flagShip = ArmyCommander.FlagShipOrNull(party);
            Figurehead figurehead = flagShip?.Figurehead;
            if (figurehead == null || figurehead != DefaultFigureheads.Lion
                || ArmyCommander.AgentOnShip(agent, flagShip, bypassSteppedShipCheck: false))
            {
                return;
            }

            __result += figurehead.EffectAmount;
        }
    }
}
