# Project: Arena FPS Deathmatch Game

Inspired by Quake and Unreal Tournament. Web-based (Unity WebGL export) with mobile-responsive UI and online multiplayer.

## Tech Stack

- **Engine**: Unity (latest LTS) → WebGL build target
- **Networking**: Unity Netcode for GameObjects (NGO)
- **Platform**: WebGL (primary), mobile browser (secondary)

## Game Design

### Players
- 6–12 players per match

### Weapons
- Pickup-based: weapons spawn at fixed map locations
- Players run over pickups to collect them (Quake-style)

### Movement Mechanics
- Rocket jumping (explosive self-launch)
- Strafe jumping (speed gain via strafe + jump combo)
- Double jump
- Wall running

### Game Modes
1. **Free For All (FFA)** — every player for themselves, most frags wins
2. **Team Deathmatch** — two teams, highest team frag count wins
3. **Capture the Flag (CTF)** — steal enemy flag, return to base
4. **Last Man Standing** — limited lives, survive the longest

## Build Order

1. Unity project setup — NGO package, WebGL build target configured
2. FPS player controller — all 4 movement mechanics
3. Weapon system — pickup spawners, hitscan + projectile weapons
4. Networking — host/client lobby, player sync, kill feed
5. Game modes — FFA first, then Team DM, CTF, Last Man Standing
6. UI — HUD, scoreboard, lobby screen
7. WebGL export + mobile layout pass

## Conventions

- Use NGO `NetworkBehaviour` for all networked components
- Server-authoritative hit detection
- Separate assemblies for gameplay, networking, and UI
- Keep WebGL build size lean — avoid large asset bundles
