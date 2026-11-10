
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 10f;

    private float direction = 1f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    public void SetDirection(float newDirection)
    {
        direction = newDirection;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            direction * speed,
            0f
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            return;
        }

        BossHealth boss = other.GetComponent<BossHealth>();
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (boss != null)
        {
            boss.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (enemy != null)
        {
            // Patroller, Shooter or Charger
            enemy.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
