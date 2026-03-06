using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private bool useLookAhead = true;
    [SerializeField] private float lookAheadDistance = 1.5f;
    [SerializeField] private float lookAheadSmooth = 0.1f;

    [Header("Camera Bounds")]
    [SerializeField] private Vector2 minBounds; // world min (x, y)
    [SerializeField] private Vector2 maxBounds; // world max (x, y)

    private Vector3 velocity = Vector3.zero;
    private Vector3 lookAheadOffset = Vector3.zero;
    private Vector3 lookAheadVelocity = Vector3.zero;
    private Rigidbody2D targetRb;

    private void Start()
    {
        if (target != null) targetRb = target.GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

       
        if (useLookAhead && targetRb != null)
        {
            float direction = Mathf.Sign(targetRb.linearVelocity.x);
            float desiredLook = direction * lookAheadDistance;
            Vector3 desired = new Vector3(desiredLook, 0f, 0f);
            lookAheadOffset = Vector3.SmoothDamp(lookAheadOffset, desired, ref lookAheadVelocity, lookAheadSmooth);
        }
        else
        {
            lookAheadOffset = Vector3.zero;
        }

        Vector3 targetPos = target.position + offset + lookAheadOffset;
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // Clamp camera to borders
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        float minX = minBounds.x + camHalfWidth;
        float maxX = maxBounds.x - camHalfWidth;
        float minY = minBounds.y + camHalfHeight;
        float maxY = maxBounds.y - camHalfHeight;

        float clampedX = Mathf.Clamp(smoothPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothPos.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, offset.z);
    }
}
