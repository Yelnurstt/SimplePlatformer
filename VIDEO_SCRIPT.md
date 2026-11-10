# Video script (2–3 min, English A2–B1)

Not for the repository: read it, then say it in your own words.
Replace everything in [brackets] with real results.

---

**1. Intro (15 s)** — show the main menu, then Level2.

Hi, my name is [name]. This is my game Simple Platformer, Assignment 10. This week I added three enemy types with different behaviour. All three are in Level 2.

**2. Patroller (30 s)** — walk into its view, then hide behind the white wall.

The first enemy is the Patroller. It has four states: Patrol, Chase, Attack and Cooldown. It walks on its route. When it sees me, it chases me. To see me, it checks three things: distance, angle, and a raycast. The white wall is on the Walls layer, so it blocks the ray. If I hide behind the wall, the Patroller loses me and goes back to patrol. Before it hits, it turns red for 0.6 seconds. This is the warning.

**3. Shooter (25 s)** — stand in front of it, then step away.

The second enemy is the Shooter. It does not move. Its states are Idle, Aim, Shoot and Cooldown. When I am close and it is on the screen, it turns red and aims for 0.6 seconds. Then it shoots a slow bullet. After that it waits 1.5 seconds. So I have time to jump or move.

**4. Charger (25 s)** — let it blink, jump over the dash, shoot it while it is gray.

The third enemy is the Charger. Its states are Idle, Wind-up, Dash and Stunned. When I am on the same floor and close, it blinks red and plays a sound for 0.7 seconds. Then it dashes in a straight line. The direction is chosen at the start, so I can jump over it. After the dash it is gray and stunned for one second. This is my chance to shoot it.

**5. Script map (25 s)** — show SCRIPT_MAP.md on GitHub.

This is my script map. Every enemy has its own script with its own state machine. All three use the Player transform. The Patroller and the Charger use a raycast with the Walls layer. They damage the player through the Health script. The Shooter spawns a projectile, and the projectile damages the player. My bullet calls EnemyHealth on the enemies.

**6. Playtest (25 s)**

Two new players tested the game on my phone. [Name 1] needed [X] tries, [Name 2] needed [Y] tries. [Most deaths were at ...]. So I changed two numbers: [enemy, field: old → new] and [enemy, field: old → new]. After that the level took [Z] tries.

**7. AI and end (10 s)**

I used ChatGPT and Claude Code to help with the code. I wrote what I asked and what I fixed in DECISIONS.md. Thank you for watching.
