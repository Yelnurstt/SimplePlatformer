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

## Week 11 — Assignment 11: Data and Saves

- 2026-11-17  Asked Claude Code to move all balance numbers into ScriptableObjects. We made a base class EnemyStats (HP, damage, wind-up, cooldown) and PatrollerStats / ShooterStats / ChargerStats on top of it, plus PlayerStats, BossStats and ItemStats. Technical numbers (raycast length, blink speed) stay in the scripts, because a designer would not change them. If a stats asset is missing, the enemy writes an error and turns itself off instead of crashing.
- 2026-11-17  Three levels from LevelData assets: the same LevelLoader is in Level1, Level2 and BossLevel and creates the enemies from prefabs, sets the coins, time limit and next scene. The enemies were moved out of Level2 into prefabs. The lecture uses FindFirstObjectByType, but Unity 6.6 marks it obsolete (3 warnings in my Console), so we use FindAnyObjectByType (there is only one player).
- 2026-11-17  JSON save with JsonUtility in persistentDataPath. JsonUtility cannot save a Dictionary, so best times are a list of LevelResult. Load uses try/catch: a missing or broken file gives defaults. Save writes save.tmp first, then copies it to save.json, so a phone shutdown in the middle does not break the old save. Saves after a level, on volume change, on OnApplicationPause(true) and on quit. Tested in the editor: broken save.json → warning in Console, game works with defaults.
- 2026-11-17  Main menu: level buttons are locked/unlocked from the save, best time shown on the button. Volume uses − / + buttons (10% steps) and is stored in save.json, not PlayerPrefs, so all data is in one file. Reset progress opens a confirm panel (Yes / No) and keeps the volume, because volume is a setting, not progress.

## Week 12 — Assignment 12: Sound Pass

- 2026-11-24  I did not know where the old sounds came from, so there was no license for README. Asked Claude Code to make all sounds and both music tracks with a Python script (Tools/make_sounds.py): now every sound is own work. Music loops are built so the tail is added to the start: no click at the loop point.
- 2026-11-24  Our old code (from earlier weeks) played every sound with AudioSource.PlayClipAtPoint. The lecture says this skips the mixer, so the SFX slider would not work. Changed to one AudioManager with a fixed set of AudioSources routed to the SFX, UI and Tracks groups.
- 2026-11-24  Old music was a BackgroundMusic object in every level, so it started again on every scene load. Removed it; AudioManager lives through all scenes (DontDestroyOnLoad) and does not restart the same track on Retry.
- 2026-11-24  Last week one volume used AudioListener.volume (linear). Now Music and SFX are two values in save.json and go to the mixer in decibels: Log10(value) × 20, slider min 0.0001 = −80 dB.
