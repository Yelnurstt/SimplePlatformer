using UnityEngine;

// Side-scrolling camera: follows the player left/right, like in Mario.
// The height stays the same, so the jump does not shake the screen.
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;          // the Player
    [SerializeField] private float lookAhead = 2f;      // m: show more of the level in front
    [SerializeField] private float smoothTime = 0.15f;  // s: smaller = faster follow
    [SerializeField] private float minX = 0f;           // camera never goes left of this
    [SerializeField] private float maxX = 60f;          // camera never goes right of this

    private float velocityX;

    void LateUpdate()
    {
        if (target == null) return;

        float wantedX = Mathf.Clamp(target.position.x + lookAhead, minX, maxX);

        float x = Mathf.SmoothDamp(transform.position.x, wantedX, ref velocityX, smoothTime);

        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
