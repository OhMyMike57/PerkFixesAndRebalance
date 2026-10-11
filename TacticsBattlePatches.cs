using HarmonyLib;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Experimental: mirror party-leader auto-resolve Tactics (and related) damage into commanded battles at the
    /// agreed fractions. Most effects are land-only (matching sim). Tactical Mastery also applies in naval battles at
    /// half strength, like vanilla autoresolve. Captain effects stay vanilla.
    /// </summary>
    internal static class TacticsBattle
    {
        internal const float SituationalScale = 0.5f;
        internal const float CoachingBattle = 0.01f;
        internal const float EliteReservesBattle = -0.04f;
        internal const float TrustedCommanderBattle = 0.04f;
        internal const float OnTheMarchSiegeAttack = 0.02f;
        // Sim is 10% each; half of that (5%) is too strong for field attacker / outnumbered, which are common.
        internal const float CounterOffensiveBattle = 0.02f;
        internal const float SkillPerLevel = 0.0001f;
        internal const float MasteryPerLevel = 0.0005f;
        internal const float PreBattlePerSkillPoint = 0.0001f; // 1% per 100 skill difference (tooltip)

        internal static bool Enabled => PerkChanges.TacticsBattleEffects;

        internal static MobileParty PartyOf(Agent agent) => FirstMate.PartyOf(agent);

        internal static bool IsLandBattleContext(MobileParty party)
        {
            if (party == null || party.IsCurrentlyAtSea)
            {
                return false;
            }

            Mission mission = Mission.Current;
            if (mission != null && (mission.IsNavalBattle || mission.IsNavalRaidBattle))
            {
                return false;
            }

            MapEvent mapEvent = party.MapEvent;
            return mapEvent == null || !mapEvent.IsNavalMapEvent;
        }

        internal static bool IsNavalBattleContext(MobileParty party)
        {
            if (party == null)
            {
                return false;
            }

            Mission mission = Mission.Current;
            if (mission != null && (mission.IsNavalBattle || mission.IsNavalRaidBattle))
            {
                return true;
            }

            if (party.MapEvent != null && party.MapEvent.IsNavalMapEvent)
            {
                return true;
            }

            return party.IsCurrentlyAtSea;
        }

        /// <summary>
        /// Tactical Mastery battle mirror. Full commanded rate on land; half at sea (same as sim).
        /// Vanilla/army-commander mode: army leader's own party only. Party-leader mode: any party whose leader has it.
        /// </summary>
        internal static float MasteryFactor(MobileParty party, bool reduceIncoming)
        {
            if (party?.LeaderHero == null
                || !party.LeaderHero.GetPerkValue(DefaultPerks.Tactics.TacticalMastery)
                || Campaign.Current?.Models?.CharacterDevelopmentModel == null)
            {
                return 0f;
            }

            if (!PerkChanges.TacticalMasteryPartyLeader
                && (party.Army == null || party.Army.LeaderParty != party))
            {
                return 0f;
            }

            float perLevel = MasteryPerLevel;
            if (IsNavalBattleContext(party))
            {
                perLevel *= 0.5f;
            }
            else if (!IsLandBattleContext(party))
            {
                return 0f;
            }

            int over = party.LeaderHero.GetSkillValue(DefaultSkills.Tactics)
                - Campaign.Current.Models.CharacterDevelopmentModel.MinSkillRequiredForEpicPerkBonus;
            if (over <= 0)
            {
                return 0f;
            }

            float factor = over * perLevel;
            return reduceIncoming ? -factor : factor;
        }

        internal static float Scale(float damage, float factorSum)
        {
            if (factorSum == 0f)
            {
                return damage;
            }

            ExplainedNumber n = new ExplainedNumber(damage, false, null);
            n.AddFactor(factorSum);
            return n.ResultNumber;
        }
    }

    [HarmonyPatch(typeof(SandboxAgentApplyDamageModel), nameof(SandboxAgentApplyDamageModel.ApplyDamageAmplifications))]
    internal static class TacticsBattleDealtPatch
    {
        private static void Postfix(ref AttackInformation attackInformation, ref float __result)
        {
            if (!TacticsBattle.Enabled)
            {
                return;
            }

            Agent attackerAgent = attackInformation.IsAttackerAgentMount
                ? attackInformation.AttackerAgent?.RiderAgent
                : attackInformation.AttackerAgent;
            Agent victimAgent = attackInformation.IsVictimAgentMount
                ? attackInformation.VictimAgent?.RiderAgent
                : attackInformation.VictimAgent;

            CharacterObject attacker = (attackInformation.IsAttackerAgentMount
                ? attackInformation.AttackerRiderAgentCharacter
                : attackInformation.AttackerAgentCharacter) as CharacterObject;
            CharacterObject victim = (attackInformation.IsVictimAgentMount
                ? attackInformation.VictimRiderAgentCharacter
                : attackInformation.VictimAgentCharacter) as CharacterObject;

            if (attacker == null || victim == null)
            {
                return;
            }

            MobileParty striker = TacticsBattle.PartyOf(attackerAgent);
            MobileParty struck = TacticsBattle.PartyOf(victimAgent);
            if (striker == null || struck == null)
            {
                return;
            }

            bool land = TacticsBattle.IsLandBattleContext(striker);
            bool naval = TacticsBattle.IsNavalBattleContext(striker);
            if (!land && !naval)
            {
                return;
            }

            // Naval commanded battles: only Tactical Mastery (half), matching sim.
            if (!land)
            {
                float masteryOnly = TacticsBattle.MasteryFactor(striker, reduceIncoming: false);
                if (masteryOnly != 0f)
                {
                    __result = TacticsBattle.Scale(__result, masteryOnly);
                }

                return;
            }

            float factor = 0f;
            MapEvent battle = striker.MapEvent ?? struck.MapEvent;

            if (attacker.IsInfantry && victim.IsMounted && striker.HasPerk(DefaultPerks.Tactics.TightFormations))
            {
                factor += DefaultPerks.Tactics.TightFormations.PrimaryBonus * TacticsBattle.SituationalScale;
            }

            TerrainType terrain = Campaign.Current != null
                ? Campaign.Current.MapSceneWrapper.GetFaceTerrainType(striker.CurrentNavigationFace)
                : TerrainType.Plain;
            if ((terrain == TerrainType.Snow || terrain == TerrainType.Forest)
                && striker.HasPerk(DefaultPerks.Tactics.ExtendedSkirmish))
            {
                factor += DefaultPerks.Tactics.ExtendedSkirmish.PrimaryBonus * TacticsBattle.SituationalScale;
            }

            if ((terrain == TerrainType.Plain || terrain == TerrainType.Steppe || terrain == TerrainType.Desert)
                && striker.HasPerk(DefaultPerks.Tactics.DecisiveBattle))
            {
                factor += DefaultPerks.Tactics.DecisiveBattle.PrimaryBonus * TacticsBattle.SituationalScale;
            }

            if (!striker.IsBandit && struck.IsBandit && striker.HasPerk(DefaultPerks.Tactics.LawKeeper))
            {
                factor += DefaultPerks.Tactics.LawKeeper.PrimaryBonus * TacticsBattle.SituationalScale;
            }

            if (striker.HasPerk(DefaultPerks.Tactics.Coaching))
            {
                factor += TacticsBattle.CoachingBattle;
            }

            int strikerHealthy = striker.MemberRoster.TotalHealthyCount;
            int struckHealthy = struck.MemberRoster.TotalHealthyCount;
            if (strikerHealthy > struckHealthy && striker.HasPerk(DefaultPerks.Tactics.Encirclement))
            {
                factor += DefaultPerks.Tactics.Encirclement.PrimaryBonus * TacticsBattle.SituationalScale;
            }

            if (strikerHealthy < struckHealthy && striker.HasPerk(DefaultPerks.Tactics.Counteroffensive, checkSecondaryRole: true))
            {
                factor += TacticsBattle.CounterOffensiveBattle;
            }

            bool struckIsDefender = battle != null && IsOnSide(battle, struck, BattleSideEnum.Defender);
            bool strikerIsAttacker = battle != null && !struckIsDefender;

            if (battle != null && battle.IsSiegeAssault && strikerIsAttacker && striker.HasPerk(DefaultPerks.Tactics.Besieged))
            {
                factor += DefaultPerks.Tactics.Besieged.PrimaryBonus * TacticsBattle.SituationalScale;
            }

            if (struckIsDefender && striker.HasPerk(DefaultPerks.Scouting.Vanguard))
            {
                factor += DefaultPerks.Scouting.Vanguard.PrimaryBonus * TacticsBattle.SituationalScale;
            }

            if (battle != null && (battle.IsSiegeOutside || battle.IsSallyOut) && strikerIsAttacker
                && striker.HasPerk(DefaultPerks.Scouting.Rearguard, checkSecondaryRole: true))
            {
                factor += DefaultPerks.Scouting.Rearguard.SecondaryBonus * TacticsBattle.SituationalScale;
            }

            if (battle != null && battle.IsSallyOut && struckIsDefender
                && striker.HasPerk(DefaultPerks.Scouting.Vanguard, checkSecondaryRole: true))
            {
                factor += DefaultPerks.Scouting.Vanguard.SecondaryBonus * TacticsBattle.SituationalScale;
            }

            if (battle != null && battle.IsFieldBattle && strikerIsAttacker
                && striker.HasPerk(DefaultPerks.Tactics.Counteroffensive))
            {
                factor += TacticsBattle.CounterOffensiveBattle;
            }

            if (battle != null && battle.IsSiegeAssault && strikerIsAttacker && striker.HasPerk(DefaultPerks.Tactics.OnTheMarch))
            {
                factor += TacticsBattle.OnTheMarchSiegeAttack;
            }

            if (struck.IsBandit && striker.HasPerk(DefaultPerks.Scouting.Patrols, checkSecondaryRole: true))
            {
                factor += DefaultPerks.Scouting.Patrols.SecondaryBonus * TacticsBattle.SituationalScale;
            }

            Hero strikerLeader = striker.LeaderHero;
            if (strikerLeader != null)
            {
                factor += strikerLeader.GetSkillValue(DefaultSkills.Tactics) * TacticsBattle.SkillPerLevel;

                if (struck.LeaderHero != null && striker.HasPerk(DefaultPerks.Tactics.PreBattleManeuvers, checkSecondaryRole: true))
                {
                    int diff = strikerLeader.GetSkillValue(DefaultSkills.Tactics)
                        - struck.LeaderHero.GetSkillValue(DefaultSkills.Tactics);
                    if (diff > 0)
                    {
                        factor += diff * TacticsBattle.PreBattlePerSkillPoint;
                    }
                }

                factor += TacticsBattle.MasteryFactor(striker, reduceIncoming: false);
            }

            if (factor != 0f)
            {
                __result = TacticsBattle.Scale(__result, factor);
            }
        }

        private static bool IsOnSide(MapEvent battle, MobileParty party, BattleSideEnum side)
        {
            foreach (MapEventParty item in battle.PartiesOnSide(side))
            {
                if (item.Party == party.Party)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(SandboxAgentApplyDamageModel), nameof(SandboxAgentApplyDamageModel.ApplyDamageReductions))]
    internal static class TacticsBattleTakenPatch
    {
        private static void Postfix(ref AttackInformation attackInformation, ref float __result)
        {
            if (!TacticsBattle.Enabled)
            {
                return;
            }

            Agent attackerAgent = attackInformation.IsAttackerAgentMount
                ? attackInformation.AttackerAgent?.RiderAgent
                : attackInformation.AttackerAgent;
            Agent victimAgent = attackInformation.IsVictimAgentMount
                ? attackInformation.VictimAgent?.RiderAgent
                : attackInformation.VictimAgent;

            CharacterObject attacker = (attackInformation.IsAttackerAgentMount
                ? attackInformation.AttackerRiderAgentCharacter
                : attackInformation.AttackerAgentCharacter) as CharacterObject;
            CharacterObject victim = (attackInformation.IsVictimAgentMount
                ? attackInformation.VictimRiderAgentCharacter
                : attackInformation.VictimAgentCharacter) as CharacterObject;

            if (attacker == null || victim == null)
            {
                return;
            }

            MobileParty striker = TacticsBattle.PartyOf(attackerAgent);
            MobileParty struck = TacticsBattle.PartyOf(victimAgent);
            if (striker == null || struck == null)
            {
                return;
            }

            bool land = TacticsBattle.IsLandBattleContext(struck);
            bool naval = TacticsBattle.IsNavalBattleContext(struck);
            if (!land && !naval)
            {
                return;
            }

            if (!land)
            {
                float masteryOnly = TacticsBattle.MasteryFactor(struck, reduceIncoming: true);
                if (masteryOnly != 0f)
                {
                    __result = TacticsBattle.Scale(__result, masteryOnly);
                }

                return;
            }

            float factor = 0f;

            // Loose Formations: reduce ranged damage to infantry (sim adds negative factor on the hit).
            if (attacker.IsRanged && victim.IsInfantry && struck.HasPerk(DefaultPerks.Tactics.LooseFormations))
            {
                factor += DefaultPerks.Tactics.LooseFormations.PrimaryBonus * TacticsBattle.SituationalScale;
            }

            if (victim.Tier >= 3 && struck.HasPerk(DefaultPerks.Tactics.EliteReserves))
            {
                factor += TacticsBattle.EliteReservesBattle;
            }

            Hero struckLeader = struck.LeaderHero;
            if (struckLeader != null)
            {
                factor -= struckLeader.GetSkillValue(DefaultSkills.Tactics) * TacticsBattle.SkillPerLevel;
            }

            factor += TacticsBattle.MasteryFactor(struck, reduceIncoming: true);

            if (factor != 0f)
            {
                __result = TacticsBattle.Scale(__result, factor);
            }
        }
    }

    /// <summary>
    /// Trusted Commander secondary is sim-only in vanilla. Mirror 1/5 (+4%) for player-commanded land battles.
    /// </summary>
    [HarmonyPatch(typeof(DefaultPartyTrainingModel), nameof(DefaultPartyTrainingModel.CalculateXpGainFromBattles))]
    internal static class TrustedCommanderBattleXpPatch
    {
        private static void Postfix(FlattenedTroopRosterElement troopRosterElement, PartyBase party, ref ExplainedNumber __result)
        {
            if (!TacticsBattle.Enabled || party?.MobileParty == null || !party.IsMobile
                || party.MobileParty.IsCurrentlyAtSea || party.MapEvent == null)
            {
                return;
            }

            // Vanilla already applies the full bonus for AI / player simulation.
            if (party.MapEvent.IsPlayerSimulation || !party.MapEvent.IsPlayerMapEvent)
            {
                return;
            }

            if (!party.MobileParty.HasPerk(DefaultPerks.Leadership.TrustedCommander, checkSecondaryRole: true))
            {
                return;
            }

            __result.AddFactor(TacticsBattle.TrustedCommanderBattle, DefaultPerks.Leadership.TrustedCommander.Name);
        }
    }
}
