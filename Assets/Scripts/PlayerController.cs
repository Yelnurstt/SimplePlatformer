using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f;
    public TMP_Text coinsText;
    public AudioClip jumpSound;
    public float coyoteTime = 0.1f; // s: you can still jump a moment after leaving the edge

    private Rigidbody2D rb;
    private ContactFilter2D groundFilter;
    private float lastGroundedTime = -1f;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private bool grounded = false;
    private int coinsCollected = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // "Ground" = any solid collider under the feet:
        // the contact must push the player UP (normal angle 45..135 degrees).
        // Side contacts with a wall do not count, triggers do not count.
        groundFilter.useTriggers = false;
        groundFilter.useNormalAngle = true;
        groundFilter.minNormalAngle = 45f;
        groundFilter.maxNormalAngle = 135f;
    }

    void Start()
    {
        UpdateCoinsText();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        bool canJump = Time.time - lastGroundedTime <= coyoteTime;

        if (value.isPressed && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            grounded = false;
            lastGroundedTime = -1f; // no double jump

            if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position, 0.8f);
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);

        // Check the ground every physics step instead of Enter/Exit events.
        // Old way broke on the seam between two ground pieces:
        // Exit from piece A came after Enter on piece B -> "not grounded".
        grounded = rb.IsTouching(groundFilter);

        // Right after a jump the feet still touch the ground for one step;
        // the speed check stops that from counting as "on the ground".
        if (grounded && rb.linearVelocity.y < jumpForce * 0.5f)
            lastGroundedTime = Time.time;
    }

    void Update()
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput.x));
            anim.SetBool("IsGrounded", grounded);
        }

        if (sr != null && Mathf.Abs(moveInput.x) > 0.01f)
        {
            sr.flipX = moveInput.x < 0f;
        }

        if (transform.position.y < -6f)
        {
            RestartLevel();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            GetComponent<Health>().TakeDamage(1);
        }
    }

    public void CollectCoin()
    {
        coinsCollected++;
        UpdateCoinsText();
        Debug.Log("Coins: " + coinsCollected);
    }

    public bool HasAllCoins()
    {
        return coinsCollected >= 3;
    }

    void UpdateCoinsText()
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + coinsCollected + "/3";
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
