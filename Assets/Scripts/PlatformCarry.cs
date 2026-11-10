
using UnityEngine;

public class PlatformCarry : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.transform.SetParent(transform, true);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // When the level restarts, Unity turns objects off and still calls
        // this method. Changing the parent at that moment is not allowed,
        // and it is not needed: the scene is being unloaded anyway.
        if (!gameObject.activeInHierarchy ||
            !collision.gameObject.activeInHierarchy)
            return;

        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (collision.transform.parent == transform)
            {
                collision.transform.SetParent(null, true);
            }
        }
    }
}
