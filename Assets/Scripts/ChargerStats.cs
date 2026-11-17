using UnityEngine;

[CreateAssetMenu(fileName = "Charger_Stats", menuName = "Game/Charger Stats")]
public class ChargerStats : EnemyStats
{
    [Header("Detection")]
    public float triggerRange = 5f;          // m: starts the wind-up
    public float heightTolerance = 1f;       // m: player must be on the same floor

    [Header("Dash")]
    public float dashSpeed = 7f;             // m/s
    public float dashDistance = 4f;          // m
    public float maxDistanceFromStart = 4f;  // m: stays in its own area

    [Header("Recovery")]
    public float stunTime = 1f;              // s: the player's chance to hit back
}
