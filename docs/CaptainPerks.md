# Captain perks

121 perks. Bannerlord 1.4.8 + War Sails 1.2.8, with Perk Fixes and Rebalance (all settings at default). Effect text is what the game shows with Perk Fixes and Rebalance installed.

A captain effect applies to the troops in the formation that hero leads in a battle. The player captains the formation they're in. Heroes are assigned to formations in the order of battle screen.

- **Troop tag**: the troop types the game tags the perk for when suggesting captains. It doesn't limit the effect. `Any` means untagged. `Land` / `Naval` are Perk Fixes and Rebalance bits: scoring ORs the active context into the query, so land-only and sea-only captains only score in that context. Perks that work in both omit both bits.
- Effect texts use these terms (Perk Fixes and Rebalance rewrites the captain texts that didn't): **troops** means any unit; **infantry**, **archers**, **cavalry** and **horse archers** are troop classes from the loadout, which don't change in battle; **on foot** and **mounted** are the troop's state at that moment, so a dismounted cavalryman counts as on foot.
- **Note** lines add conditions the text leaves out.
- Captain movement-speed perks raise the troop's own speed, not its horse's, so they do nothing for mounted troops. With Perk Fixes and Rebalance they also work while holding a shield (vanilla skips several of them), and Form Fitting Armor applies once.
- Under RBM, all captain effects listed here still apply with the same conditions. RBM reimplements some of them (reload speed, throwing speed, damage bonus from speed, weapon handling in its stance system) but keeps the vanilla checks.
- [By troop type](#by-troop-type) at the end lists which perks apply to infantry, ranged troops, cavalry and horse archers.

- **Skill**: skill level required. Perks at the same level of a skill come in pairs; you pick one. **Alternative** is the other one.
- **Other effect of this perk**: the effect you also get from the same perk, with the role it needs.

## Vigor

### One Handed (9)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 25 | **Basher** | -4% melee damage taken by troops in your formation while it is in shield wall. | Any | Wrapped Handles | Personal: +50% damage and longer stun duration with shield bashes. |
| 25 | **Wrapped Handles** | +30 One Handed skill to troops in your formation. | 1H | Basher | Personal: +20% handling to one handed weapons. |
| 75 | **Shield Bearer** | +3% movement speed to troops on foot in your formation. | Foot | Cavalry | Personal: Removed movement speed penalty of wielding shields. |
| 75 | **Cavalry** | +5% melee damage by mounted troops in your formation. | Mounted + Melee | Shield Bearer | Personal: +5% damage with one handed weapons while mounted. |
| 125 | **Arrow Catcher** | Larger shield protection area against projectiles for troops in your formation. | Shield | Shieldwall | Personal: Larger shield protection area against projectiles. |
| 125 | **Shieldwall** | Larger shield protection area against projectiles to troops in your formation while in shield wall formation. | Shield | Arrow Catcher | Personal: -20% damage to your shield while blocking in wrong direction. |
| 200 | **Fleet of Foot** | +4% movement speed to troops on foot in your formation. | Foot | Steel Core Shields | Personal: +4% combat movement speed. |
| 200 | **Steel Core Shields** | -10% damage to shields of troops on foot in your formation. | Foot + Shield | Fleet of Foot | Personal: -10% damage to your shields. |
| 225 | **Deadly Purpose** | +10% melee damage by troops on foot in your formation. | Foot + Melee | Unwavering Defense | Personal: +5% damage with one handed weapons. |

### Two Handed (9)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 25 | **Wood Chopper** | +15% damage against shields with two handed weapons by troops in your formation. | 2H | Strong Grip | Personal: +30% damage to shields with two handed weapons. |
| 25 | **Strong Grip** | +30 Two Handed skill to troops in your formation. | 2H | Wood Chopper | Personal: +10% handling to two handed weapons. |
| 50 | **Head Basher** | +2% damage with any attack by troops on foot in your formation. | Foot | On the Edge | Personal: +10% damage with two handed axes and maces. |
| 50 | **On the Edge** | +2% melee swing speed to troops on foot in your formation. | Foot + Melee | Head Basher | Personal: +3% swing speed with two handed weapons. |
| 100 | **Shield breaker** | +10% damage against shields with two handed weapons by troops in your formation. | 2H | Beast Slayer | Personal: +40% damage to shields with two handed weapons. |
| 100 | **Beast Slayer** | +10% damage to mounts with two handed weapons by troops in your formation. | 2H + Land | Shield breaker | Personal: +50% damage to mounts with two handed weapons. |
| 200 | **Reckless Charge** | +2% damage with any attack and movement speed to troops on foot in your formation. | Foot | Thick Hides | Personal: +20% damage bonus from speed with two handed weapons while on foot. |
| 225 | **Vandal** | +20% damage against destructible objects by troops in your formation. | Any | Blade Master | Personal: +25% armor penetration with two handed weapons. |
| 225 | **Blade Master** | +2% melee attack speed to troops on foot in your formation. | Foot | Vandal | Personal: +10% damage with two handed weapons. |

### Polearm (14)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 25 | **Cavalry** | +2% damage with any attack, including horse charges, by mounted troops in your formation. | Mounted | Pikeman | Personal: +2% damage with polearms while mounted. |
| 25 | **Pikeman** | +2% damage with any attack by troops on foot in your formation. | Foot | Cavalry | Personal: +2% damage with polearms on foot. |
| 50 | **Braced** | +10% damage with any attack against mounted troops and their mounts by troops on foot in your formation. | Foot + Land | Keep at Bay | Personal: Polearms that can dismount ignore 25% dismount resistance on attacks against cavalry. |
| 75 | **Clean Thrust** | +30 Polearm skill to troops in your formation. | Polearm | Swift Swing | Personal: +10% thrust damage with polearms. |
| 75 | **Swift Swing** | +2% melee swing speed to troops on foot in your formation. | Foot + Melee | Clean Thrust | Personal: +5% swing speed with polearms. |
| 100 | **Footwork** | +2% movement speed to troops on foot in your formation. | Foot | Hard Knock | Personal: +2% combat movement speed with polearms. |
| 125 | **Lancer** | +30% damage bonus from speed with polearms by troops in your formation. | Polearm | Steed Killer | Personal: +20% damage bonus from speed with polearms while mounted. |
| 125 | **Steed Killer** | +30% damage to mounts with polearms by troops on foot in your formation. | Foot + Polearm + Land | Lancer | Personal: +70% damage to mounts with polearms. |
| 175 | **Phalanx** | +3% damage with polearms by troops in your formation.<br>**Note:** Every polearm melee hit, mounted or on foot. No formation requirement. | Polearm | Standard Bearer | Party leader: +30 melee weapon skills to troops in your party while in shield wall formation. |
| 175 | **Standard Bearer** | -20% battle morale loss to troops in your formation. | Any | Phalanx | Governor: -20% wages to garrisoned infantry in the governed settlement. |
| 225 | **Unstoppable Force** | +30% damage bonus from speed with polearms to mounted troops in your formation. | Mounted + Polearm | Sure Footed | Personal: Triple couch lance damage against shields. |
| 225 | **Sure Footed** | -30% charge damage taken by troops in your formation. | Land | Unstoppable Force | Personal: -40% charge damage taken. |
| 250 | **Sharpen the Tip** | +5% thrust damage with melee weapons by troops on foot in your formation. | Foot + Melee | Counterweight | Personal: +5% damage with thrust attacks made with polearms. |
| 250 | **Counterweight** | +20 polearm skill to troops in your formation. | Polearm | Sharpen the Tip | Personal: +15% handling of swingable polearms. |

## Control

### Bow (9)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 25 | **Dead Aim** | +20 Bow skill to troops in your formation. | Bow | Bow Control | Personal: +30% headshot damage with bows. |
| 25 | **Bow Control** | +5% damage with bows by troops in your formation. Doesn't apply to shields. | Bow | Dead Aim | Personal: -30% accuracy penalty while moving. |
| 50 | **Nocking Point** | +3% movement speed to troops on foot in your formation wielding a bow. | Foot + Bow | Bodkin | Personal: -50% movement speed penalty while reloading. |
| 50 | **Bodkin** | +5% armor penetration with bows by troops in your formation. | Bow | Nocking Point | Personal: +10% armor penetration with bows. |
| 75 | **Quick Adjustments** | -5% accuracy penalty with bows to troops in your formation. | Bow | Rapid Fire | Personal: -50% accuracy penalty while rotating. |
| 75 | **Rapid Fire** | +5% bow reload speed to troops in your formation. | Bow | Quick Adjustments | Personal: +25% reload speed with bows. |
| 125 | **Strong bows** | +5% damage with bows by tier 3+ troops in your formation. Doesn't apply to shields. | Bow | Trainer | Personal: +8% damage with bows. |
| 175 | **Skirmish Phase Master** | -10% damage taken from projectiles by archers and horse archers in your formation. | Ranged | Eagle Eye | Personal: -10% damage taken from projectiles. |
| 225 | **Horse Master** | +30 Bow skill to cavalry and horse archers in your formation, mounted or on foot. | Mounted + Bow | Deep Quivers | Personal: You can now use all bows on horseback. |

### Crossbow (10)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 50 | **Wind Winder** | +5% crossbow reload speed to troops in your formation. | Crossbow | Unhorser | Personal: +25% reload speed with crossbows. |
| 50 | **Unhorser** | +20% crossbow damage against mounts by troops in your formation. | Crossbow + Land | Wind Winder | Personal: +40% crossbow damage to mounts. |
| 75 | **Sheriff** | +10% crossbow damage against troops on foot by troops in your formation. | Crossbow | Donkey's Swiftness | Personal: +50% headshot damage with crossbows. |
| 75 | **Donkey's Swiftness** | +30 crossbow skill to troops in your formation. | Crossbow | Sheriff | Personal: -30% accuracy loss while moving. |
| 125 | **Puncture** | +5% armor penetration with crossbows by troops in your formation. | Crossbow | Fletcher | Personal: +10% armor penetration with crossbows. |
| 150 | **Deft Hands** | +50% resistance to getting staggered while reloading crossbows to troops in your formation. | Crossbow | Loose and Move | Personal: +50% resistance to getting staggered while reloading your crossbow. |
| 150 | **Loose and Move** | +5% movement speed to troops on foot in your formation wielding a ranged weapon. | Foot + Ranged | Deft Hands | Personal: Equipped crossbows do not slow you down. |
| 175 | **Counter Fire** | -3% damage taken from projectiles by troops in your formation wielding a crossbow. | Crossbow | Mounted Crossbowman | Personal: -10% projectile damage taken while equipped with a crossbow. |
| 225 | **Hammer Bolts** | +10% damage with crossbows by troops in your formation. Doesn't apply to shields. | Crossbow | Pavise | Personal: Crossbows can now dismount and ignore 50% dismount resistance on attacks against cavalry. |
| 250 | **Terror** | +25% morale loss to enemy due to crossbow kills by troops in your formation. | Crossbow | Picked Shots | Party leader: +20% chance of increasing the siege bombardment casualties per hit by 1. |

### Throwing (12)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 25 | **Shield Breaker** | +8% damage to shields with throwing weapons by troops in your formation. | Throwing | Quick Draw | Personal: +40% damage to shields with throwing weapons. |
| 25 | **Quick Draw** | +10% draw speed with throwing weapons to troops in your formation. | Throwing | Shield Breaker | Personal: +20% draw speed with throwing weapons. |
| 50 | **Flexible Fighter** | +15 Bow, Crossbow and Throwing skill to infantry, and +15 One Handed, Two Handed and Polearm skill to archers and horse archers in your formation.<br>**Note:** By troop class: infantry get Bow/Crossbow/Throwing skill; archers and horse archers get One/Two Handed and Polearm. Cavalry (including mounted troops with javelins but no bow/crossbow) get nothing. Vanilla tags Foot; optional MCM can set Any so horse-archer formations also score it. | Foot | Hunter | Personal: +10% damage while using throwing weapons as melee. |
| 50 | **Hunter** | +8% damage to mounts with throwing weapons by troops in your formation. | Throwing + Land | Flexible Fighter | Personal: +40% damage to mounts with throwing weapons. |
| 75 | **Mounted Skirmisher** | +10% damage with throwing weapons by mounted troops in your formation. | Mounted + Throwing | Well Prepared | Personal: -20% accuracy penalty with throwing weapons while mounted. |
| 100 | **Knock Off** | +5% throwing damage against mounted troops and their mounts by troops in your formation. | Throwing + Land | Running Throw | Personal: Thrown weapons can now dismount and ignore 25% dismount resistance on attacks against cavalry. |
| 100 | **Running Throw** | +30 throwing skill to troops in your formation. | Throwing | Knock Off | Personal: +0.25% damage bonus from speed with throwing weapons. |
| 125 | **Skirmisher** | -3% damage taken by ranged attacks to troops in your formation. | Any | Saddlebags | Personal: -10% damage taken by ranged attacks while holding a throwing weapon. |
| 200 | **Splinters** | +50% damage to shields with throwing weapons by troops in your formation.<br>**Note:** Any throwing weapon (the personal effect is throwing axes only). | Throwing | Resourceful | Personal: Triple damage against shields with throwing axes. |
| 225 | **Perfect Technique** | +10% travel speed to throwing weapons of troops in your formation. | Throwing | Long Reach | Personal: +25% travel speed to your throwing weapons. |
| 250 | **Impale** | +10% damage with throwing weapons by troops in your formation. | Throwing | Weak Spot | Personal: Javelins you throw can penetrate shields. |
| 250 | **Weak Spot** | +10% armor penetration with throwing weapons by troops in your formation. | Throwing | Impale | Personal: +30% armor penetration with throwing weapons. |

## Endurance

### Riding (10)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 25 | **Nimble Steed** | +30 Riding skill to cavalry and horse archers in your formation.<br>**Note:** Troops with a horse in their equipment, mounted or on foot. | Mounted | Full Speed | Personal: +10% maneuvering. |
| 25 | **Full Speed** | +10% charge damage dealt by troops in your formation. | Mounted | Nimble Steed | Personal: +20% charge damage dealt. |
| 75 | **Nomadic Traditions** | +10% melee damage bonus from speed to mounted troops in your formation. | Mounted + Melee | Deeper Sacks | Party leader: +30% party speed bonus from footmen on horses. |
| 100 | **Sagittarius** | -15% accuracy penalty to mounted troops in your formation. | Mounted + Ranged | Sweeping Wind | Personal: -15% accuracy penalty while mounted. |
| 150 | **Horse Archer** | +5% ranged damage by mounted troops in your formation. | Mounted + Ranged | Mounted Warrior | Personal: +10% ranged damage while mounted. |
| 150 | **Mounted Warrior** | +5% mounted melee damage by troops in your formation. | Mounted + Melee | Horse Archer | Personal: +5% mounted melee damage. |
| 200 | **Annoying Buzz** | +5% battle morale penalty to enemies with mounted ranged kills by troops in your formation. | Mounted + Ranged | Thunderous Charge | Personal: +20% battle morale penalty to enemies with mounted ranged kills. |
| 200 | **Thunderous Charge** | +10% battle morale penalty to enemies from weapon kills by mounted troops in your formation. Horse charge kills don't count. | Mounted + Melee | Annoying Buzz | Personal: +20% battle morale penalty to enemies with mounted melee kills. |
| 250 | **Tough Steed** | +10 armor to mounts of troops in your formation. | Mounted | Dauntless Steed | Personal: +20% armor to your mount. |
| 250 | **Dauntless Steed** | +5 armor to all equipped armor pieces of mounted troops in your formation. | Mounted | Tough Steed | Personal: +50% resistance to getting staggered while mounted. |

### Athletics (9)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 25 | **Morning Exercise** | +5% combat movement speed to troops on foot in your formation. | Foot | Well Built | Personal: +3% combat movement speed. |
| 50 | **Form Fitting Armor** | +4% combat movement speed to tier 3+ troops on foot in your formation. | Foot | Fury | Personal: -15% armor weight. |
| 50 | **Fury** | +10% melee weapon handling to troops on foot in your formation. | Foot | Form Fitting Armor | Personal: +10% weapon handling while on foot. |
| 100 | **Powerful** | +2% melee damage by troops in your formation. | Melee | Sprint | Personal: +4% damage with melee weapons. |
| 100 | **Sprint** | +3% combat movement speed to infantry troops in your formation.<br>**Note:** Infantry troop class only: archers and dismounted cavalry don't get it. Troop tag Foot + Melee so only infantry formations score it. | Foot + Melee | Powerful | Personal: +5% combat movement speed when you have no shields and no ranged weapons equipped. |
| 125 | **Braced** | -30% charge damage taken by troops in your formation. | Land | Surging Blow | Personal: -40% charge damage taken. |
| 125 | **Surging Blow** | +30% damage bonus from speed to troops on foot in your formation. | Foot | Braced | Personal: +30% damage bonus from speed while on foot. |
| 225 | **Strong Arms** | +20 throwing skill to troops in your formation. | Throwing | Strong Legs | Personal: +5% damage with throwing weapons. |
| 250 | **Ignore Pain** | +5 armor to all equipped armor pieces of troops on foot in your formation. | Foot | Spartan | Personal: +10% armor while on foot. |

## Cunning

### Tactics (9)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 25 | **Loose Formations** | -25% morale penalty when troops in your formation use line, loose, circle or scatter formations. | Any | Tight Formations | Party leader: -10% damage to your infantry from ranged troops when troops are sent to confront the enemy. |
| 25 | **Tight Formations** | -25% morale penalty when troops in your formation use shield wall, square, skein, column formations. | Any | Loose Formations | Party leader: +10% damage by your infantry to cavalry when troops are sent to confront the enemy. |
| 50 | **Decisive Battle** | +5% movement speed to troops on foot in your formation in plains, steppes and deserts. | Foot + Land | Extended Skirmish | Party leader: +5% damage in plains, steppes and deserts when your troops are sent to confront the enemy. |
| 50 | **Extended Skirmish** | +2% movement speed to troops on foot in your formation in snowy and forest terrains. | Foot + Land | Decisive Battle | Party leader: +10% damage in snowy and forest terrains when troops are sent to confront the enemy. |
| 75 | **Small Unit Tactics** | +5% movement speed to troops on foot in your formation when it has 15 or fewer soldiers. | Foot | Horde Leader | Party leader: +1 troop for the hideout crew |
| 100 | **Coaching** | +1% damage by troops in your formation. | Any | Law Keeper | Party leader: +3% damage when your troops are sent to confront the enemy. |
| 100 | **Law Keeper** | +4% damage against bandits by troops in your formation. | Any | Coaching | Party leader: +10% damage against bandits when your troops are sent to confront the enemy. |
| 200 | **Elite Reserves** | -5% damage taken by troops in your formation. | Any | Encirclement | Party leader: -20% less damage to tier 3+ units when troops are sent to confront the enemy. |
| 250 | **Gens d'armes** | +2% damage with any attack, including horse charges, against troops on foot by mounted troops in your formation. | Mounted | Counter Offensive | Governor: +1 daily security in the governed settlement. |

### Roguery (3)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 150 | **Partners in Crime** | +2% damage by bandit troops in your formation. | Any | Smuggler Connections | Party leader: Surrendering bandit parties can be recruited. |
| 200 | **Carver** | +2% one handed damage by troops in your formation. | 1H | Ransom Broker | Personal: +10% damage with civilian weapons. |
| 250 | **Dash and Slash** | +2% two handed weapon damage by troops in your formation. | 2H | Fleet Footed | Personal: +50% damage bonus from speed while on foot. |

## Social

### Leadership (2)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 75 | **Heroic Leader** | +10% battle morale penalty to enemies when troops in your formation kill an enemy. | Any | Authority | Governor: +1 daily loyalty in the governed settlement. |
| 175 | **Inspiring Leader** | +5% experience to troops in your formation. | Any | Uplifting Spirit | Army leader: -20% influence cost for calling parties to an army. |

## Intelligence

### Engineering (4)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 100 | **Dreadful Besieger** | +5% damage with crossbows by troops in your formation. Doesn't apply to shields. | Crossbow | Wall Breaker | Governor: +10% accuracy to your siege engines during siege bombardments in the governed settlement. |
| 100 | **Wall Breaker** | +10% damage dealt to shields by troops in your formation. | Any | Dreadful Besieger | Engineer: +25% damage dealt to walls during siege bombardment. |
| 225 | **Improved Tools** | +5% melee damage by troops in your formation. | Melee | Metallurgy | Engineer: +20% siege camp preparation speed. |
| 225 | **Metallurgy** | +5 armor to all equipped armor pieces of troops in your formation. | Any | Improved Tools | Engineer: +30% chance to remove negative modifiers on looted items. |

## War Sails

### Mariner (14)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 50 | **Shield Breaker** | +30% damage to shields dealt by crew.<br>**Note:** Naval battles only. Any attack that hits a shield. | Naval | Brute Force | Party leader: +25% naval raid speed. |
| 100 | **Enemy of the Wood** | +25% fire damage dealt by your ship and crew to enemy sails. | Naval | Naval Fighting Training | Army leader: -10 morale to enemy for each ship destroyed in battle. |
| 125 | **Rallying Cry** | +20% morale boost for crew while on own ship.<br>-10% melee damage taken by crew while on own ships. | Naval | Terror of the Seas |  |
| 125 | **Terror of the Seas** | -10% melee damage taken by crew while on enemy ships. | Naval | Rallying Cry | Party leader: +20% to morale loss suffered by enemy ships in battle. |
| 150 | **Shattering Volley** | +25% armor penetration for ranged weapons wielded by crew in naval battles.<br>**Note:** Perk Fixes and Rebalance: 25% (vanilla 50%), naval battles only. | Ranged + Naval | Shattering Blow | Personal: +25% armor penetration for ranged weapons in naval battles. |
| 150 | **Shattering Blow** | +25% armor penetration for melee weapons wielded by crew in naval battles.<br>**Note:** Perk Fixes and Rebalance: 25% (vanilla 50%), naval battles only. | Melee + Naval | Shattering Volley | Personal: +25% armor penetration for melee weapons in naval battles. |
| 175 | **Pirate Hunter** | +10% xp gain after each naval battle for regular troops under character's command.<br>**Note:** Naval battles only. Perk Fixes and Rebalance: applies from the formation captain during battle XP (like Inspiring Leader); vanilla never applied it (party HasPerk has no Captain role). | Naval | Arr! | Party leader: +20% bonus when crew is sent to confront pirates. |
| 175 | **Arr!** | +15% xp gain after each naval battle for mariner troops under character's command.<br>**Note:** Naval battles only. Perk Fixes and Rebalance: applies from the formation captain during battle XP (like Inspiring Leader); vanilla never applied it (party HasPerk has no Captain role). | Naval | Pirate Hunter | Personal: Surrendering pirate parties can be recruited. |
| 200 | **Home Turf Advantage** | +20% melee damage dealt by crew when fighting on own ship. | Melee + Naval | Boarding Master | Personal: +20% melee damage dealt by character when fighting on own ship. |
| 200 | **Boarding Master** | +15% melee damage dealt by crew when fighting on other ships. | Melee + Naval | Home Turf Advantage | Personal: +15% melee damage dealt by character when fighting on other ships. |
| 225 | **Crew of Spears** | +15% ranged damage dealt by crew armed with throwing weapons.<br>**Note:** Naval battles only. | Throwing + Naval | Mighty Blows | Personal: Impale shields with thrown javelins, and throwing axes deals damage after if they break a shield. |
| 225 | **Mighty Blows** | +15% melee damage dealt by crew armed with two-handed weapons.<br>**Note:** Naval battles only. Two handed swords, axes and maces, and two handed polearms. | 2H + Naval | Crew of Spears | Personal: Better cleave with two handed weapons swings. (Two handed weapons lose -50% less damage when they cut through the first opponent.) |
| 250 | **Warrior's Might** | +20% to throwing damage dealt by crew.<br>**Note:** Naval battles only. | Throwing + Naval | The Sky's Fury | Personal: +20% melee damage dealt by character. |
| 250 | **The Sky's Fury** | +15% to bow and crossbow damage dealt by crew. Doesn't apply to shields.<br>**Note:** Naval battles only. | Bow + Crossbow + Naval | Warrior's Might | Personal: +15% ranged damage dealt by character. |

### Boatswain (1)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 125 | **Special Arrows** | +5 armor to tier 1-3 troops in your formation in naval battles.<br>**Note:** Naval battles only (NavalAgentStatCalculateModel). Perk Fixes and Rebalance reads the sail damage effect from the first mate, as described; vanilla read it from the formation captain, as +5 fire damage (the captain value) instead of +40%. | Naval | Shipwright's Insight | First mate: +40% damage dealt by crew to enemy sails. |

### Shipmaster (6)

| Skill | Perk | Captain effect | Troop tag | Alternative | Other effect of this perk |
|---:|---|---|---|---|---|
| 25 | **Old Salt's Touch** | +30% to swimming endurance of crew | Naval | Master Angler | Navigator: +2% to travel speed at sea. |
| 50 | **Wind Rider** | -50% to deck movement penalty for crew. | Naval | Ghost Ship | Personal: -50% to deck movement penalty. |
| 100 | **Shock and Awe** | +30% morale boost when ship rams enemy ship. | Naval | Windborne | Navigator: +10% to speed on campaign map when against the wind. |
| 100 | **Windborne** | +20% to sail forces in missions. | Naval | Shock and Awe | Navigator: -50% to duration of disorganized state while at sea. |
| 200 | **Chain to Oars** | +20% to oar force in mission | Naval | Fleet Commander | Party leader: Prisoners help meet skeleton crew requirement |
| 250 | **Seaborne Fortress** | -20% to ranged damage taken by crew if not boarded. | Naval | The Corsair's Edge | Party leader: -10% to damage sustained by ships when crew is sent to confront the enemies. |

## By troop type

The game uses three different checks to decide who gets a captain effect:

- **Mounted right now**: most damage, speed and armor effects. A dismounted rider counts as on foot.
- **Horse in equipment**: Horse Master and Nimble Steed (only troops with one). This doesn't change when a troop loses its horse.
- **Troop class** (infantry, archer, cavalry, horse archer, from troop data): Sprint, Flexible Fighter and Skirmish Phase Master.

Weapon effects apply to whoever uses that weapon, so the tables below list the weapons each troop type normally uses. Mounted crossbowmen and mounted javelin throwers also get the crossbow and throwing rows of the ranged table.

### Every troop

| Perk | Skill | Effect |
|---|---|---|
| Standard Bearer | Polearm 175 | -20% morale loss. |
| Sure Footed | Polearm 225 | -30% charge damage taken. |
| Braced | Athletics 125 | -30% charge damage taken. |
| Skirmisher | Throwing 125 | -3% damage taken from missiles. |
| Counter Fire | Crossbow 175 | -3% damage taken from missiles, only while wielding a crossbow. |
| Elite Reserves | Tactics 200 | -5% damage taken. |
| Basher | One Handed 25 | -4% melee damage taken while the formation is in shield wall. |
| Metallurgy | Engineering 225 | +5 armor. |
| Arrow Catcher | One Handed 125 | Larger shield area against missiles, when holding a shield. |
| Shieldwall | One Handed 125 | Larger shield area against missiles in shield wall, when holding a shield. |
| Coaching | Tactics 100 | +1% damage against troops. |
| Law Keeper | Tactics 100 | +4% damage against bandits. |
| Partners in Crime | Roguery 150 | +2% damage by bandit troops. |
| Wall Breaker | Engineering 100 | +10% damage with any attack that hits a shield. |
| Vandal | Two Handed 225 | +20% damage against destructible objects. |
| Loose Formations, Tight Formations | Tactics 25 | -25% morale loss in the matching formations. |
| Heroic Leader | Leadership 75 | +10% enemy morale loss from kills. |
| Inspiring Leader | Leadership 175 | +5% experience. |
| Wrapped Handles | One Handed 25 | +30 One Handed skill. |
| Strong Grip | Two Handed 25 | +30 Two Handed skill. |
| Clean Thrust | Polearm 75 | +30 Polearm skill. |
| Counterweight | Polearm 250 | +20 Polearm skill. |

### Infantry

On foot, melee weapons, infantry class. Infantry who throw javelins also get the throwing rows of the ranged table.

| Perk | Skill | Effect |
|---|---|---|
| Shield Bearer | One Handed 75 | +3% movement speed. |
| Fleet of Foot | One Handed 200 | +4% movement speed. |
| Steel Core Shields | One Handed 200 | -10% damage to their shields. |
| Deadly Purpose | One Handed 225 | +10% melee damage, any melee weapon. |
| Wood Chopper | Two Handed 25 | +15% damage to shields, two handed weapons only. |
| Head Basher | Two Handed 50 | +2% damage, every attack. |
| On the Edge | Two Handed 50 | +2% swing speed, any melee weapon. |
| Shield breaker | Two Handed 100 | +10% damage to shields, two handed weapons only. |
| Beast Slayer | Two Handed 100 | +10% damage to horses, two handed weapons only. |
| Reckless Charge | Two Handed 200 | +2% damage (every attack) and +2% movement speed. |
| Blade Master | Two Handed 225 | +2% swing and thrust speed, any melee weapon. |
| Pikeman | Polearm 25 | +2% damage, every attack. |
| Braced | Polearm 50 | +10% damage against riders and their horses, every attack. |
| Swift Swing | Polearm 75 | +2% swing speed, any melee weapon. |
| Footwork | Polearm 100 | +2% movement speed. |
| Lancer | Polearm 125 | +30% damage bonus from speed, polearms. |
| Steed Killer | Polearm 125 | +30% damage to horses, polearms. |
| Phalanx | Polearm 175 | +3% damage, polearms. |
| Sharpen the Tip | Polearm 250 | +5% thrust damage, any melee weapon. |
| Flexible Fighter | Throwing 50 | +15 Bow, Crossbow and Throwing skill. |
| Morning Exercise | Athletics 25 | +5% movement speed. |
| Form Fitting Armor | Athletics 50 | +4% movement speed, tier 3+. |
| Fury | Athletics 50 | +10% weapon handling, melee weapons. |
| Powerful | Athletics 100 | +2% melee damage. |
| Sprint | Athletics 100 | +3% movement speed. |
| Surging Blow | Athletics 125 | +30% damage bonus from speed. |
| Ignore Pain | Athletics 250 | +5 armor. |
| Decisive Battle | Tactics 50 | +5% movement speed in plains, steppes and deserts. |
| Extended Skirmish | Tactics 50 | +2% movement speed in snow and forests. |
| Small Unit Tactics | Tactics 75 | +5% movement speed with 15 or fewer soldiers. |
| Carver | Roguery 200 | +2% damage, one handed weapons. |
| Dash and Slash | Roguery 250 | +2% damage, two handed weapons. |
| Improved Tools | Engineering 225 | +5% melee damage. |

### Ranged

On foot, bows, crossbows or throwing weapons, archer class.

| Perk | Skill | Effect |
|---|---|---|
| Dead Aim | Bow 25 | +20 Bow skill. |
| Bow Control | Bow 25 | +5% bow damage, not on shields. |
| Nocking Point | Bow 50 | +3% movement speed while wielding a bow. |
| Bodkin | Bow 50 | +5% armor penetration, bows. |
| Quick Adjustments | Bow 75 | -5% accuracy penalty, bows. |
| Rapid Fire | Bow 75 | +5% reload speed, bows. |
| Strong bows | Bow 125 | +5% bow damage for tier 3+, not on shields. |
| Skirmish Phase Master | Bow 175 | -10% damage taken from missiles. |
| Wind Winder | Crossbow 50 | +5% reload speed, crossbows. |
| Unhorser | Crossbow 50 | +20% crossbow damage to horses. |
| Sheriff | Crossbow 75 | +10% crossbow damage against troops on foot. |
| Donkey's Swiftness | Crossbow 75 | +30 Crossbow skill. |
| Puncture | Crossbow 125 | +5% armor penetration, crossbows. |
| Deft Hands | Crossbow 150 | +50% stagger resistance while reloading a crossbow. |
| Loose and Move | Crossbow 150 | +5% movement speed while wielding a ranged weapon. |
| Counter Fire | Crossbow 175 | -3% damage taken from missiles while wielding a crossbow. |
| Hammer Bolts | Crossbow 225 | +10% crossbow damage, not on shields. |
| Terror | Crossbow 250 | +25% enemy morale loss from crossbow kills. |
| Dreadful Besieger | Engineering 100 | +5% crossbow damage, not on shields. |
| Shield Breaker | Throwing 25 | +8% throwing damage to shields. |
| Quick Draw | Throwing 25 | +10% draw speed, throwing weapons. |
| Flexible Fighter | Throwing 50 | +15 One Handed, Two Handed and Polearm skill. |
| Hunter | Throwing 50 | +8% throwing damage to horses. |
| Knock Off | Throwing 100 | +5% throwing damage against riders and their horses. |
| Running Throw | Throwing 100 | +30 Throwing skill. |
| Strong Arms | Athletics 225 | +20 Throwing skill. |
| Splinters | Throwing 200 | +50% throwing damage to shields. |
| Perfect Technique | Throwing 225 | +10% throwing weapon speed. |
| Impale | Throwing 250 | +10% throwing damage. |
| Weak Spot | Throwing 250 | +10% armor penetration, throwing weapons. |
| Head Basher | Two Handed 50 | +2% damage, every attack including missiles. |
| Reckless Charge | Two Handed 200 | +2% damage (every attack including missiles) and +2% movement speed. |
| Pikeman | Polearm 25 | +2% damage, every attack including missiles. |
| Braced | Polearm 50 | +10% damage against riders and their horses, every attack including missiles. |
| Morning Exercise, Shield Bearer, Fleet of Foot, Footwork, Form Fitting Armor | | Movement speed, as for infantry. |
| Decisive Battle, Extended Skirmish, Small Unit Tactics | Tactics | Movement speed, as for infantry. |
| Ignore Pain | Athletics 250 | +5 armor. |

When they switch to melee weapons they also get the infantry melee rows, except Sprint and the infantry half of Flexible Fighter.

### Cavalry

Mounted, melee weapons, cavalry class.

| Perk | Skill | Effect |
|---|---|---|
| Cavalry | One Handed 75 | +5% melee damage, any melee weapon. |
| Arrow Catcher | One Handed 125 | Larger shield area against missiles, when holding a shield. |
| Wood Chopper, Shield breaker, Beast Slayer | Two Handed 25, 100 | Same as infantry, two handed weapons only. |
| Cavalry | Polearm 25 | +2% damage, every attack including horse charges. |
| Lancer | Polearm 125 | +30% damage bonus from speed, polearms. |
| Phalanx | Polearm 175 | +3% damage, polearms. |
| Unstoppable Force | Polearm 225 | +30% damage bonus from speed, polearms. |
| Nimble Steed | Riding 25 | +30 Riding skill. |
| Full Speed | Riding 25 | +10% horse charge damage. |
| Nomadic Traditions | Riding 75 | +10% damage bonus from speed, melee. |
| Mounted Warrior | Riding 150 | +5% melee damage. |
| Thunderous Charge | Riding 200 | +10% enemy morale loss from melee kills. |
| Tough Steed | Riding 250 | +10 armor to their horses. |
| Dauntless Steed | Riding 250 | +5 armor. |
| Powerful | Athletics 100 | +2% melee damage. |
| Gens d'armes | Tactics 250 | +2% damage against troops on foot, every attack including horse charges. |
| Carver | Roguery 200 | +2% damage, one handed weapons. |
| Dash and Slash | Roguery 250 | +2% damage, two handed weapons. |
| Improved Tools | Engineering 225 | +5% melee damage. |

#### Dismounted cavalry

Perks they get only once on foot. They lose the mounted rows above except Nimble Steed, which stays but does nothing. They never get Sprint or Flexible Fighter (troop class).

| Perk | Skill | Effect |
|---|---|---|
| Steel Core Shields | One Handed 200 | -10% damage to their shields. |
| Deadly Purpose | One Handed 225 | +10% melee damage, any melee weapon. |
| Head Basher | Two Handed 50 | +2% damage, every attack. |
| On the Edge | Two Handed 50 | +2% swing speed, any melee weapon. |
| Reckless Charge | Two Handed 200 | +2% damage (every attack) and +2% movement speed. |
| Blade Master | Two Handed 225 | +2% swing and thrust speed, any melee weapon. |
| Pikeman | Polearm 25 | +2% damage, every attack. |
| Braced | Polearm 50 | +10% damage against riders and their horses, every attack. |
| Swift Swing | Polearm 75 | +2% swing speed, any melee weapon. |
| Steed Killer | Polearm 125 | +30% damage to horses, polearms. |
| Sharpen the Tip | Polearm 250 | +5% thrust damage, any melee weapon. |
| Fury | Athletics 50 | +10% weapon handling, melee weapons. |
| Surging Blow | Athletics 125 | +30% damage bonus from speed. |
| Ignore Pain | Athletics 250 | +5 armor. |
| Morning Exercise, Shield Bearer, Fleet of Foot, Footwork, Form Fitting Armor | | Movement speed, as for infantry. |
| Decisive Battle, Extended Skirmish, Small Unit Tactics | Tactics | Movement speed, as for infantry. |

### Horse archers

Mounted, bows, horse archer class.

| Perk | Skill | Effect |
|---|---|---|
| Dead Aim | Bow 25 | +20 Bow skill. |
| Bow Control | Bow 25 | +5% bow damage, not on shields. |
| Bodkin | Bow 50 | +5% armor penetration, bows. |
| Quick Adjustments | Bow 75 | -5% accuracy penalty, bows. |
| Rapid Fire | Bow 75 | +5% reload speed, bows. |
| Strong bows | Bow 125 | +5% bow damage for tier 3+, not on shields. |
| Skirmish Phase Master | Bow 175 | -10% damage taken from missiles. |
| Horse Master | Bow 225 | +30 Bow skill. |
| Flexible Fighter | Throwing 50 | +15 One Handed, Two Handed and Polearm skill. |
| Mounted Skirmisher | Throwing 75 | +10% throwing damage, for horse archers with javelins. |
| Cavalry | Polearm 25 | +2% damage, every attack including arrows. |
| Nimble Steed | Riding 25 | +30 Riding skill. |
| Full Speed | Riding 25 | +10% horse charge damage. |
| Sagittarius | Riding 100 | -15% accuracy penalty while mounted. |
| Horse Archer | Riding 150 | +5% damage, any ranged weapon. |
| Annoying Buzz | Riding 200 | +5% enemy morale loss from ranged kills. |
| Tough Steed | Riding 250 | +10 armor to their horses. |
| Dauntless Steed | Riding 250 | +5 armor. |
| Gens d'armes | Tactics 250 | +2% damage against troops on foot, every attack including arrows. |

When they switch to melee weapons they also get the cavalry melee rows.

#### Dismounted horse archers

Perks they get only once on foot. They lose Cavalry (Polearm), Sagittarius, Horse Archer, Annoying Buzz, Dauntless Steed and Gens d'armes. They keep Horse Master (horse in equipment), and Skirmish Phase Master and Flexible Fighter (troop class). They never get Sprint.

| Perk | Skill | Effect |
|---|---|---|
| Nocking Point | Bow 50 | +3% movement speed while wielding a bow. |
| Loose and Move | Crossbow 150 | +5% movement speed while wielding a ranged weapon. |
| Head Basher | Two Handed 50 | +2% damage, every attack including arrows. |
| Reckless Charge | Two Handed 200 | +2% damage (every attack including arrows) and +2% movement speed. |
| Pikeman | Polearm 25 | +2% damage, every attack including arrows. |
| Braced | Polearm 50 | +10% damage against riders and their horses, every attack including arrows. |
| Ignore Pain | Athletics 250 | +5 armor. |
| Morning Exercise, Shield Bearer, Fleet of Foot, Footwork, Form Fitting Armor | | Movement speed, as for infantry. |
| Decisive Battle, Extended Skirmish, Small Unit Tactics | Tactics | Movement speed, as for infantry. |

With melee weapons they also get the on-foot melee rows of dismounted cavalry.

### Naval battles

There are no horses at sea, so War Sails captain perks depend only on the weapon and the ship. See the War Sails section for each perk's conditions.

