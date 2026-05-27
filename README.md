# AI Survival Shooter

A top-down survival shooter made in Unity for the Artificial Intelligence with Unity course.

## About the Game

The player must survive against different enemy types while collecting score, ammo, and health pickups.

Enemies use Unity NavMesh for movement and State Machine Behaviours for their AI logic. The game includes melee and ranged enemies, slow and fast variants, rescue NPCs, slow zones, ammo management, and a final score/high score system.

## Main Features

- Top-down survival shooter gameplay.
- Melee enemy with patrol, chase, and attack behavior.
- Ranged enemy with patrol, chase, attack, and retreat behavior.
- Fast and slow enemy variants.
- Fast enemies can use NavMesh Links to cross special jump zones.
- Slow zones reduce movement speed.
- Rescue NPCs that can be attacked by enemies.
- Score reward for rescuing NPCs.
- Score penalty if the player kills an NPC.
- Ammo system with reload mechanic.
- Ammo pickups that also heal the player.
- Player dash for mobility.
- Enemies become more aggressive when the player has low health.
- Game over screen with restart and quit options.
- High score saved locally.

## Controls

| Action | Input |
|---|---|
| Move | WASD / Arrow Keys |
| Aim | Mouse |
| Shoot | Left Click |
| Reload | R |
| Dash | Left Shift |
| Rescue NPC | E |

## Technical Notes

- Built with Unity 6.
- Uses Unity NavMesh and NavMesh Links.
- Enemy behavior is handled through Animator State Machine Behaviours.
- Health is managed through a reusable `HealthComponent`.
- UI feedback displays score, health, ammo, enemies alive, and high score.
- The game includes a reusable scoring system and game over flow.

## How to Play

Survive as long as possible, eliminate enemies, rescue NPCs, collect ammo, and avoid getting surrounded.

Fast enemies can take special jump paths through NavMesh Links, while slow enemies cannot. Ammo is limited, so the player must reload and collect pickups around the map to keep fighting.
