using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public BossStats stats;   // maxHp: Boss_Stats asset
    public AudioClip[] hitSounds;   // same 3 hit versions as the enemies
    public TMP_Text bossHpText;
    public Slider bossHpBar;

    private int hp;   // current HP lives here, not in the asset

    void Awake()
    {
        hp = stats.maxHp;
    }

    void Start()
    {
        UpdateHPText();
    }

    public void TakeDamage(int damage)
    {
        if (hp <= 0) return;

        hp = Mathf.Max(0, hp - damage);
        UpdateHPText();
        AudioManager.Instance.PlayRandom(hitSounds, 0.9f);
        Debug.Log("Boss HP: " + hp);

        if (hp <= 0)
        {
            Debug.Log("Boss Defeated!");
            LevelLoader.Instance.CompleteLevel();   // next scene comes from Level_03
        }
    }

    void UpdateHPText()
    {
        if (bossHpText != null)
        {
            bossHpText.text = "BOSS HP: " + hp + "/" + stats.maxHp;
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
