## Version 1.0.1
Actually link the dll this time (oops).

### Snare Flea
Hopefully fix the previous version in multiplayer.

## Version 1.0.0

### Bunker Spider
The spider can somehow easily be avoided and is often not worth dealing with given its taking 3 power levels (same as the Bracken for instance) and also takes a whooping 5 hits to kill, the following changes aim at addressing that:
- PowerLevel 3 => 2
  - More likely to appear than the Bracken but less than Hoarding Bugs
- MaxCount 1 => 2
  - There can be at most 2 spiders inside at once, which should make dodging it a bit riskier with the added webs and them being able to cover more ground
- Health 5 => 4
  - It takes one less hit to kill
- Damage 90 => 60
  - It still kills a player in 2 hits, but a one-hit combination of Blob or Yippee + Spider doesn't kill you
- HitCooldown 1 => ~1.33 sec
  - Aiming for 45 DPS (half the base value) which gives ~1.33 sec of cooldown for 60 damage, which should give a slightly more forgiving window to fight it

### Coilhead
Coilheads can cut a run short if you're in a smaller group as it often appears with other monsters given its low power level, so let's make it happen a bit less often:
- PowerLevel 1 => 2
- MaxCount 5 => 4

### Ghost Girl
Similarly to the coilhead, seeing one when alone or in a small group can mean ending the run earlier or risking a wipe, so let's make it less likely:
- PowerLevel 2 => 3

### Jester
They don't appear very often and are somehow manageable given the telegraphed behaviour, so let's consider it a swap of power level with the ghost girl:
- PowerLevel 3 => 2

### Snare Flea
If you know to look for them and have (pro-)flashlights, they are manageable, but they are also a great threat for a measly 1 power level as they can wipe a group with little chances to retaliate, so let's address that:
- Changing Snare Flea's second chance from single player to last survivor (multiplayer included)
- Snare Fleas drop from any player after inflicting 60+ damage
- Prevent Snare Fleas stacking on the same player by making them non-targetable while latched on
