using System;
using System.Collections.Concurrent;
using System.Reflection;
using HarmonyLib;
using Helpers;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>A troop counts as a crossbow troop if any of its battle equipment sets carries a crossbow.</summary>
    internal static class CrossbowTroops
    {
        private static readonly ConcurrentDictionary<CharacterObject, bool> Cache = new ConcurrentDictionary<CharacterObject, bool>();

        public static void Clear() => Cache.Clear();

        public static bool Has(CharacterObject character)
        {
            return character != null && Cache.GetOrAdd(character, Compute);
        }

        private static bool Compute(CharacterObject character)
        {
            foreach (Equipment equipment in character.BattleEquipments)
            {
                if (equipment == null)
                {
                    continue;
                }

                for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.NumAllWeaponSlots; i++)
                {
                    if (equipment[i].Item?.ItemType == ItemObject.ItemTypeEnum.Crossbow)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Vanilla applies Picked Shots' wage discount to the whole tier 4+ ranged wage pool in one AddPerkBonusForParty call.
    /// Inside GetTotalWage the discount is scaled by the crossbow troops' share of that pool instead.
    /// </summary>
    [HarmonyPatch(typeof(DefaultPartyWageModel), nameof(DefaultPartyWageModel.GetTotalWage))]
    internal static class PickedShotsWageScopePatch
    {
        [ThreadStatic] internal static TroopRoster Roster;

        private static void Prefix(TroopRoster troopRoster, out TroopRoster __state)
        {
            __state = Roster;
            Roster = troopRoster;
        }

        private static void Finalizer(TroopRoster __state)
        {
            Roster = __state;
        }

        internal static float CrossbowShare(TroopRoster roster)
        {
            int ranged = 0;
            int crossbow = 0;
            for (int i = 0; i < roster.Count; i++)
            {
                TroopRosterElement element = roster.GetElementCopyAtIndex(i);
                CharacterObject character = element.Character;
                if (character == null || character.IsHero || !character.IsRanged || character.Tier < 4)
                {
                    continue;
                }

                int wage = character.TroopWage * element.Number;
                ranged += wage;
                if (CrossbowTroops.Has(character))
                {
                    crossbow += wage;
                }
            }

            return ranged > 0 ? (float)crossbow / ranged : 0f;
        }
    }

    /// <summary>Picked Shots' +HP to ranged troops only counts for crossbow troops, in missions and in RBM's auto-resolve.</summary>
    [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), nameof(SandboxAgentStatCalculateModel.GetEffectiveMaxHealth))]
    internal static class PickedShotsMissionHealthScopePatch
    {
        [ThreadStatic] internal static bool InScope;
        [ThreadStatic] internal static CharacterObject Troop;

        private static void Prefix(Agent agent, out (bool, CharacterObject) __state)
        {
            __state = (InScope, Troop);
            InScope = true;
            Troop = agent?.Character as CharacterObject;
        }

        private static void Finalizer((bool, CharacterObject) __state)
        {
            (InScope, Troop) = __state;
        }
    }

    [HarmonyPatch]
    internal static class PickedShotsSimulationHealthScopePatch
    {
        private static MethodBase Target
        {
            get
            {
                Type type = AccessTools.TypeByName("RBMCampaign.SimulationTroopHitPoints");
                return type == null ? null : AccessTools.Method(type, "BuildCommandedHealth");
            }
        }

        private static bool Prepare() => Target != null;

        private static MethodBase TargetMethod() => Target;

        private static void Prefix(CharacterObject troop, out (bool, CharacterObject) __state)
        {
            __state = (PickedShotsMissionHealthScopePatch.InScope, PickedShotsMissionHealthScopePatch.Troop);
            PickedShotsMissionHealthScopePatch.InScope = true;
            PickedShotsMissionHealthScopePatch.Troop = troop;
        }

        private static void Finalizer((bool, CharacterObject) __state)
        {
            (PickedShotsMissionHealthScopePatch.InScope, PickedShotsMissionHealthScopePatch.Troop) = __state;
        }
    }

    [HarmonyPatch(typeof(PerkHelper), nameof(PerkHelper.AddPerkBonusForParty))]
    internal static class PickedShotsPerkBonusPatch
    {
        private static bool Prefix(
            PerkObject perk,
            MobileParty party,
            bool isPrimaryBonus,
            ref ExplainedNumber stat,
            bool shouldApplyNavalMultiplier)
        {
            if (perk == null || perk != PerkChanges.RestrictedPickedShots)
            {
                return true;
            }

            if (!isPrimaryBonus)
            {
                return !PickedShotsMissionHealthScopePatch.InScope
                    || CrossbowTroops.Has(PickedShotsMissionHealthScopePatch.Troop);
            }

            TroopRoster roster = PickedShotsWageScopePatch.Roster;
            if (roster == null)
            {
                return true;
            }

            if (party == null || !party.HasPerk(perk))
            {
                return false;
            }

            float share = PickedShotsWageScopePatch.CrossbowShare(roster);
            if (share > 0f)
            {
                float bonus = perk.PrimaryBonus * share * (shouldApplyNavalMultiplier ? 0.5f : 1f);
                stat.AddFactor(bonus, perk.Name);
            }

            return false;
        }
    }
}
