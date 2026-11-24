using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.CollectCoin();

            if (pickupSound != null)
            {
                AudioManager.Instance.PlaySfx(pickupSound, 0.7f);
            }

            Destroy(gameObject);
        }
    }
}
