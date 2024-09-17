### Version 2.1.0
- Fix a Serialization error in the ConfigManager
- Fix an error with the PowerLevel (changed from int to float)

#### Bunker Spider
- Remove the change in PowerLevel since it was added in V50

#### Coilhead
- PowerLevel 1 => 1.5

## Version 2.0.0
Now fully configurable and synced with the Host!

### Version 1.1.0
Fix potential error when accessing player's health.

#### Snare Flea
Add an audio cue when a Snare Flea is on a ceiling (triggering 3 to 4 times per minute).

### Version 1.0.2
Update dll versioning so it shows up in the thunderstore.

### Version 1.0.1
Actually link the dll this time (oops).

#### Snare Flea
Hopefully fix the previous version in multiplayer.

## Version 1.0.0

### Bunker Spider
- PowerLevel 3 => 2
- MaxCount 1 => 2
- Health 5 => 4
- Damage 90 => 60
- HitCooldown 1 => ~1.33 sec

### Coilhead
- PowerLevel 1 => 2
- MaxCount 5 => 4

### Ghost Girl
- PowerLevel 2 => 3

### Jester
- PowerLevel 3 => 2

### Snare Flea
- A second chance is given to the last survivor in multiplayer
- Snare Fleas stop clinging from any player after inflicting 60+ damage
- Prevent Snare Fleas stacking on the same player by making players non-targetable while latched on
