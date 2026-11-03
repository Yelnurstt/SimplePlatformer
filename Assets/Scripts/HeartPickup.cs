
using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Health playerHealth = other.GetComponent<Health>();

        if (playerHealth != null)
        {
            playerHealth.Heal(1);
            Destroy(gameObject);
        }
    }
}
