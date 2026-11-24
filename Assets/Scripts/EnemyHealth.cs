using System.Collections;
using UnityEngine;

// Health for the three AI enemies. The player's bullet calls TakeDamage.
// Max HP comes from the same stats asset as the enemy's AI.
// Current HP lives here, on THIS enemy: never change stats.maxHp.
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;         // Patroller_Stats, Shooter_Stats or Charger_Stats
    [SerializeField] private float flashTime = 0.1f;   // s: white flash on hit (visual only)
    [SerializeField] private AudioClip[] hitSounds;    // 2-3 versions, one is picked at random

    private int hp;
    private SpriteRenderer sr;

    void Awake()
    {
        hp = stats.maxHp;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (hp <= 0) return;

        hp -= damage;

        AudioManager.Instance.PlayRandom(hitSounds, 0.8f);

        if (hp <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (sr != null)
            StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        Color before = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(flashTime);

        // The AI may have changed the color meanwhile (for example to red);
        // only put the old color back if it is still white.
        if (sr.color == Color.white)
            sr.color = before;
    }
}
