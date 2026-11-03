using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class Health : MonoBehaviour
{
    public int hp = 3;
    public TMP_Text hpText;
    public AudioClip hurtSound;

    private bool invincible = false;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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
            AudioSource.PlayClipAtPoint(hurtSound, transform.position, 0.8f);
        }

        if (hp <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            StartCoroutine(Invincibility());
        }
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(3, hp + amount);
        UpdateHPText();
        Debug.Log("HP after healing: " + hp);
    }

    IEnumerator Invincibility()
    {
        invincible = true;

        for (int i = 0; i < 10; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.2f);
        }

        sr.enabled = true;
        invincible = false;
    }

    void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + hp + "/3";
        }
    }
}
