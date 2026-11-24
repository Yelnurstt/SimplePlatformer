# Simple Platformer

This is my 2D mobile platform game made in Unity 6. I started it for my midterm and continued working on it for Assignment 8–9.

## About the game

The player runs, jumps, collects coins, and avoids spikes and enemies. You need to collect three coins to open the exit. There are two normal levels and a final boss level. The boss has 3 HP.

## Screens and controls

- **Main Menu:** Play and Quit.
- **Game HUD:** coins, HP, joystick, Jump and Pause. The boss level also uses Fire.
- **Pause Menu:** Resume, Restart and Main Menu.
- **Mobile controls:** on-screen joystick and buttons.

## Levels

- `MainMenu` — start screen.
- `Level1` — coins, platforms and spikes.
- `Level2` — coins, a moving platform and an enemy.
- `BossLevel` — fight the boss.
- `WinScreen` — shown after winning.

## How to open the project

1. Open the project folder in Unity Hub.
2. Use Unity Editor **6000.6.5f1** (Unity 6).
3. Open `Assets/Scenes/MainMenu.unity`.
4. Press Play to start the game.

The project uses Unity Input System and TextMesh Pro. Do not upload the Unity `Library` folder to GitHub.

## Assignment 8–9

For this assignment, I added a mobile main menu, Pause / Resume / Restart / Main Menu controls, and UI layout changes. I tested the Level1 HUD in Device Simulator on a 16:9 phone, a phone with a notch, and a 4:3 tablet. I also used GitHub commits and a feature branch with a merged pull request.

The Game Design Document (GDD), UI screenshots and demo video are submitted in Canvas.

## Next steps

I want to improve the enemy, add simple saving, and polish the sounds and effects. I am keeping the game small and easy to play.

## Sounds and music

All sounds and both music tracks are **own work**. They are generated from simple waves (square, triangle, noise) by `Tools/make_sounds.py` in this repository. No sounds from the internet. Run `python3 Tools/make_sounds.py` to make them again.

| File | What | Source, license |
|---|---|---|
| `Jump.wav` | player jump, rising blip | own work (Tools/make_sounds.py) |
| `Hurt.wav` | player takes damage | own work |
| `EnemyHit1-3.wav` | enemy / boss is hit, 3 versions | own work |
| `CoinPickup.wav` | coin pickup, two-note chime | own work |
| `Shoot.wav` | player shot | own work |
| `WindUp.wav` | enemy wind-up before an attack | own work |
| `Death.wav` | player death | own work |
| `Click.wav` | UI button click | own work |
| `Win.wav` | level complete | own work |
| `Lose.wav` | player died / time is up | own work |
| `MenuMusic.wav` | menu music, calm loop, 84 BPM | own work |
| `LevelMusic.wav` | level music, faster loop, 140 BPM | own work |

## Assignment 12 — Sound Pass

- `AudioManager` (Assets/Resources) plays every sound through the `GameMix` mixer: groups Master, Music (child Tracks), SFX, UI.
- Menu and level music switch with a 1 s crossfade; Retry does not restart the music.
- Music and SFX sliders in the main menu, in decibels (Log10 × 20), saved in `save.json`.
- Pause: the Paused snapshot makes the music quieter and muffled (Lowpass 800 Hz).
- Variation: random pitch 0.92–1.08 on effects, 3 versions of the hit sound.
