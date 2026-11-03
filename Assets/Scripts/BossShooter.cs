
using UnityEngine;

public class BossShooter : MonoBehaviour
{
    public BossProjectile projectilePrefab;
    public Transform player;

    public float fireInterval = 2f;

    private float timer = 0f;

    void Update()
    {
        if (player == null || projectilePrefab == null)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= fireInterval)
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
            + (Vector3)(direction * 1.2f);

        BossProjectile bullet = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        bullet.SetDirection(direction);
    }
}
