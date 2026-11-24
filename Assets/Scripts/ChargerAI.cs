using UnityEngine;

// Charger: waits, warns (blinks red), dashes in a straight line, then is stunned.
// Tests the player's timing: jump or step away at the end of the wind-up.
[RequireComponent(typeof(Rigidbody2D))]
public class ChargerAI : MonoBehaviour
{
    public enum State { Idle, WindUp, Dash, Stunned }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallsMask;      // tick only Walls
    [SerializeField] private AudioClip windUpSound;    // optional

    [Header("Data")]
    [SerializeField] private ChargerStats stats;   // all balance numbers: Charger_Stats asset

    [Header("Technical")]
    [SerializeField] private float blinkInterval = 0.1f;      // s: red/normal blink speed
    [SerializeField] private float wallCheckDistance = 0.4f;  // m: stops before a wall
    [SerializeField] private LayerMask groundMask = 1;        // Default layer: the floor
    [SerializeField] private float edgeCheckAhead = 0.4f;     // m: looks for floor in front
    [SerializeField] private float groundCheckLength = 1f;    // m: how far down to look for floor
    [SerializeField] private Color stunColor = Color.gray;

    [Header("Debug")]
    [SerializeField] private State currentState = State.Idle;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Camera gameCamera;
    private Color normalColor;

    private float timer;          // seconds in the current state
    private float startX;
    private float dashStartX;
    private int dashDir = 1;      // -1 = left, 1 = right
    private bool hitThisDash;

    void Awake()
    {
        // No data asset = the enemy cannot work. Say it clearly and switch off.
        if (stats == null)
        {
            Debug.LogError(name + ": Stats asset is missing", this);
            enabled = false;
            return;
        }

        // Enemies are created from LevelData, so a prefab cannot keep a link
        // to the player in the scene. Find the player once, here in Awake.
        if (player == null)
        {
            PlayerController pc = FindAnyObjectByType<PlayerController>();
            if (pc != null) player = pc.transform;
        }

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        gameCamera = Camera.main;
        startX = transform.position.x;

        if (sr != null)
            normalColor = sr.color;

        // Safety: "Nothing" in the Inspector would make walls invisible
        if (wallsMask.value == 0)
            wallsMask = LayerMask.GetMask("Walls");
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                if (timer >= stats.cooldown && CanSeePlayer() && IsOnScreen())
                    SetState(State.WindUp);
                break;

            case State.WindUp:
                // Blink red: this is the warning
                if (sr != null)
                {
                    bool red = (int)(timer / blinkInterval) % 2 == 0;
                    sr.color = red ? Color.red : normalColor;
                }

                if (timer >= stats.windUp)
                    SetState(State.Dash);
                break;

            case State.Dash:
                // The dash itself happens in FixedUpdate
                break;

            case State.Stunned:
                if (timer >= stats.stunTime)
                    SetState(State.Idle);
                break;
        }
    }

    void FixedUpdate()
    {
        if (currentState != State.Dash) return;

        float nextX = rb.position.x + dashDir * stats.dashSpeed * Time.fixedDeltaTime;

        bool tooFar = Mathf.Abs(nextX - dashStartX) >= stats.dashDistance;
        bool outOfArea = Mathf.Abs(nextX - startX) > stats.maxDistanceFromStart;

        if (tooFar || outOfArea || IsWallAhead() || !IsGroundAhead())
        {
            SetState(State.Stunned);
            return;
        }

        rb.MovePosition(new Vector2(nextX, rb.position.y));
    }

    bool CanSeePlayer()
    {
        Vector2 toPlayer = player.position - transform.position;

        // 1. Distance
        if (toPlayer.magnitude > stats.triggerRange) return false;

        // 2. Same floor (it can only dash left or right)
        if (Mathf.Abs(toPlayer.y) > stats.heightTolerance) return false;

        // 3. Walls block the view
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            toPlayer.normalized,
            toPlayer.magnitude,
            wallsMask
        );

        return hit.collider == null;
    }

    bool IsWallAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            Vector2.right * dashDir,
            wallCheckDistance,
            wallsMask
        );

        return hit.collider != null;
    }

    // A ray down, a bit in front of the enemy: no floor = edge, stop the dash
    bool IsGroundAhead()
    {
        Vector2 origin = rb.position + Vector2.right * dashDir * edgeCheckAhead;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckLength, groundMask);

        return hit.collider != null;
    }

    bool IsOnScreen()
    {
        if (gameCamera == null) return false;

        Vector3 p = gameCamera.WorldToViewportPoint(transform.position);

        return p.z > 0f &&
               p.x >= 0f && p.x <= 1f &&
               p.y >= 0f && p.y <= 1f;
    }

    void SetState(State next)
    {
        currentState = next;
        timer = 0f;

        if (next == State.WindUp)
        {
            // The direction is chosen now and does not change during the dash,
            // so the player can step out of the line.
            dashDir = player.position.x >= transform.position.x ? 1 : -1;

            if (windUpSound != null)
                AudioManager.Instance.PlaySfx(windUpSound, 0.8f);
        }

        if (next == State.Dash)
        {
            dashStartX = rb.position.x;
            hitThisDash = false;
        }

        if (sr != null)
            sr.color = next == State.Stunned ? stunColor : normalColor;

        Debug.Log("Charger state: " + next);
    }

    // The enemy collider is a trigger: damage only while dashing, once per dash
    void OnTriggerEnter2D(Collider2D other) { TryHit(other); }
    void OnTriggerStay2D(Collider2D other) { TryHit(other); }

    void TryHit(Collider2D other)
    {
        if (currentState != State.Dash || hitThisDash) return;

        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(stats.damage);
            hitThisDash = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.triggerRange);

        // Dash lane
        Gizmos.color = Color.red;
        Vector3 from = transform.position;
        Gizmos.DrawLine(from + Vector3.left * stats.dashDistance, from + Vector3.right * stats.dashDistance);
    }
}
