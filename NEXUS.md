# Perk Fixes and Rebalance

## Summary

Fixes broken and misleading perks, clarifies tooltips to match the real code, and rebalances a handful of overtuned or misplaced effects. Fully configurable in MCM; War Sails and RBM are optional.

## Description

Fixes broken and misleading perks, clarifies tooltips to match the real code, and rebalances a handful of overtuned or misplaced effects.

The vanilla perk system has the following inconsistencies / problems:

- Several perks never apply as written (wrong role, captain HasPerk dead ends, War Sails first-mate effects read from formation captains).
- Tooltips omit land-only auto-resolve limits, quiver types, and who must hold the perk.
- Armor penetration and movement-speed captain bonuses are broader or buggier than the skill trees suggest.
- Medicine and surgeon perks sit on the party leader while the text talks about the surgeon; Minister of Health scales harshly.
- Tactics party-leader bonuses are auto-resolve only, and fought battles grant almost no Tactics XP from troop kills compared to simulation.
- Doctor's Oath surgery Medicine XP is missing or weaker in fought battles (worse with RBM).

This mod aims to make perk text trustworthy and the important effects work where the game says they do. Defaults lean toward fairer numbers and correct roles; every gameplay change has an MCM toggle. Description clarifications can be turned off if another mod edits the same strings.

### Details

- **Broken perks that start working:** Arr! / Pirate Hunter captain naval battle XP; War Sails first-mate effects that vanilla never applied (Streamlined Operations, Smooth Operator, Accuracy Training, and related role moves); Special Arrows sail damage from the first mate as described.
- **Role and ownership fixes (on by default):** Surgeon for Sledges / Fortitude Tonic / Minister of Health; quartermaster for Artisan Community and Rapid Development; engineer skill for Masterwork; Tactical Mastery as party leader; selected army-commander perks when not in an army; Boatswain / Shipmaster effects moved off the party leader where the text implies the role hero.
- **Battle fixes (on by default):** Shattering Blow/Volley naval-only at 25% (vanilla 50% everywhere); Vandal two-handed only; movement-speed perks while holding a shield; Form Fitting Armor once; captain weapon-skill bonuses for mounted troops; Picked Shots limited to crossbow troops.
- **Captain scoring:** Land / Naval troop-usage bits so order-of-battle suggestions match where the perk actually matters (scoring only; battle application still follows game code / MCM).
- **Tooltip clarifications (on by default):** Captain conditions, auto-resolve wording, quiver types, role labels that did not match the code. No gameplay change when other options are off.
- **XP fixes (always on):** Fought-battle Tactics XP from troop kills matches auto-resolve's sim basis; Doctor's Oath grants surgery Medicine XP in fought battles like auto-resolve (covers the RBM gap).
- **Experimental (off by default):** Role perks from party leader or role hero; Tactics auto-resolve damage effects also in battles you fight at reduced strength.
- **Minister of Health:** Default 0.675 HP per Medicine point above 250 (vanilla 1); slider in MCM. Reads the surgeon when that option is on.

### Requirements

Bannerlord v1.4.8. Tested with Harmony v2.4.2, ButterLib v2.12.0, UIExtenderEx v2.13.3, MCM v5.12.3.

### Compatibility

Safe to add mid-save; MCM changes apply on Done or when a session starts. May conflict with other perk-description or perk-effect overhaul mods — turn off **Clarify perk descriptions** or individual toggles as needed. War Sails (NavalDLC) and RBM are optional; naval and RBM-specific patches activate when those modules are present.

### Recommended mods

None required. Pairs cleanly with RBM when both are loaded (Minister of Health and Doctor's Oath respect RBM's healing/auto-resolve paths where patched).
