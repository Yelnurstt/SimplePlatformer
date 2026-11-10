# DECISIONS.md

Decision log for working with AI (ChatGPT, Claude Code). 3–5 lines per week:
what I asked, what the AI got wrong, what I changed and why.

## Week 10 — Assignment 10: Three Enemy Types

- 2026-11-10  Asked ChatGPT for a Patroller FSM (Patrol → Chase → Attack → Cooldown). I made a new PatrollerAI script instead of changing the old EnemyPatrol, so Level1 keeps working. Added a Walls layer, a SightWall and a Raycast with wallsMask.
- 2026-11-10  Bug: the Patroller walked through the wall. A Kinematic Rigidbody2D moved with MovePosition ignores walls, so Claude Code added a short Raycast in the move direction. It still did not work: the real reason was that Walls Mask was saved as "Nothing" in Level2. Set it to Walls, and the script now uses the Walls layer if the mask is empty.
- 2026-11-10  ShooterAI reuses the boss bullet (BossProjectile). Bug: bullets flew through the wall, they were destroyed only on the "Ground" tag. Added a check for the Walls layer; tested: bullets now stop at the wall. Also fixed a PlatformCarry error on death (SetParent while the scene unloads).
- 2026-11-10  Asked Claude Code for a third enemy, ChargerAI (Idle → WindUp → Dash → Stunned): 0.7 s red blinking and a sound before the dash, direction locked at the start of the wind-up, stops at walls and edges, 1 s stun. Added EnemyHealth so my bullets can kill enemies, and turned on the Fire button in Level2.
- 2026-11-11  Level2 was one screen and the exit was visible at once. Asked Claude Code to make it a side-scroller: CameraFollow script, a longer level with pits and platforms, enemies placed along the way. We did it in small steps and I tested each step before the commit. Changed Patroller patrolDistance 1.2 → 3 and Charger triggerRange 3 → 5 for the longer ground.
- 2026-11-11  Bug I found: the jump sometimes did not work. Reason: the ground check used OnCollisionEnter/Exit, and on the seam between two ground pieces Exit came after Enter. Now the player checks the ground every physics step (IsTouching with a normal-angle filter) and has 0.1 s coyote time. Tested: works in Level2.
- 2026-11-11  Playtest on iPhone: Nurzhan 2 tries (fell into a pit), Kuka 2 tries (killed by enemies). That is inside the 2–4 target, so I kept the enemy numbers. Both said the player's head goes off the top of the screen when jumping. Changed camera size 4.5 → 5 and camera height 0 → 0.5; Claude Code added a small vertical follow (topMargin 2 m) to CameraFollow.
