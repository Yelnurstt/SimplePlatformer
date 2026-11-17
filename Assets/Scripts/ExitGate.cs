
using UnityEngine;

// The exit. Which scene comes next is in the LevelData asset now.
public class ExitGate : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player == null)
        {
            return;
        }

        if (!player.HasAllCoins())
        {
            Debug.Log("Collect all coins first!");
            return;
        }

        LevelLoader.Instance.CompleteLevel();
    }
}
