# Script map — Assignment 10 (Level2)

```mermaid
flowchart LR
    PI[PlayerInput<br/>joystick, Jump, Fire] --> PC[PlayerController<br/>move, jump]
    PI --> PS[PlayerShooter]
    CF[CameraFollow<br/>on Main Camera] -- follows --> PC
    PS -- spawns --> PB[PlayerBullet]
    PB -- TakeDamage --> EH[EnemyHealth<br/>on every enemy]

    subgraph Patroller[PatrollerAI]
        P1[Patrol] -- sees player --> P2[Chase]
        P2 -- close + on screen --> P3[Attack<br/>red, 0.6 s]
        P2 -- lost 1.5 s --> P1
        P3 --> P4[Cooldown 1.5 s]
        P4 --> P1
    end

    subgraph Shooter[ShooterAI]
        S1[Idle] -- player in range + on screen --> S2[Aim<br/>red, 0.6 s]
        S2 --> S3[Shoot]
        S3 --> S4[Cooldown 1.5 s]
        S4 --> S1
    end

    subgraph Charger[ChargerAI]
        C1[Idle] -- sees player, same floor --> C2[WindUp<br/>blinks red, 0.7 s]
        C2 --> C3[Dash]
        C3 -- distance / wall / edge --> C4[Stunned<br/>gray, 1 s]
        C4 --> C1
    end

    PL[(Player Transform)] --> Patroller
    PL --> Shooter
    PL --> Charger
    Patroller -- Raycast, Walls layer --> W[SightWall]
    Charger -- Raycast, Walls layer --> W
    Shooter -- spawns --> BP[BossProjectile]
    BP -- destroyed by --> W
    Patroller -- TakeDamage --> H[Health<br/>player HP]
    Charger -- TakeDamage --> H
    BP -- TakeDamage --> H
```

| Script | Job | Talks to |
|---|---|---|
| PatrollerAI | Patrol → Chase → Attack → Cooldown. Sight: distance, angle, Raycast | Player Transform, Health, Walls layer |
| ShooterAI | Idle → Aim → Shoot → Cooldown. Keeps its place, shoots | Player Transform, BossProjectile |
| ChargerAI | Idle → WindUp → Dash → Stunned. Dashes left/right | Player Transform, Health, Walls layer |
| BossProjectile | Flies, hurts the player, stops at Ground and Walls | Health |
| EnemyHealth | HP of an enemy, white flash, destroy at 0 | (called by PlayerBullet) |
| PlayerBullet | Player's bullet | BossHealth, EnemyHealth |
| CameraFollow | Side-scrolling camera, stops at the level ends | Player Transform |

Level2 from left to right: spike → coin 1 → pit → SightWall + Patroller → pit with a step (coin 2) → platforms (coin 3) → Shooter → pit → heart → Charger → pit with a platform → exit.

# Script map — Assignment 11 (data and saves)

```mermaid
flowchart LR
    subgraph Data[ScriptableObject assets - Assets/Data]
        ST[Patroller / Shooter / Charger Stats<br/>Player / Boss / Item Stats]
        LD[Level_01 / Level_02 / Level_03<br/>LevelData]
    end
    ST --> AI[Enemy AI, Player, Boss]
    LD --> LL[LevelLoader<br/>one in every level scene]
    LL -- spawns prefabs --> AI
    EG[ExitGate] -- CompleteLevel --> LL
    BH[BossHealth] -- CompleteLevel --> LL
    LL -- LevelCompleted --> SM[SaveManager<br/>created automatically,<br/>lives in all scenes]
    MM[MainMenuUI] -- IsUnlocked, SetVolume,<br/>ResetProgress --> SM
    SM -- Load / Save / Delete --> SS[SaveSystem<br/>JSON, try/catch, save.tmp]
    SS --> F[(save.json<br/>persistentDataPath)]
```

| Script | Job |
|---|---|
| EnemyStats + 3 children, PlayerStats, BossStats, ItemStats | All balance numbers as assets |
| LevelData | One level as data: enemies, coins, time limit, next scene |
| LevelLoader | Builds the level from LevelData, timer, CompleteLevel |
| SaveData | What is in save.json: version, unlocked levels, best times, volume |
| SaveSystem | Read/write JSON, safe loading, temp file |
| SaveManager | Save in memory; saves after a level, on pause and on quit |
| MainMenuUI | Level select (locked/unlocked), volume, reset with confirm |

# Script map — Assignment 12 (sound)

```mermaid
flowchart LR
    PC[PlayerController<br/>jump, fall] --> AM
    PS[PlayerShooter] --> AM
    CO[Coin] --> AM
    H[Health<br/>hurt, death] --> AM
    EH[EnemyHealth / BossHealth<br/>3 hit versions] --> AM
    AI[Patroller / Shooter / Charger AI<br/>wind-up at the START of the attack] --> AM
    LL[LevelLoader<br/>win, time up] --> AM
    PM[PauseMenu] -- SetPaused --> AM
    MM[MainMenuUI<br/>Music + SFX sliders] -- SetMusicVolume / SetSfxVolume --> SM[SaveManager<br/>save.json]
    SM --> AM

    AM[AudioManager<br/>Resources prefab, DontDestroyOnLoad<br/>random pitch 0.92-1.08<br/>crossfade 1 s] --> SFX[SFX group<br/>4 AudioSources]
    AM --> UI[UI group<br/>click on every Button]
    AM --> TR[Tracks group<br/>2 music AudioSources]
    TR --> MU[Music group<br/>MusicVol]
    SFX --> MA[Master]
    UI --> MA
    MU --> MA
    AM -- Normal / Paused snapshot<br/>Tracks: -6 dB, Lowpass 800 Hz --> TR
```

| Script | Job |
|---|---|
| AudioManager | Every sound in the game: SFX with random pitch, UI click, music crossfade, pause snapshot, volume in dB |
| MainMenuUI | Music and SFX sliders, read and write the save |
| SaveManager / SaveData | musicVolume and sfxVolume in save.json, applied at launch |
| PauseMenu | Calls SetPaused(true/false) |
| Gameplay scripts | Only call AudioManager.Instance.PlaySfx / PlayRandom / PlayDeath / PlayWin / PlayLose |

Why the slider is on **Music** and the pause effect on its child **Tracks**: after `SetFloat("MusicVol")` the script owns that volume and a snapshot cannot move it.
