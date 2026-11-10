# Playtest — Assignment 10, Level2

Rules: a new player, on the phone (iPhone). I only say "play". No hints. Count tries until the level is beaten. Target: 2–4 tries.

## Round 1 (before changes)

| Tester | Tries | What happened | Which part |
|---|---|---|---|
| Nurzhan | 2 | First try: fell into a pit | pit |
| Kuka | 2 | First try: killed by enemies | enemies |

Result: both testers beat the level in 2 tries. This is inside the target (2–4), so I did not make the enemies easier or harder.

Both testers noticed one problem: when the player jumps, the head goes out of the top of the phone screen.

## Changes after the playtest

| What | Field | Old value | New value | Why |
|---|---|---|---|---|
| Main Camera | Orthographic Size | 4.5 | 5 | show more space above the player |
| Main Camera (CameraFollow) | minY (camera height) | 0 | 0.5 | move the view up, the head stays on screen |

Also added to CameraFollow: the camera moves up only when the player gets closer than topMargin = 2 m to the top edge (for example, jumping from a high platform).

## Round 2 (after changes)

| Tester | Tries after |
|---|---|
| | |

Numbers you can tune in the Inspector (current values):

- Patroller: viewRange 5, chaseSpeed 2.8, patrolDistance 3, windUp 0.6, cooldown 1.5
- Shooter: viewRange 7, aimTime 0.6, cooldown 1.5
- Charger: triggerRange 5, windUp 0.7, dashSpeed 7, maxDistanceFromStart 4, stunTime 1, cooldown 1
- EnemyHealth: maxHp 2 on each enemy
- Camera: size 5, minY 0.5, topMargin 2
