
using UnityEngine;

public class ShooterAI : MonoBehaviour
{
    public enum State { Idle, Aim, Shoot, Cooldown }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private BossProjectile projectilePrefab;

    [Header("Data")]
    [SerializeField] private ShooterStats stats;   // all balance numbers: Shooter_Stats asset

    [Header("Technical")]
    [SerializeField] private float spawnOffset = 0.8f;  // m: bullet appears in front of the enemy
    [SerializeField] private AudioClip windUpSound; // optional: plays when aiming starts

    [Header("Debug")]
    [SerializeField] private State currentState = State.Idle;

    private SpriteRenderer sr;
    private Camera gameCamera;
    private Color normalColor;
    private float timer;

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
                <= stats.viewRange
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
                else if (timer >= stats.windUp)
                    SetState(State.Shoot);
                break;

            case State.Shoot:
                Fire();
                SetState(State.Cooldown);
                break;

            case State.Cooldown:
                if (timer >= stats.cooldown)
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

        bullet.Setup(direction, stats.bulletSpeed, stats.damage);
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

        if (next == State.Aim && windUpSound != null)
            AudioSource.PlayClipAtPoint(windUpSound, transform.position, 0.8f);

        Debug.Log("Shooter state: " + next);
    }
}
