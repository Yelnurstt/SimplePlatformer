
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.left;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Start()
    {
        Destroy(gameObject, 5f);
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
            playerHealth.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
