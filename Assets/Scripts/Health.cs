using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

// Player HP. Max HP and invincible time come from the Player_Stats asset.
// Current HP lives here, on the player, not in the asset.
public class Health : MonoBehaviour
{
    [Header("Data")]
    public PlayerStats stats;

    [Header("UI and sound")]
    public TMP_Text hpText;
    public AudioClip hurtSound;
    public float blinkInterval = 0.2f; // s: blink speed while invincible (visual only)

    private int hp;
    private bool invincible = false;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        hp = stats.maxHp;
    }

    void Start()
    {
        UpdateHPText();
    }

    public void TakeDamage(int damage)
    {
        if (invincible) return;

        hp -= damage;
        UpdateHPText();
        Debug.Log("HP: " + hp);

        if (hurtSound != null)
        {
            AudioManager.Instance.PlaySfx(hurtSound, 0.8f);
        }

        if (hp <= 0)
        {
            AudioManager.Instance.PlayDeath();   // death + lose sound, they survive the reload
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            StartCoroutine(Invincibility());
        }
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(stats.maxHp, hp + amount);
        UpdateHPText();
        Debug.Log("HP after healing: " + hp);
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        float time = 0f;

        while (time < stats.invincibleTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            time += blinkInterval;
        }

        sr.enabled = true;
        invincible = false;
    }

    void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + hp + "/" + stats.maxHp;
        }
    }
}
