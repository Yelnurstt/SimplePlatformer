using UnityEngine;

[CreateAssetMenu(fileName = "Player_Stats", menuName = "Game/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;        // m/s
    public float jumpForce = 8f;
    public float coyoteTime = 0.1f;     // s: jump a moment after leaving the edge

    [Header("Health")]
    public int maxHp = 3;
    public float invincibleTime = 2f;   // s: no damage after a hit

    [Header("Shooting")]
    public float shootCooldown = 0.4f;  // s between shots
    public float bulletSpeed = 10f;     // m/s
    public int bulletDamage = 1;
}
