# Personal ↔ Party Leader role-label audit (v1.0.1)

Bannerlord 1.4.8. Criteria: an effect is **Party Leader** if the live code only (or effectively only) reads the **party leader** (`LeaderHero`, `HasPerk` / `AddPerkBonusForParty` without PartyMember aggregation). It is **Personal** if any hero who has the perk can trigger it for themselves (or as the acting character), including companions who are not leading.

Label-only fixes go under **Clarify perk descriptions**. No gameplay change.

## Fixed in `ApplyVanillaRoleLabelFixes`

| Perk | Change |
| --- | --- |
| Rogue Extraordinaire | Personal → Party Leader (loot epic) — 1.0.0 |
| Deep Pockets (bandit wages) | Personal → Party Leader (secondary) — 1.0.0 |
| Immortal Charm | Personal → Clan Leader — 1.0.0 |
| We Pledge Our Swords | → Clan Leader (primary) — 1.0.0 |
| Improviser (disorganized morale) | Party Member → Party Leader — 1.0.0 |
| **Warlord** | Personal → Party Leader (battle influence) — 1.0.1 |
| **Famous Commander** | Personal → Party Leader (battle renown + recruitment XP) — 1.0.1 |
| **Besieged** | Personal → Party Leader (siege influence, secondary) — 1.0.1 |
| **Granary Accountant** | Personal → Party Leader (food sell penalty) — 1.0.1 |
| **Tradeyard Foreman** | Personal → Party Leader (pottery/tools/silk/jewelry sell penalty) — 1.0.1 |

## Keep as Personal

| Perk | Why Personal is correct |
| --- | --- |
| **Baptised in Blood** (primary, infantry XP per your 2H kill) | Killer hero’s perk; companions can proc it |
| **Make a Difference** (primary, morale on your kill) | Your kill; secondary shared XP is already Party Leader |
| **Oratory** (renown/influence per issue) | Acting hero resolving the issue |
| **Self Promoter** (renown on tournament win) | Winner’s perk |
| **Manhunter** (better ransom deals) | `sellerHero.GetPerkValue` |
| **Preventive Medicine** / HP personals | Character stat / own recovery |
| **Parade** (loyalty while waiting) | Any matching hero in main party / settlement; not leader-only |

## Party Leader → Personal

No high-confidence reverse cases in CampaignSystem after filtering false positives. Re-check MountAndBlade / Naval if needed later.
