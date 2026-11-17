
using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    public ItemStats items;   // heartHeal: Item_Stats asset

    void OnTriggerEnter2D(Collider2D other)
    {
        Health playerHealth = other.GetComponent<Health>();

        if (playerHealth != null)
        {
            playerHealth.Heal(items.heartHeal);
            Destroy(gameObject);
        }
    }
}
