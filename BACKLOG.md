# Arena FPS — Product Backlog

Inspired by Quake / Unreal Tournament. Unity 6 WebGL, online multiplayer via NGO.
Items are ordered by priority within each epic. Complete epics roughly in order top-to-bottom.

---

## Epic 1 — Core Movement & Feel
> Foundation everything else builds on. Must feel great before moving forward.

| # | Item | Notes |
|---|------|-------|
| 1.1 | FPS character controller (done) | Walk, strafe, jump |
| 1.2 | Strafe jumping (done) | Quake-style air acceleration |
| 1.3 | Double jump (done) | 2 jumps max, resets on land |
| 1.4 | Wall running (done) | Detect walls left/right, timer, wall-jump off |
| 1.5 | Rocket jumping (done) | `ApplyExplosiveForce()` on explosion |
| 1.6 | Player controller polish | Tune feel — landing smoothing, coyote time, step-up |
| 1.7 | Player death & respawn | 3–5s countdown timer, respawn at spawn point |
| 1.8 | Spawn point system | Weighted random spawn, avoid spawning near enemies |

---

## Epic 2 — Weapons
> All 4 weapons with distinct feel. Pickup-based. Drops on death.

| # | Item | Notes |
|---|------|-------|
| 2.1 | Weapon base class | Fire rate, ammo, hitscan/projectile interface |
| 2.2 | Machine Gun | Rapid hitscan, low damage, high ammo |
| 2.3 | Shotgun | Close-range spread, 8 pellets, fast fire rate |
| 2.4 | Railgun | Instant hitscan, 80–100 dmg, slow fire rate, visual beam |
| 2.5 | Rocket Launcher | Projectile, splash damage, calls `ApplyExplosiveForce` |
| 2.6 | Weapon pickup spawner | Fixed locations on map, 30s respawn timer |
| 2.7 | Weapon drop on death | All weapons drop to ground on player death |
| 2.8 | Ammo pickup | Separate ammo boxes on map |
| 2.9 | Weapon switch | Scroll wheel + number keys 1–4 |
| 2.10 | Hit marker | Crosshair flash on hit |
| 2.11 | Damage numbers | Floating numbers above hit location |

---

## Epic 3 — Health & Armor
| # | Item | Notes |
|---|------|-------|
| 3.1 | Health system | 100 HP base, death at 0 |
| 3.2 | Armor system | Absorbs 66% of damage up to armor value |
| 3.3 | Health pickup | +25 HP, map spawn, 30s respawn |
| 3.4 | Armor pickup | +50 armor, map spawn, 30s respawn |
| 3.5 | Mega Health | +100 HP (capped at 200), rare map spawn |
| 3.6 | Regeneration | Slow HP regen after 5s out of combat (optional — decide later) |

---

## Epic 4 — Networking
| # | Item | Notes |
|---|------|-------|
| 4.1 | NGO host/client setup | Server authority model |
| 4.2 | Player network transform | Smooth interpolation for remote players |
| 4.3 | Server-authoritative hit detection | Lag compensation via NGO |
| 4.4 | Quick play matchmaking | Auto-join best available match |
| 4.5 | Custom lobby rooms | Create/join by room code |
| 4.6 | Lobby UI | Room list, player list, ready check, map vote |
| 4.7 | Kill feed sync | Networked kill events → all clients |
| 4.8 | Disconnect handling | Graceful host migration or session end |
| 4.9 | Latency display | Ping shown on scoreboard |

---

## Epic 5 — Game Modes
> Build FFA first, then re-use infrastructure for others.

| # | Item | Notes |
|---|------|-------|
| 5.1 | **FFA** — Free For All | First to frag limit OR most kills at time limit |
| 5.2 | **TDM** — Team Deathmatch | Two teams, highest team frags at time limit |
| 5.3 | **CTF** — Capture The Flag | Carry enemy flag to base; first to score limit |
| 5.4 | **LMS** — Last Man Standing | Limited lives; spectate on death; last alive wins |
| 5.5 | Mode-specific win conditions | Frag limit FFA/LMS; time limit TDM; score limit CTF |
| 5.6 | Game mode selector in lobby | Host picks mode before match |

---

## Epic 6 — Map
> 1 map at launch. Tight arena optimised for 6–12 players.

| # | Item | Notes |
|---|------|-------|
| 6.1 | Arena blockout | Grey-box layout with vertical play, open areas, tight corridors |
| 6.2 | Wall-run corridors | Dedicated stretches for wall-run movement |
| 6.3 | Weapon & item placement | Strategic positions — railgun up high, rocket down low |
| 6.4 | Spawn points | 8–12 spread evenly around map |
| 6.5 | CTF flag bases | Two bases on opposite ends for CTF mode |
| 6.6 | Art pass | Replace grey-box with military/industrial textures and props |
| 6.7 | Ambient map audio | Background loop + environmental audio triggers |
| 6.8 | Occlusion culling | Performance — cull geometry not in view |

---

## Epic 7 — HUD
| # | Item | Notes |
|---|------|-------|
| 7.1 | Health bar | Bottom-left, visual fill bar + numeric value |
| 7.2 | Armor bar | Beneath health bar |
| 7.3 | Ammo counter | Bottom-right, current/reserve |
| 7.4 | Current weapon icon | Next to ammo counter |
| 7.5 | Kill feed | Top-right, fades after 4s |
| 7.6 | Minimap | Top-left, overhead view, shows teammates + pickups |
| 7.7 | Crosshair | Dynamic — expands on move/shoot |
| 7.8 | Respawn countdown | Centred on screen after death |
| 7.9 | Match timer | Top-centre countdown |
| 7.10 | Score display | Top-centre below timer (frags or team scores) |

---

## Epic 8 — Scoreboard & Post-Match
| # | Item | Notes |
|---|------|-------|
| 8.1 | In-match scoreboard | Tab key — kills, deaths, K/D, ping |
| 8.2 | Post-match scoreboard | Full final rankings |
| 8.3 | MVP highlight | Top player spotlight (kill count, accuracy) |
| 8.4 | XP summary screen | XP earned breakdown + rank progress bar |
| 8.5 | Rematch vote | Players vote to replay same map/mode |

---

## Epic 9 — Progression & Ranks
| # | Item | Notes |
|---|------|-------|
| 9.1 | XP system | Earn XP per kill, assist, objective, match completion |
| 9.2 | Level system | XP thresholds per level (scale up over time) |
| 9.3 | Rank tiers | Bronze → Silver → Gold → Platinum → Diamond → Elite |
| 9.4 | Rank display | Shown in lobby, scoreboard, player nameplate |
| 9.5 | Persistent XP storage | Save XP/level per guest session (localStorage for WebGL) |

---

## Epic 10 — Cosmetics
| # | Item | Notes |
|---|------|-------|
| 10.1 | Character skin system | Swap player mesh/material |
| 10.2 | Default skins (3) | Soldier variants — olive, desert, urban |
| 10.3 | Weapon skin system | Per-weapon material swap |
| 10.4 | Default weapon skins (2 per weapon) | Base + unlockable variant |
| 10.5 | Kill effect system | Visual effect on frag (blood, sparks, stylised) |
| 10.6 | Default kill effects (3) | Standard, electric, fire |
| 10.7 | Cosmetic unlock via XP/level | Tie unlocks to progression milestones |
| 10.8 | Loadout screen | Select skin, weapon skin, kill effect before match |

---

## Epic 11 — Audio
| # | Item | Notes |
|---|------|-------|
| 11.1 | Weapon fire SFX | Distinct per weapon — machine gun, shotgun, railgun, rocket |
| 11.2 | Weapon impact SFX | Hit on flesh vs environment |
| 11.3 | Footstep audio | Positional 3D audio, surface-dependent |
| 11.4 | Announcer voice | UT-style: "Double Kill!", "Headshot!", "First Blood!", "Rampage!" |
| 11.5 | Ambient map audio | Environmental loop + event-based triggers |
| 11.6 | UI sound FX | Button clicks, menu transitions |
| 11.7 | Audio mixer | Master, SFX, music, announcer volume channels |
| 11.8 | Audio settings in menu | Sliders for each mixer channel |

---

## Epic 12 — Main Menu & Settings (in progress)
| # | Item | Notes |
|---|------|-------|
| 12.1 | Main menu scene (done) | New Game, Load Game, Settings, Quit |
| 12.2 | Settings — mouse sensitivity (done) | Slider, saved to PlayerPrefs |
| 12.3 | Settings — movement speed (done) | Slider, saved to PlayerPrefs |
| 12.4 | Settings — audio mixer controls | Per-channel volume sliders |
| 12.5 | Settings — graphics quality | Low / Medium / High preset |
| 12.6 | Settings — fullscreen / resolution | WebGL window size options |
| 12.7 | Guest username entry | Input field on first launch, saved locally |

---

## Epic 13 — WebGL & Mobile
| # | Item | Notes |
|---|------|-------|
| 13.1 | WebGL build configuration | Compression, memory, loading screen |
| 13.2 | 120 FPS target on desktop | Profile and hit frame budget |
| 13.3 | Mobile-responsive UI | Anchors and scaling for small screens |
| 13.4 | Mobile touch controls | Virtual joystick + fire/jump buttons |
| 13.5 | Mobile performance pass | Reduce draw calls, lower shadow quality on mobile |
| 13.6 | Loading screen | Progress bar while WebGL initialises |

---

## Icebox (future consideration)
| Item | Notes |
|------|-------|
| Additional maps (2–5) | After launch, based on player feedback |
| OAuth login (Google/Discord) | Upgrade from guest when persistence needed |
| Spectator mode | Free-cam or follow-cam for eliminated players |
| Replay system | Save and replay match highlights |
| Anti-cheat | Server validation, rate limiting |
| Leaderboards | Global and friends rankings |
| Tournaments | Bracket system for organised play |
| Custom game rules | Gravity modifier, instagib mode, etc. |
