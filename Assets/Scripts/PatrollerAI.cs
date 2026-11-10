
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrollerAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack, Cooldown }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallsMask;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 2.8f;
    [SerializeField] private float patrolDistance = 1.2f;
    [SerializeField] private float maxChaseDistance = 4f;

    [Header("Vision")]
    [SerializeField] private float viewRange = 5f;
    [SerializeField] private float viewAngle = 140f;
    [SerializeField] private float loseSightTime = 1.5f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float windUp = 0.6f;
    [SerializeField] private float cooldown = 1.5f;
    [SerializeField] private int damage = 1;

    [Header("Debug")]
    [SerializeField] private State currentState = State.Patrol;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Health playerHealth;
    private Camera gameCamera;

    private Color normalColor;
    private float startX;
    private float leftX;
    private float rightX;
    private float stateTimer;
    private float lostTimer;
    private int direction = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
            normalColor = sr.color;

        startX = transform.position.x;
        leftX = startX;
        rightX = startX + patrolDistance;

        if (player != null)
            playerHealth = player.GetComponent<Health>();

        gameCamera = Camera.main;
    }

    void Update()
    {
        if (player == null) return;

        stateTimer += Time.deltaTime;
        bool seesPlayer = CanSeePlayer();

        switch (currentState)
        {
            case State.Patrol:
                if (seesPlayer)
                    SetState(State.Chase);
                break;

            case State.Chase:
                if (seesPlayer)
                    lostTimer = 0f;
                else
                    lostTimer += Time.deltaTime;

                if (lostTimer >= loseSightTime)
                {
                    SetState(State.Patrol);
                }
                else if (seesPlayer &&
                         DistanceToPlayer() <= attackRange &&
                         IsOnScreen())
                {
                    SetState(State.Attack);
                }
                break;

            case State.Attack:
                if (stateTimer >= windUp)
                {
                    if (DistanceToPlayer() <= attackRange &&
                        CanSeePlayer() &&
                        IsOnScreen())
                    {
                        if (playerHealth != null)
                            playerHealth.TakeDamage(damage);
                    }

                    SetState(State.Cooldown);
                }
                break;

            case State.Cooldown:
                if (stateTimer >= cooldown)
                {
                    SetState(seesPlayer ? State.Chase : State.Patrol);
                }
                break;
        }
    }

    void FixedUpdate()
    {
        float nextX = rb.position.x;

        if (currentState == State.Patrol)
        {
            float targetX = direction > 0 ? rightX : leftX;

            nextX = Mathf.MoveTowards(
                rb.position.x,
                targetX,
                patrolSpeed * Time.fixedDeltaTime
            );

            if (Mathf.Abs(nextX - targetX) < 0.02f)
                direction *= -1;
        }
        else if (currentState == State.Chase && player != null)
        {
            direction = player.position.x >= rb.position.x ? 1 : -1;

            float targetX = Mathf.Clamp(
                player.position.x,
                startX - maxChaseDistance,
                startX + maxChaseDistance
            );

            nextX = Mathf.MoveTowards(
                rb.position.x,
                targetX,
                chaseSpeed * Time.fixedDeltaTime
            );
        }

        if (currentState == State.Patrol ||
            currentState == State.Chase)
        {
            rb.MovePosition(new Vector2(nextX, rb.position.y));
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        // 1. Check distance
        if (distance > viewRange) return false;

        // 2. Check viewing angle
        Vector2 facing = Vector2.right * direction;

        if (Vector2.Angle(facing, toPlayer) > viewAngle / 2f)
            return false;

        // 3. Check for walls
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            toPlayer.normalized,
            distance,
            wallsMask
        );

        return hit.collider == null;
    }

    float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    bool IsOnScreen()
    {
        if (gameCamera == null) return false;

        Vector3 p = gameCamera.WorldToViewportPoint(
            transform.position
        );

        return p.z > 0f &&
               p.x >= 0f && p.x <= 1f &&
               p.y >= 0f && p.y <= 1f;
    }

    void SetState(State next)
    {
        if (currentState == next) return;

        currentState = next;
        stateTimer = 0f;

        if (next == State.Chase)
            lostTimer = 0f;

        // Red color warns the player before an attack
        if (sr != null)
            sr.color = next == State.Attack ? Color.red : normalColor;

        Debug.Log("Patroller state: " + next);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);
    }
}
