using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Helpers;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Vandal only counts for two-handed weapons, and Shattering Blow/Volley only in naval battles. Both the Sandbox and
    /// the War Sails strike models read these through PerkHelper inside CalculateAdjustedArmorForBlow, and War Sails calls
    /// the Sandbox model as its base, so the scope is saved and restored for nesting.
    /// </summary>
    [HarmonyPatch]
    internal static class ArmorPenetrationScopePatch
    {
        [ThreadStatic] internal static bool SuppressVandal;
        [ThreadStatic] internal static bool SuppressShattering;

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(SandboxStrikeMagnitudeModel), nameof(SandboxStrikeMagnitudeModel.CalculateAdjustedArmorForBlow));

            Type naval = AccessTools.TypeByName("NavalDLC.GameComponents.NavalStrikeMagnitudeModel");
            MethodInfo navalMethod = naval == null ? null : AccessTools.DeclaredMethod(naval, "CalculateAdjustedArmorForBlow");
            if (navalMethod != null)
            {
                yield return navalMethod;
            }
        }

        private static void Prefix(WeaponComponentData weaponComponent, out (bool, bool) __state)
        {
            __state = (SuppressVandal, SuppressShattering);
            SuppressVandal = PerkChanges.RestrictedVandal != null && weaponComponent?.RelevantSkill != DefaultSkills.TwoHanded;
            SuppressShattering = (PerkChanges.SeaOnlyShatteringBlow != null || PerkChanges.SeaOnlyShatteringVolley != null)
                && Mission.Current?.IsNavalBattle != true;
        }

        private static void Finalizer((bool, bool) __state)
        {
            (SuppressVandal, SuppressShattering) = __state;
        }

        internal static bool IsSuppressed(PerkObject perk)
        {
            if (perk == null)
            {
                return false;
            }

            if (SuppressVandal && perk == PerkChanges.RestrictedVandal)
            {
                return true;
            }

            return SuppressShattering && (perk == PerkChanges.SeaOnlyShatteringBlow || perk == PerkChanges.SeaOnlyShatteringVolley);
        }
    }

    [HarmonyPatch(typeof(PerkHelper), nameof(PerkHelper.AddPerkBonusForCharacter))]
    internal static class ArmorPenetrationCharacterBonusPatch
    {
        private static bool Prefix(PerkObject perk, bool isPrimaryBonus)
        {
            return !(isPrimaryBonus && ArmorPenetrationScopePatch.IsSuppressed(perk));
        }
    }

    [HarmonyPatch(typeof(PerkHelper), nameof(PerkHelper.AddPerkBonusFromCaptain))]
    internal static class ArmorPenetrationCaptainBonusPatch
    {
        private static bool Prefix(PerkObject perk)
        {
            return !ArmorPenetrationScopePatch.IsSuppressed(perk);
        }
    }
}
