# DECISIONS.md

Decision log for working with AI (ChatGPT, Claude Code). 3–5 lines per week:
what I asked, what the AI got wrong, what I changed and why.

## Week 10 — Assignment 10: Three Enemy Types

- 2026-11-10  Asked ChatGPT for a Patroller FSM (Patrol → Chase → Attack → Cooldown). I made a new PatrollerAI script instead of changing the old EnemyPatrol, so Level1 keeps working. Added a Walls layer, a SightWall and a Raycast with wallsMask.
- 2026-11-10  Bug: the Patroller walked through the wall. A Kinematic Rigidbody2D moved with MovePosition ignores walls, so Claude Code added a short Raycast in the move direction. It still did not work: the real reason was that Walls Mask was saved as "Nothing" in Level2. Set it to Walls, and the script now uses the Walls layer if the mask is empty.
- 2026-11-10  ShooterAI reuses the boss bullet (BossProjectile). Bug: bullets flew through the wall, they were destroyed only on the "Ground" tag. Added a check for the Walls layer; tested: bullets now stop at the wall. Also fixed a PlatformCarry error on death (SetParent while the scene unloads).
- 2026-11-10  Asked Claude Code for a third enemy, ChargerAI (Idle → WindUp → Dash → Stunned): 0.7 s red blinking and a sound before the dash, direction locked at the start of the wind-up, stops at walls and edges, 1 s stun. Added EnemyHealth so my bullets can kill enemies, and turned on the Fire button in Level2.
