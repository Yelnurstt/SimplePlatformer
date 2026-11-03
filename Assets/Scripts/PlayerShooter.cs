using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float cooldown = 0.4f;
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

        nextShotTime = Time.time + cooldown;

        Vector3 spawnPosition = transform.position
            + new Vector3(facing * 0.8f, 0f, 0f);

        GameObject bullet = Instantiate(
            bulletPrefab,
            spawnPosition,
            Quaternion.identity
        );

        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        if (playerBullet != null)
        {
            playerBullet.SetDirection(facing);
        }

        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, transform.position, 0.6f);
        }
    }
}
