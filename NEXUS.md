# Perk Fixes and Rebalance

## Summary

Fixes broken and misleading perks, clarifies tooltips to match the real code, and rebalances a handful of overtuned or misplaced effects. Fully configurable in MCM; War Sails and RBM are optional.

## Description

The vanilla perk system has a lot of inconsistencies / bugs:

- A number of perk effects don't match their descriptions. (Example: Form Fitting Armor is described as +4% combat movement speed to tier 3+ troops on foot, but vanilla can give +8%, +4%, or nothing depending on troop class and whether they hold a shield.)
- Some perk descriptions don't accurately describe when they apply — or whether they apply at all. (Example: Shattering Blow/Volley read like Mariner naval perks but give 50% armor penetration in every land battle too.)
- Some skills don't gain XP like they are supposed to. (Example: fought battles grant almost no Tactics XP from troop kills compared to auto-resolve; Doctor's Oath surgery Medicine XP is missing or weaker in combat, worse with RBM.)
- Some Party Leader effects are labeled Personal and vice versa.
- Captain suggestions ignore whether a captain perk is land-only or naval-only, and some formation matches are wrong. (Example: charge and mount perks score at sea; Mariner ship perks score on land; Flexible Fighter never scores for horse archers.)

The vanilla perk system also has some problems / imbalances:

- Some high-level weapon perks are so generically powerful that the player is incentivized to take them even if they don't use that weapon. (Example: Two-Handed's Vandal gives 25% armor penetration for all weapons.)
- Some of the most important party-role perks are party-leader-only, incentivizing the player to take them rather than delegate to companions (Examples: Capstone perks for Medicine and Engineering. War Sails ship-limit perks.)
- Tactics is effectively useless unless auto-resolving.

This mod aims to address all the issues listed above and make perks more consistent and balanced. Defaults lean toward fairer numbers and correct roles; every gameplay change has an MCM toggle.

### Details

**Broken perks that start working (on by default)**

- **Arr! / Pirate Hunter [Mariner, 175]:** Captain XP after naval battles now applies correctly from the captain.
- **Streamlined Operations [Boatswain, 50]:** +10% ballista reload in naval battles comes from the first mate (instead of the captain).
- **Smooth Operator [Boatswain, 150]:** Extra ballista ammo comes from the ship owner's first mate (instead of the captain).
- **Accuracy Training [Boatswain, 150]:** +30% ranged damage to shields in naval battles comes from the first mate (instead of the captain).
- **Special Arrows [Boatswain, 125]:** +40% fire-arrow damage to enemy sails (instead of a flat +5) and comes from the first mate (instead of the captain).

**Role and ownership fixes (on by default)**

- **Sledges [Medicine, 75]:** +15 mount HP comes from the surgeon, not the party leader.
- **Fortitude Tonic [Medicine, 225]:** Party hero HP bonus comes from the surgeon; every hero except the surgeon gets it.
- **Minister of Health [Medicine, 275]:** Troop HP bonus uses the surgeon's perk and Medicine skill. Default strength is 0.675 HP per Medicine point above 250 (vanilla 1; slider in MCM).
- **Artisan Community [Trade, 125]:** Extra merchant recruit slot comes from the quartermaster, not the recruiting hero.
- **Rapid Development [Trade, 175]:** Trade-good price discount only needs the quartermaster (not the clan leader as well).
- **Masterwork [Engineering, 275]:** Siege-engine damage scales with the engineer's Engineering, not the party leader's.
- **Tactical Mastery [Tactics, 275]:** Party-leader perk: any party gets it from its leader, including when not in an army.
- **On The March [Engineering, 150], Great Leader [Leadership, 225], Enemy of the Wood [Mariner, 100], Commodore [Shipmaster, 275]:** Also work for a party that is not in an army, using that party's leader.
- **Gilded Purse [Boatswain, 100], Popular Captain [Boatswain, 175], Port Authority / Blessings of the Sea [Boatswain, 200], Shipwright's Hand [Boatswain, 225], Merchant Fleet [Boatswain, 250]:** Moved from party leader to first mate where the effect belongs.
- **Unflinching / Shore Master [Shipmaster, 175], Fleet Commander [Shipmaster, 200], Stormrider / Master and Commander [Shipmaster, 225]:** Ship-handling / ship-limit effects moved from party leader to navigator (troop XP halves stay with the leader).

**Battle fixes (on by default)**

- **Shattering Blow / Shattering Volley [Mariner, 150]:** Armor penetration only in naval battles, default 25% (vanilla 50% in every battle).
- **Vandal [Two-Handed, 225]:** Armor penetration only with two-handed weapons (vanilla: all weapons).
- **Movement speed while holding a shield:** Captain and personal movement-speed perks that skipped a shield in hand now apply with a shield too (including Morning Exercise, Shield Bearer, Fleet of Foot, Reckless Charge, Footwork, Form Fitting Armor, Sprint, Self Medication).
- **Form Fitting Armor [Athletics, 50]:** Always +4% to tier 3+ troops on foot, as described.
- **Wrapped Handles [One-Handed, 25], Strong Grip [Two-Handed, 25], Clean Thrust [Polearm, 75], Counterweight [Polearm, 250]:** Captain weapon-skill bonuses go to mounted troops too.
- **Flexible Fighter [Throwing, 50]:** Cavalry get the same +15 Bow, Crossbow and Throwing skill as infantry. Archers and horse archers still get +15 One Handed, Two Handed and Polearm.
- **Picked Shots [Crossbow, 250]:** Wage cut and HP bonus only for crossbow troops (not all ranged).

**Captain scoring (order-of-battle suggestions)**

- Land-only and naval-only captain perks only score in the matching battle type (for example charge/mount perks on land; Mariner ship perks at sea). This only affects who the UI suggests as captain, not who gets the bonus in a fight.
- **Flexible Fighter [Throwing, 50]:** Horse-archer formations score this perk by default.

**Tooltip clarifications (on by default)**

- Clearer text for when captain bonuses apply, which quiver types get ammo, auto-resolve-only effects, and role labels that did not match the code (for example Warlord, Famous Commander, Rogue Extraordinaire, Immortal Charm, Improviser). No gameplay change by itself. Turn off if another mod edits the same strings.

**XP fixes (always on)**

- **Tactics:** In battles you fight, troop kills grant Tactics XP on the same basis as auto-resolve.
- **Doctor's Oath [Medicine, 75]:** Surgery Medicine XP when your side downs enemies in fought battles, matching auto-resolve (also when RBM is loaded).

**Experimental (off by default)**

- **Tactics auto-resolve effects in battles:** Party-leader Tactics (and a few related) auto-resolve bonuses also apply in land battles you fight. Situational ones — terrain, infantry vs cavalry, outnumbering, many attacker/defender cases — apply at half their auto-resolve strength. Strong always-on ones — Coaching, Elite Reserves, Counter Offensive, Trusted Commander troop XP — apply at about one-fifth. Tactical Mastery applies at one-tenth of its auto-resolve rate on land (half that at sea). Perk tooltips show the fought-battle numbers.
- **Role perks from party leader or role hero:** Scout, quartermaster, surgeon, engineer, first mate, and navigator effects apply if either that role's hero or the party leader has the perk (counted once). That means that a high-medicine party leader and a surgeon companion can take complementary perks to collectively get access to all medicine perks. Skill-scaling perks use the higher skill.

### Requirements

Bannerlord v1.4.8. Tested with Harmony v2.4.2, ButterLib v2.12.0, UIExtenderEx v2.13.3, MCM v5.12.3.

### Compatibility

Safe to add mid-save; MCM changes apply on Done or when a session starts. May conflict with other perk-description or perk-effect overhaul mods — turn off **Clarify perk descriptions** or individual toggles as needed. War Sails (NavalDLC) and RBM are optional; naval and RBM-specific patches activate when those modules are present.

### Other mods by me

- [Same Rules for All Heroes](https://www.nexusmods.com/mountandblade2bannerlord/mods/13937) — Consistent attribute and focus point totals for every hero — player, companions, lords, and children.
- [Perk QoL - Respec and Highlighting](https://www.nexusmods.com/mountandblade2bannerlord/mods/13954) — Respec perks directly on the character screen. Highlight perks based on role or matching search text.
