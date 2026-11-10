using System.Collections;
using UnityEngine;

// Health for the three AI enemies. The player's bullet calls TakeDamage.
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 2;            // hits to kill
    [SerializeField] private float flashTime = 0.1f;   // s: white flash on hit
    [SerializeField] private AudioClip hitSound;       // optional

    private int hp;
    private SpriteRenderer sr;

    void Awake()
    {
        hp = maxHp;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (hp <= 0) return;

        hp -= damage;

        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 0.8f);

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
