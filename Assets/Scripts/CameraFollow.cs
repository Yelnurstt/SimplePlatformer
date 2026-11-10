using UnityEngine;

// Side-scrolling camera: follows the player left/right, like in Mario.
// Up/down it moves only when the player gets too close to the top edge,
// so normal jumps do not shake the screen, but the head never leaves it.
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;          // the Player
    [SerializeField] private float lookAhead = 2f;      // m: show more of the level in front
    [SerializeField] private float smoothTime = 0.15f;  // s: smaller = faster follow
    [SerializeField] private float minX = 0f;           // camera never goes left of this
    [SerializeField] private float maxX = 60f;          // camera never goes right of this
    [SerializeField] private float minY = 0.5f;         // normal camera height
    [SerializeField] private float topMargin = 2f;      // m: free space kept above the player

    private Camera cam;
    private float velocityX;
    private float velocityY;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        float wantedX = Mathf.Clamp(target.position.x + lookAhead, minX, maxX);

        // Top edge of the view = camera y + orthographicSize.
        // Keep the player at least topMargin below it.
        float wantedY = Mathf.Max(minY, target.position.y + topMargin - cam.orthographicSize);

        float x = Mathf.SmoothDamp(transform.position.x, wantedX, ref velocityX, smoothTime);
        float y = Mathf.SmoothDamp(transform.position.y, wantedY, ref velocityY, smoothTime);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
