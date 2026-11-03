
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGate : MonoBehaviour
{
    public string nextScene;

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

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.Log("Level Complete!");
            return;
        }

        SceneManager.LoadScene(nextScene);
    }
}
