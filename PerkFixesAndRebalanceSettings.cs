using System.Runtime.CompilerServices;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;

namespace PerkFixesAndRebalance
{
    /// <summary>
    /// MCM menu: personal fighting and captain effects under Battle, every other role under Campaign, one switch for
    /// all text-only tooltip clarifications under Description fixes, and larger optional experiments under Experimental.
    /// MCM lists groups by GroupOrder ascending, then by name; a top-level group with no options of its own always has
    /// order -1, so Battle comes before Campaign by name. Changes re-apply when you click Done, or when a session starts.
    /// </summary>
    public sealed class PerkFixesAndRebalanceSettings : AttributeGlobalSettings<PerkFixesAndRebalanceSettings>
    {
        private const string Battle = "Battle";
        private const string ArmorPenetration = Battle + "|Armor penetration";
        private const string Shattering = ArmorPenetration + "|Shattering Blow/Volley (Mariner, 150)";
        private const string Vandal = ArmorPenetration + "|Vandal (Two-Handed, 225)";
        private const string MovementSpeed = Battle + "|Movement speed";
        private const string SkillBonuses = Battle + "|Captain skill bonuses for all troops";
        private const string Mariner = Battle + "|Mariner (War Sails)";
        private const string VanillaNoHorse = " Vanilla skips any troop with a horse in its equipment, so cavalry and horse archers never get it, even on foot.";
        private const string Campaign = "Campaign";
        private const string Crossbow = Campaign + "|Crossbow";
        private const string Trade = Campaign + "|Trade";
        private const string Medicine = Campaign + "|Medicine";
        private const string Engineering = Campaign + "|Engineering";
        private const string Tactics = Campaign + "|Tactics";
        private const string ArmyCommander = Campaign + "|Army commander";
        private const string Boatswain = Campaign + "|Boatswain (War Sails)";
        private const string Shipmaster = Campaign + "|Shipmaster (War Sails)";
        private const string Descriptions = "Description fixes";
        private const string Experimental = "Experimental";
        private const string LoadNote = " Applies when you click Done, or when a game is loaded.";

        public override string Id => "PerkFixesAndRebalance";
        public override string DisplayName => "Perk Fixes and Rebalance";
        public override string FolderName => "PerkFixesAndRebalance";
        public override string FormatType => "json2";
        public override char SubGroupDelimiter => '|';

        public override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            // MCM fires this after Done; session-start re-apply covers the case where InitializeAll ran before settings loaded.
            if ((propertyName == SaveTriggered || propertyName == LoadingComplete)
                && TaleWorlds.CampaignSystem.Campaign.Current != null)
            {
                PerkChanges.ReapplyAll();
            }
        }

        [SettingPropertyBool("Naval battles only", Order = 0, RequireRestart = false,
            HintText = "The melee (Blow) and ranged (Volley) armor penetration, personal and captain, only applies in naval battles. Vanilla applies it in every battle." + LoadNote)]
        [SettingPropertyGroup(Shattering, GroupOrder = 0)]
        public bool ShatteringNavalOnly { get; set; } = true;

        [SettingPropertyInteger("Armor penetration %", 0, 100, "0", Order = 1, RequireRestart = false,
            HintText = "Armor penetration from each perk, personal and captain. Vanilla is 50." + LoadNote)]
        [SettingPropertyGroup(Shattering)]
        public int ShatteringArmorPenetration { get; set; } = 25;

        [SettingPropertyBool("Two-handed weapons only", Order = 0, RequireRestart = false,
            HintText = "The armor penetration only applies to two-handed weapon attacks. Vanilla applies it to every attack, like a general bonus rather than a Two Handed one." + LoadNote)]
        [SettingPropertyGroup(Vandal, GroupOrder = 1)]
        public bool VandalTwoHandedOnly { get; set; } = true;

        [SettingPropertyInteger("Armor penetration %", 0, 100, "0", Order = 1, RequireRestart = false,
            HintText = "Armor penetration from Vandal. Vanilla is 25." + LoadNote)]
        [SettingPropertyGroup(Vandal)]
        public int VandalArmorPenetration { get; set; } = 25;

        [SettingPropertyBool("Movement speed perks work while holding a shield", Order = 0, RequireRestart = false,
            HintText = "Vanilla skips these while a troop has a shield in its off hand: captain Morning Exercise, Shield Bearer, Fleet of Foot, Reckless Charge, Footwork, Form Fitting Armor and Sprint, and personal Morning Exercise and Self Medication. This applies them with a shield too." + LoadNote)]
        [SettingPropertyGroup(MovementSpeed, GroupOrder = 1)]
        public bool ShieldMovementSpeedFix { get; set; } = true;

        [SettingPropertyBool("Form Fitting Armor (Athletics, 50): applies once", Order = 1, RequireRestart = false,
            HintText = "Vanilla adds the captain bonus twice to tier 3+ infantry without a shield (+8%), once to others without a shield, and not at all to non-infantry with a shield. This gives every tier 3+ unmounted troop +4%, as described." + LoadNote)]
        [SettingPropertyGroup(MovementSpeed)]
        public bool FormFittingArmorOnce { get; set; } = true;

        [SettingPropertyBool("Wrapped Handles (One-Handed, 25)", Order = 0, RequireRestart = false,
            HintText = "The captain +One Handed skill goes to every troop in the formation, mounted or on foot." + VanillaNoHorse + LoadNote)]
        [SettingPropertyGroup(SkillBonuses, GroupOrder = 2)]
        public bool WrappedHandlesAllTroops { get; set; } = true;

        [SettingPropertyBool("Strong Grip (Two-Handed, 25)", Order = 1, RequireRestart = false,
            HintText = "The captain +Two Handed skill goes to every troop in the formation, mounted or on foot." + VanillaNoHorse + LoadNote)]
        [SettingPropertyGroup(SkillBonuses)]
        public bool StrongGripAllTroops { get; set; } = true;

        [SettingPropertyBool("Clean Thrust (Polearm, 75)", Order = 2, RequireRestart = false,
            HintText = "The captain +Polearm skill goes to every troop in the formation, mounted or on foot." + VanillaNoHorse + LoadNote)]
        [SettingPropertyGroup(SkillBonuses)]
        public bool CleanThrustAllTroops { get; set; } = true;

        [SettingPropertyBool("Counterweight (Polearm, 250)", Order = 3, RequireRestart = false,
            HintText = "The captain +Polearm skill goes to every troop in the formation, mounted or on foot." + VanillaNoHorse + LoadNote)]
        [SettingPropertyGroup(SkillBonuses)]
        public bool CounterweightAllTroops { get; set; } = true;

        [SettingPropertyBool("Flexible Fighter (Throwing, 50): also score for horse archers", Order = 4, RequireRestart = false,
            HintText = "Order-of-battle only. Vanilla tags Foot (infantry and archers). Horse archers get the melee-skill half of the captain bonus but do not score. When on, the tag is Any so horse-archer formations score it too. Does not change who receives the bonus in battle." + LoadNote)]
        [SettingPropertyGroup(SkillBonuses)]
        public bool FlexibleFighterScoreHorseArchers { get; set; } = true;

        [SettingPropertyBool("Flexible Fighter (Throwing, 50): cavalry get Bow, Crossbow and Throwing", Order = 5, RequireRestart = false,
            HintText = "Cavalry get the same captain skill bonus as infantry. Archers and horse archers still get One Handed, Two Handed and Polearm only. Turn off for vanilla, where cavalry get nothing from this perk." + LoadNote)]
        [SettingPropertyGroup(SkillBonuses)]
        public bool FlexibleFighterCavalry { get; set; } = true;

        [SettingPropertyBool("Picked Shots (250): crossbow troops only", Order = 0, RequireRestart = false,
            HintText = "The -50% wage for tier 4+ ranged troops and the +5 HP for ranged troops only apply to troops that carry a crossbow." + LoadNote)]
        [SettingPropertyGroup(Crossbow, GroupOrder = 0)]
        public bool PickedShotsCrossbowOnly { get; set; } = true;

        [SettingPropertyBool("Artisan Community (125): recruit slot from quartermaster", Order = 0, RequireRestart = false,
            HintText = "The +1 recruitment slot with merchant notables comes from the quartermaster of the recruiting hero's party, as described. Vanilla reads the recruiting hero (usually the party leader)." + LoadNote)]
        [SettingPropertyGroup(Trade, GroupOrder = 2)]
        public bool ArtisanCommunityQuartermaster { get; set; } = true;

        [SettingPropertyBool("Rapid Development (175): price discount from quartermaster", Order = 1, RequireRestart = false,
            HintText = "The -25% price when buying clay, iron, raw silk and silver only needs the quartermaster to have the perk, as described. Vanilla also needs the clan leader to have it." + LoadNote)]
        [SettingPropertyGroup(Trade)]
        public bool RapidDevelopmentQuartermaster { get; set; } = true;

        [SettingPropertyBool("Sledges (75): mount HP from surgeon", Order = 0, RequireRestart = false,
            HintText = "The +15 mount HP comes from the party surgeon instead of the party leader." + LoadNote)]
        [SettingPropertyGroup(Medicine, GroupOrder = 3)]
        public bool SledgesSurgeon { get; set; } = true;

        [SettingPropertyBool("Fortitude Tonic (225): hero HP from surgeon", Order = 1, RequireRestart = false,
            HintText = "The party-wide +HP goes to every hero except the surgeon, instead of every hero except the leader." + LoadNote)]
        [SettingPropertyGroup(Medicine)]
        public bool FortitudeTonicSurgeon { get; set; } = true;

        [SettingPropertyBool("Minister of Health (275): troop HP from surgeon", Order = 2, RequireRestart = false,
            HintText = "Troop HP bonus reads the surgeon's perk and Medicine instead of the leader's, in battles and in RBM's auto-resolve." + LoadNote)]
        [SettingPropertyGroup(Medicine)]
        public bool MinisterOfHealthSurgeon { get; set; } = true;

        [SettingPropertyFloatingInteger("Minister of Health (275): HP per Medicine point above 250", 0.05f, 2f, "0.000", Order = 3, RequireRestart = false,
            HintText = "Troop HP per Medicine point above 250, from whoever holds the perk. Vanilla is 1. The default 0.675 gives +50 HP at 330 Medicine." + LoadNote)]
        [SettingPropertyGroup(Medicine)]
        public float MinisterOfHealthHitpointsPerSkillPoint { get; set; } = 0.675f;

        [SettingPropertyBool("Masterwork (275): skill from engineer", Order = 0, RequireRestart = false,
            HintText = "The siege engine damage scales with the Engineering of the engineer, who is the one that needs the perk. Vanilla uses the party leader's Engineering." + LoadNote)]
        [SettingPropertyGroup(Engineering, GroupOrder = 4)]
        public bool MasterworkEngineerSkill { get; set; } = true;

        [SettingPropertyBool("Tactical Mastery (275): party leader", Order = 0, RequireRestart = false,
            HintText = "Vanilla labels Mastery as army commander but only ever buffs the army leader's own party, and only while an army exists. This makes it a party leader perk so any party (including a lone one) gets it from its leader." + LoadNote)]
        [SettingPropertyGroup(Tactics, GroupOrder = 5)]
        public bool TacticalMasteryPartyLeader { get; set; } = true;

        [SettingPropertyBool("Army commander perks apply without an army", Order = 0, RequireRestart = false,
            HintText = "On The March, Great Leader (battle-start morale), Enemy of the Wood, and Commodore also work for a party that is not in an army, using that party's leader. Does not invent a commander for ad-hoc multi-party battles. Encirclement combat is already party leader; its cohesion discount still needs a real army." + LoadNote)]
        [SettingPropertyGroup(ArmyCommander, GroupOrder = 6)]
        public bool ArmyCommanderPerksWhenAlone { get; set; } = true;

        [SettingPropertyBool("Streamlined Operations (50): ballista reload from first mate", Order = 0, RequireRestart = false,
            HintText = "The +10% ballista reload speed in naval battles comes from the first mate, as described. Vanilla reads it from the formation captain, where it never applies. Also says reload speed instead of reload time." + LoadNote)]
        [SettingPropertyGroup(Boatswain, GroupOrder = 7)]
        public bool StreamlinedOperationsFirstMate { get; set; } = true;

        [SettingPropertyBool("Gilded Purse (100): ship capture from first mate", Order = 1, RequireRestart = false,
            HintText = "The ship capture chance comes from the first mate instead of the party leader." + LoadNote)]
        [SettingPropertyGroup(Boatswain)]
        public bool GildedPurseFirstMate { get; set; } = true;

        [SettingPropertyBool("Arr! / Pirate Hunter (175): captain battle XP", Order = 0, RequireRestart = false,
            HintText = "Captain XP after naval battles works from the formation captain (like Inspiring Leader). Vanilla never applies it: it uses party HasPerk for the Captain role, which always fails. Naval battles only; descriptions and captain scoring match." + LoadNote)]
        [SettingPropertyGroup(Mariner, GroupOrder = 1)]
        public bool ArrPirateHunterCaptainXp { get; set; } = true;

        [SettingPropertyBool("Special Arrows (125): sail damage from first mate", Order = 2, RequireRestart = false,
            HintText = "The +40% damage to enemy sails from troops' fire arrows comes from the first mate, as described. Vanilla reads it from the formation captain and adds the captain value instead (+5 flat damage). Captain +5 armor for tier 1-3 troops stays with the captain and only applies in naval battles." + LoadNote)]
        [SettingPropertyGroup(Boatswain)]
        public bool SpecialArrowsFirstMate { get; set; } = true;

        [SettingPropertyBool("Smooth Operator (150): ballista ammo from first mate", Order = 3, RequireRestart = false,
            HintText = "The +5 ballista ammo comes from the ship owner's first mate, as described. Vanilla reads it from the ship's captain, where it never applies." + LoadNote)]
        [SettingPropertyGroup(Boatswain)]
        public bool SmoothOperatorFirstMate { get; set; } = true;

        [SettingPropertyBool("Accuracy Training (150): ranged shield damage from first mate", Order = 4, RequireRestart = false,
            HintText = "The +30% ranged damage to shields in naval battles comes from the first mate, as described. Vanilla reads it from the formation captain, where it never applies." + LoadNote)]
        [SettingPropertyGroup(Boatswain)]
        public bool AccuracyTrainingFirstMate { get; set; } = true;

        [SettingPropertyBool("Popular Captain (175): deck size from first mate", Order = 5, RequireRestart = false,
            HintText = "The deck size bonus comes from the first mate instead of the party leader. The mariner recruitment discount stays with the leader." + LoadNote)]
        [SettingPropertyGroup(Boatswain)]
        public bool PopularCaptainFirstMate { get; set; } = true;

        [SettingPropertyBool("Port Authority (200): ship limit from first mate", Order = 6, RequireRestart = false,
            HintText = "The +1 ship to the command limit comes from the first mate instead of the party leader." + LoadNote)]
        [SettingPropertyGroup(Boatswain)]
        public bool PortAuthorityFirstMate { get; set; } = true;

        [SettingPropertyBool("Blessings of the Sea (200): ship limit from first mate", Order = 7, RequireRestart = false,
            HintText = "The +1 ship to the command limit comes from the first mate instead of the party leader." + LoadNote)]
        [SettingPropertyGroup(Boatswain)]
        public bool BlessingsOfTheSeaFirstMate { get; set; } = true;

        [SettingPropertyBool("Shipwright's Hand (225): ship repair from first mate", Order = 8, RequireRestart = false,
            HintText = "Repairing your ships from discarded ones comes from the first mate instead of the party leader." + LoadNote)]
        [SettingPropertyGroup(Boatswain)]
        public bool ShipwrightsHandFirstMate { get; set; } = true;

        [SettingPropertyBool("Merchant Fleet (250): battle ship limit from first mate", Order = 9, RequireRestart = false,
            HintText = "The +1 ship to the battle limit comes from the first mate instead of the party leader." + LoadNote)]
        [SettingPropertyGroup(Boatswain)]
        public bool MerchantFleetFirstMate { get; set; } = true;

        [SettingPropertyBool("Unflinching (175): disembark speed from navigator", Order = 0, RequireRestart = false,
            HintText = "The faster disembarking comes from the navigator instead of the party leader." + LoadNote)]
        [SettingPropertyGroup(Shipmaster, GroupOrder = 8)]
        public bool UnflinchingNavigator { get; set; } = true;

        [SettingPropertyBool("Shore Master (175): recall time from navigator", Order = 1, RequireRestart = false,
            HintText = "The halved ship recall time comes from the navigator instead of the party leader, so both effects are the navigator's." + LoadNote)]
        [SettingPropertyGroup(Shipmaster)]
        public bool ShoreMasterNavigator { get; set; } = true;

        [SettingPropertyBool("Fleet Commander (200): both effects from navigator", Order = 2, RequireRestart = false,
            HintText = "Both effects (fleet-size speed penalty and skeleton crew) come from the navigator instead of the party leader." + LoadNote)]
        [SettingPropertyGroup(Shipmaster)]
        public bool FleetCommanderNavigator { get; set; } = true;

        [SettingPropertyBool("Stormrider (225): ship limit from navigator", Order = 3, RequireRestart = false,
            HintText = "The +1 ship limit comes from the navigator instead of the party leader. The troop XP stays with the leader." + LoadNote)]
        [SettingPropertyGroup(Shipmaster)]
        public bool StormriderNavigator { get; set; } = true;

        [SettingPropertyBool("Master and Commander (225): ship limit from navigator", Order = 4, RequireRestart = false,
            HintText = "The +1 ship limit comes from the navigator instead of the party leader. The troop XP stays with the leader." + LoadNote)]
        [SettingPropertyGroup(Shipmaster)]
        public bool MasterAndCommanderNavigator { get; set; } = true;

        [SettingPropertyBool("Clarify perk descriptions", Order = 0, RequireRestart = false,
            HintText = "Clearer perk tooltips only: captain conditions, quiver types, auto-resolve wording, and similar. No gameplay change. Turn off if another mod edits those texts." + LoadNote)]
        [SettingPropertyGroup(Descriptions, GroupOrder = 0)]
        public bool ClarifyPerkDescriptions { get; set; } = true;

        [SettingPropertyBool("Role perks from party leader or role hero", Order = 0, RequireRestart = false,
            HintText = "A scout, quartermaster, surgeon, engineer, first mate or navigator effect applies if either that role's hero or the party leader has the perk (once). Skill-scaling uses the higher skill. Ship-limit pairs still give +1. Also applies to AI." + LoadNote)]
        [SettingPropertyGroup(Experimental, GroupOrder = 0)]
        public bool LeaderOrRoleHero { get; set; }

        [SettingPropertyBool("Tactics auto-resolve effects in battles", Order = 1, RequireRestart = false,
            HintText = "Auto-resolve Tactics bonuses also apply in battles you fight, at about 20%–50% strength. Land battles for most effects; Tactical Mastery also at sea at half rate. Captain bonuses are unchanged." + LoadNote)]
        [SettingPropertyGroup(Experimental)]
        public bool TacticsBattleEffects { get; set; }
    }

    /// <summary>MCM's instance is null until MCM has loaded; fall back to defaults until then.</summary>
    internal static class Settings
    {
        private static readonly PerkFixesAndRebalanceSettings Defaults = new PerkFixesAndRebalanceSettings();

        public static PerkFixesAndRebalanceSettings Current => PerkFixesAndRebalanceSettings.Instance ?? Defaults;
    }
}
