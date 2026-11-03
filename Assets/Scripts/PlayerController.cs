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

    private Rigidbody2D rb;
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
        if (value.isPressed && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            grounded = false;

            if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position, 0.8f);
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }

        if (collision.gameObject.CompareTag("Hazard"))
        {
            GetComponent<Health>().TakeDamage(1);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
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
