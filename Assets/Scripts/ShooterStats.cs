using UnityEngine;

// windUp (from EnemyStats) = aim time before the shot
[CreateAssetMenu(fileName = "Shooter_Stats", menuName = "Game/Shooter Stats")]
public class ShooterStats : EnemyStats
{
    public float viewRange = 7f;     // m
    public float bulletSpeed = 5f;   // m/s
}
