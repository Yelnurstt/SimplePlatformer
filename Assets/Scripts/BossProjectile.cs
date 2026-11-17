
using UnityEngine;

// Enemy bullet (Shooter and Boss). Speed and damage are set by the shooter
// from its stats asset when the bullet is created.
public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;  // s: removed after this (technical)

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.left;
    private float speed;
    private int damage;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 newDirection, float newSpeed, int newDamage)
    {
        direction = newDirection.normalized;
        speed = newSpeed;
        damage = newDamage;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Health playerHealth = other.GetComponent<Health>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") ||
                 other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            // Ground or a wall (Walls layer) stops the bullet
            Destroy(gameObject);
        }
    }
}
