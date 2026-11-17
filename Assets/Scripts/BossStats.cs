using UnityEngine;

[CreateAssetMenu(fileName = "Boss_Stats", menuName = "Game/Boss Stats")]
public class BossStats : ScriptableObject
{
    public int maxHp = 3;             // hits to win
    public float fireInterval = 2f;   // s between shots
    public float bulletSpeed = 5f;    // m/s
    public int bulletDamage = 1;
}
