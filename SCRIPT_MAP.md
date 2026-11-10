# Script map — Assignment 10 (Level2)

```mermaid
flowchart LR
    PI[PlayerInput<br/>joystick, Jump, Fire] --> PC[PlayerController<br/>move, jump]
    PI --> PS[PlayerShooter]
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
