using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// One fight's Doctor's Oath trace. Written to Modules\PerkFixesAndRebalance\doctors_oath_xp.log.
    /// </summary>
    internal static class DoctorsOathXpLog
    {
        private static readonly object Gate = new object();
        private static string _path;

        internal static void Line(string text)
        {
            try
            {
                lock (Gate)
                {
                    if (_path == null)
                    {
                        _path = Path.Combine(BasePath.Name, "Modules", "PerkFixesAndRebalance", "doctors_oath_xp.log");
                    }

                    File.AppendAllText(_path, DateTime.Now.ToString("HH:mm:ss.fff ") + text + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                Debug.Print("[PerkFixesAndRebalance] doctors oath log failed: " + e.Message);
            }
        }

        internal static void BattleStart()
        {
            DoctorsOathCombatSurgeryXpPatch.ResetLogCount();
            DoctorsOathXpLogRemovalPatch.ResetLogCount();
            MobileParty main = Campaign.Current != null ? MobileParty.MainParty : null;
            Hero surgeon = main?.EffectiveSurgeon;
            bool perk = main != null && main.HasPerk(DefaultPerks.Medicine.DoctorsOath);
            string rate = "n/a";
            string skill = "n/a";
            string focus = "n/a";
            if (surgeon?.HeroDeveloper != null)
            {
                skill = surgeon.GetSkillValue(DefaultSkills.Medicine).ToString();
                focus = surgeon.HeroDeveloper.GetFocus(DefaultSkills.Medicine).ToString();
                rate = surgeon.HeroDeveloper.GetFocusFactor(DefaultSkills.Medicine).ToString("0.###");
            }

            Line("---- battle ----");
            Line("rbmAtLoad=" + DoctorsOathRbmSkillPatch.OverridesSurvival
                + " rbmNow=" + DoctorsOathRbmSkillPatch.RbmActive()
                + " mainParty=" + (main?.Name?.ToString() ?? "null")
                + " hasDoctorsOath=" + perk
                + " surgeon=" + (surgeon?.Name?.ToString() ?? "null")
                + " medicine=" + skill
                + " focus=" + focus
                + " learningRate=" + rate);
            if (surgeon != null && surgeon.HeroDeveloper != null
                && surgeon.HeroDeveloper.GetFocusFactor(DefaultSkills.Medicine) <= 0f)
            {
                Line("NOTE learning rate is 0, so Medicine XP is awarded and then thrown away. Skill is over the learning limit.");
            }
        }
    }
}
