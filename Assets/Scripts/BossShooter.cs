
using UnityEngine;

public class BossShooter : MonoBehaviour
{
    public BossStats stats;   // fire interval, bullet speed and damage: Boss_Stats asset
    public BossProjectile projectilePrefab;
    public Transform player;
    public float spawnOffset = 1.2f;   // m: bullet appears in front of the boss (technical)

    private float timer = 0f;

    void Update()
    {
        if (player == null || projectilePrefab == null)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= stats.fireInterval)
        {
            timer = 0f;
            Fire();
        }
    }

    void Fire()
    {
        Vector2 direction =
            (player.position - transform.position).normalized;

        Vector3 spawnPosition = transform.position
            + (Vector3)(direction * spawnOffset);

        BossProjectile bullet = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        bullet.Setup(direction, stats.bulletSpeed, stats.bulletDamage);
    }
}
