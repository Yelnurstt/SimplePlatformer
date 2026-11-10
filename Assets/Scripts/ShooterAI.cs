
using UnityEngine;

public class ShooterAI : MonoBehaviour
{
    public enum State { Idle, Aim, Shoot, Cooldown }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private BossProjectile projectilePrefab;

    [Header("Settings")]
    [SerializeField] private float viewRange = 7f;
    [SerializeField] private float aimTime = 0.6f;
    [SerializeField] private float cooldown = 1.5f;
    [SerializeField] private float spawnOffset = 0.8f;

    [Header("Debug")]
    [SerializeField] private State currentState = State.Idle;

    private SpriteRenderer sr;
    private Camera gameCamera;
    private Color normalColor;
    private float timer;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        gameCamera = Camera.main;

        if (sr != null)
            normalColor = sr.color;
    }

    void Update()
    {
        if (player == null || projectilePrefab == null)
            return;

        timer += Time.deltaTime;

        bool canAttack =
            Vector2.Distance(transform.position, player.position)
                <= viewRange
            && IsOnScreen();

        switch (currentState)
        {
            case State.Idle:
                if (canAttack)
                    SetState(State.Aim);
                break;

            case State.Aim:
                if (!canAttack)
                    SetState(State.Idle);
                else if (timer >= aimTime)
                    SetState(State.Shoot);
                break;

            case State.Shoot:
                Fire();
                SetState(State.Cooldown);
                break;

            case State.Cooldown:
                if (timer >= cooldown)
                    SetState(State.Idle);
                break;
        }
    }

    void Fire()
    {
        Vector2 direction =
            ((Vector2)(player.position - transform.position))
            .normalized;

        Vector3 spawnPosition =
            transform.position + (Vector3)(direction * spawnOffset);

        BossProjectile bullet = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        bullet.SetDirection(direction);
    }

    bool IsOnScreen()
    {
        if (gameCamera == null) return false;

        Vector3 point =
            gameCamera.WorldToViewportPoint(transform.position);

        return point.z > 0f &&
               point.x >= 0f && point.x <= 1f &&
               point.y >= 0f && point.y <= 1f;
    }

    void SetState(State next)
    {
        currentState = next;
        timer = 0f;

        if (sr != null)
            sr.color = next == State.Aim
                ? Color.red : normalColor;

        Debug.Log("Shooter state: " + next);
    }
}
