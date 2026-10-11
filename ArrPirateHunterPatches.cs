using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Arr! / Pirate Hunter captain XP: vanilla calls AddPerkBonusForParty with Captain role, but
    /// MobileParty.HasPerk has no Captain case, so the bonus never applies. Apply it like Inspiring Leader
    /// from the formation captain during GetXpFromHit, naval battles only.
    /// </summary>
    [HarmonyPatch]
    internal static class ArrPirateHunterCaptainXpPatch
    {
        private static MethodBase Target
        {
            get
            {
                Type type = AccessTools.TypeByName("NavalDLC.GameComponents.NavalDLCCombatXpModel");
                return type == null
                    ? null
                    : AccessTools.Method(type, "GetXpFromHit", new[]
                    {
                        typeof(CharacterObject), typeof(CharacterObject), typeof(CharacterObject),
                        typeof(PartyBase), typeof(int), typeof(bool), typeof(CombatXpModel.MissionTypeEnum)
                    });
            }
        }

        private static bool Prepare() => Target != null;

        private static MethodBase TargetMethod() => Target;

        private static void Postfix(
            CharacterObject attackerTroop,
            CharacterObject captain,
            PartyBase attackerParty,
            ref ExplainedNumber __result)
        {
            if (PerkChanges.ArrCaptainXp == null && PerkChanges.PirateHunterCaptainXp == null)
            {
                return;
            }

            if (attackerTroop == null || attackerTroop.IsHero || !IsNavalXpContext(attackerParty))
            {
                return;
            }

            CharacterObject captainCharacter = captain;
            if (captainCharacter == null || !captainCharacter.IsHero)
            {
                captainCharacter = attackerParty?.LeaderHero?.CharacterObject;
            }

            if (captainCharacter == null || !captainCharacter.IsHero)
            {
                return;
            }

            PerkObject arr = PerkChanges.ArrCaptainXp;
            if (arr != null && attackerTroop.IsMariner && captainCharacter.GetPerkValue(arr))
            {
                __result.AddFactor(arr.SecondaryBonus, arr.Name);
            }

            PerkObject pirateHunter = PerkChanges.PirateHunterCaptainXp;
            if (pirateHunter != null && attackerTroop.IsRegular && captainCharacter.GetPerkValue(pirateHunter))
            {
                __result.AddFactor(pirateHunter.SecondaryBonus, pirateHunter.Name);
            }
        }

        private static bool IsNavalXpContext(PartyBase attackerParty)
        {
            Mission mission = Mission.Current;
            if (mission != null && (mission.IsNavalBattle || mission.IsNavalRaidBattle))
            {
                return true;
            }

            if (attackerParty?.MapEvent != null && attackerParty.MapEvent.IsNavalMapEvent)
            {
                return true;
            }

            return attackerParty != null && attackerParty.IsMobile && attackerParty.MobileParty.IsCurrentlyAtSea;
        }
    }
}
