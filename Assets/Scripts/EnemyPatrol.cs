
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float patrolDistance = 1.2f;

    private Rigidbody2D rb;
    private float leftX;
    private float rightX;
    private int direction = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        leftX = transform.position.x;
        rightX = leftX + patrolDistance;
    }

    void FixedUpdate()
    {
        Vector2 nextPosition = rb.position;

        nextPosition.x +=
            direction * speed * Time.fixedDeltaTime;

        if (nextPosition.x >= rightX)
        {
            nextPosition.x = rightX;
            direction = -1;
        }
        else if (nextPosition.x <= leftX)
        {
            nextPosition.x = leftX;
            direction = 1;
        }

        rb.MovePosition(nextPosition);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Health playerHealth =
            other.GetComponent<Health>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1);
        }
    }
}
