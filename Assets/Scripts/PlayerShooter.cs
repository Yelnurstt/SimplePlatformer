using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    public PlayerStats stats;   // shoot cooldown, bullet speed and damage: Player_Stats asset
    public GameObject bulletPrefab;
    public float spawnOffset = 0.8f;   // m: bullet appears in front of the player (technical)
    public AudioClip shootSound;

    private float facing = 1f;
    private float nextShotTime = 0f;

    public void OnMove(InputValue value)
    {
        float x = value.Get<Vector2>().x;

        if (Mathf.Abs(x) > 0.1f)
        {
            facing = Mathf.Sign(x);
        }
    }

    public void OnFire(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        if (Time.time < nextShotTime)
        {
            return;
        }

        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet Prefab is missing!");
            return;
        }

        nextShotTime = Time.time + stats.shootCooldown;

        Vector3 spawnPosition = transform.position
            + new Vector3(facing * spawnOffset, 0f, 0f);

        GameObject bullet = Instantiate(
            bulletPrefab,
            spawnPosition,
            Quaternion.identity
        );

        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        if (playerBullet != null)
        {
            playerBullet.Setup(facing, stats.bulletSpeed, stats.bulletDamage);
        }

        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, transform.position, 0.6f);
        }
    }
}
