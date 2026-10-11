using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Helpers;
using SandBox.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// Vanilla SetPerkAndBannerEffectsOnAgent only adds these movement speed perks in the "no shield in the off hand" branch:
    /// personal Morning Exercise and Self Medication, and captain Morning Exercise, Shield Bearer, Fleet of Foot, Reckless Charge,
    /// Footwork, Form Fitting Armor and Sprint. Form Fitting Armor is then added again for tier 3+ infantry outside that branch.
    /// The transpiler calls <see cref="Adjust"/> on the speed ExplainedNumber right before it is stored in MaxSpeedMultiplier.
    /// </summary>
    [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "SetPerkAndBannerEffectsOnAgent")]
    internal static class MovementSpeedPatch
    {
        private static bool Prepare() => AccessTools.Method(typeof(SandboxAgentStatCalculateModel), "SetPerkAndBannerEffectsOnAgent") != null;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();
            MethodInfo setter = AccessTools.PropertySetter(typeof(AgentDrivenProperties), nameof(AgentDrivenProperties.MaxSpeedMultiplier));
            MethodInfo result = AccessTools.PropertyGetter(typeof(ExplainedNumber), nameof(ExplainedNumber.ResultNumber));

            int store = code.FindLastIndex(c => c.Calls(setter));
            int read = store < 0 ? -1 : code.FindLastIndex(store, c => c.Calls(result));
            if (read < 1 || (code[read - 1].opcode != OpCodes.Ldloca && code[read - 1].opcode != OpCodes.Ldloca_S))
            {
                Debug.Print("[PerkFixesAndRebalance] MovementSpeedPatch: MaxSpeedMultiplier store not found; movement speed fixes are off.");
                return code;
            }

            code.InsertRange(read, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(code[read - 1].opcode, code[read - 1].operand),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MovementSpeedPatch), nameof(Adjust))),
            });
            return code;
        }

        private static void Adjust(Agent agent, CharacterObject character, ref ExplainedNumber speed)
        {
            bool shieldFix = PerkChanges.ShieldMovementSpeedFix;
            bool formFittingOnce = PerkChanges.FormFittingArmorOnce;
            if ((!shieldFix && !formFittingOnce) || agent == null || character == null || agent.HasMount)
            {
                return;
            }

            Agent captainAgent = agent.Formation?.Captain;
            CharacterObject captain = captainAgent == agent ? null : captainAgent?.Character as CharacterObject;
            bool shielded = HoldsShield(agent);

            if (shieldFix && shielded)
            {
                PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.MorningExercise, character, true, ref speed, false);
                PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Medicine.SelfMedication, character, false, ref speed, false);
                if (captain != null)
                {
                    PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.MorningExercise, captain, ref speed);
                    PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.ShieldBearer, captain, ref speed);
                    PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.FleetOfFoot, captain, ref speed);
                    PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.RecklessCharge, captain, ref speed);
                    PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.Footwork, captain, ref speed);
                    if (!formFittingOnce && character.Tier >= 3)
                    {
                        PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.FormFittingArmor, captain, ref speed);
                    }

                    if (character.IsInfantry)
                    {
                        PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.Sprint, captain, ref speed);
                    }
                }
            }

            PerkObject formFitting = DefaultPerks.Athletics.FormFittingArmor;
            if (formFittingOnce && captain != null && character.Tier >= 3
                && formFitting.SecondaryRole == PartyRole.Captain && captain.GetPerkValue(formFitting))
            {
                int applied = (shielded ? 0 : 1) + (character.IsInfantry ? 1 : 0);
                if (applied != 1)
                {
                    speed.AddFactor((1 - applied) * formFitting.SecondaryBonus, formFitting.Name);
                }
            }
        }

        private static bool HoldsShield(Agent agent)
        {
            EquipmentIndex offhand = agent.GetOffhandWieldedItemIndex();
            return offhand != EquipmentIndex.None && agent.Equipment[offhand].Item?.PrimaryWeapon?.IsShield == true;
        }
    }
}
