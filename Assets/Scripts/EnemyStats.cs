using UnityEngine;

// Base data for every AI enemy. Each enemy type adds its own numbers
// in a child class (PatrollerStats, ShooterStats, ChargerStats).
// Only START values live here. Current HP lives on the enemy (EnemyHealth).
public abstract class EnemyStats : ScriptableObject
{
    [Header("Common")]
    public int maxHp = 2;              // hits from the player's bullet to kill
    public int damage = 1;             // damage to the player
    [Range(0.3f, 0.8f)]
    public float windUp = 0.6f;        // s: warning before the attack
    public float cooldown = 1.5f;      // s: rest after the attack
}
