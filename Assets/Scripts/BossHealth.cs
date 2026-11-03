using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public int hp = 3;
    public TMP_Text bossHpText;
    public Slider bossHpBar;

    void Start()
    {
        UpdateHPText();
    }

    public void TakeDamage(int damage)
    {
        if (hp <= 0) return;

        hp = Mathf.Max(0, hp - damage);
        UpdateHPText();
        Debug.Log("Boss HP: " + hp);

        if (hp <= 0)
        {
            Debug.Log("Boss Defeated!");
            SceneManager.LoadScene("WinScreen");
        }
    }

    void UpdateHPText()
    {
        if (bossHpText != null)
        {
            bossHpText.text = "BOSS HP: " + hp + "/3";
        }

        if (bossHpBar != null)
        {
            bossHpBar.value = hp;
        }
    }

    [ContextMenu("Test Hit")]
    public void TestHit()
    {
        if (Application.isPlaying)
        {
            TakeDamage(1);
        }
    }
}
