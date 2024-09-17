# LCFairCompany
Mod to tweak some monsters in Lethal Company to make them more balanced, now fully configurable and synced with the Host!
Also fixes Snare Fleas stacking on the same player and add some QoL (e.g. second chance works in MP, audio cue client-side) - see details below.

## Details
<details open>
  <summary>Click to expand/collapse</summary>

### Bunker Spider
The spider can somehow easily be avoided and is often not worth dealing with given its fast/high damage and that it takes a whooping 5 hits to kill, the following changes aim at encouraging players to fight it more:
- MaxCount 1 => 2
  - There can be at most 2 spiders inside at once, which should make dodging it a bit riskier with the added webs and them being able to cover more ground
- Health 5 => 4
  - It takes one less hit to kill
- Damage 90 => 60
  - It still kills a player in 2 hits, but a one-hit combination of Blob or Loot Bug + Spider doesn't kill you
- HitCooldown 1 => ~1.33 sec
  - Aiming for 45 DPS (half the base value) which gives ~1.33 sec of cooldown for 60 damage, which should give a slightly more forgiving window to fight it

### Coilhead
Coilheads can cut a run short if you're in a smaller group as it often appears with other monsters given its low power level, so let's make it happen a bit less often:
- PowerLevel 1 => 1.5
- MaxCount 5 => 4

### Ghost Girl
Similarly to the coilhead, seeing one when alone or in a small group can mean ending the run earlier or risking a wipe, so let's make it less likely:
- PowerLevel 2 => 3

### Jester
They don't appear very often and are somehow manageable given the telegraphed behaviour, so let's consider it a swap of power level with the ghost girl:
- PowerLevel 3 => 2

### Snare Flea
If you know to look for them and have (pro)flashlights, they are manageable, but they are also a great threat for a measly 1 power level as they can wipe a group with little chances to retaliate, so let's address that:
- A second chance is given to the last survivor in multiplayer
- Snare Fleas stop clinging from any player after inflicting enough damage (60 by default)
- Prevent Snare Fleas stacking on the same player by making players non-targetable while latched on
- Add a quiet audio shriek when a Snare Flea is on a ceiling (triggering 3 to 4 times per minute by default)

</details>