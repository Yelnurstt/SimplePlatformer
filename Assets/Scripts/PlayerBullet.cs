
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;  // s: removed after this (technical)

    private float direction = 1f;
    private float speed;   // set by PlayerShooter from Player_Stats
    private int damage;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Setup(float newDirection, float newSpeed, int newDamage)
    {
        direction = newDirection;
        speed = newSpeed;
        damage = newDamage;
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
            boss.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (enemy != null)
        {
            // Patroller, Shooter or Charger
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
