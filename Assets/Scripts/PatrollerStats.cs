using UnityEngine;

[CreateAssetMenu(fileName = "Patroller_Stats", menuName = "Game/Patroller Stats")]
public class PatrollerStats : EnemyStats
{
    [Header("Movement")]
    public float patrolSpeed = 2f;       // m/s
    public float chaseSpeed = 2.8f;      // m/s (player runs 5)
    public float patrolDistance = 3f;    // m: length of the route
    public float maxChaseDistance = 4f;  // m: never chases further from the start

    [Header("Vision")]
    public float viewRange = 5f;         // m
    public float viewAngle = 140f;       // degrees, whole cone
    public float loseSightTime = 1.5f;   // s: gives up after this

    [Header("Attack")]
    public float attackRange = 1.2f;     // m
}
