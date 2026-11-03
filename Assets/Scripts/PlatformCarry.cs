
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
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (collision.transform.parent == transform)
            {
                collision.transform.SetParent(null, true);
            }
        }
    }
}
