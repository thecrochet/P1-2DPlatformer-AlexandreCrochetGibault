using UnityEngine;

// Attach to a trigger collider (Is Trigger = true). Configure to enable/disable vertical follow on enter/exit.
[RequireComponent(typeof(Collider2D))]
public class CameraVerticalZone : MonoBehaviour
{
    [Tooltip("Enable vertical follow while player is inside this trigger.")]
    [SerializeField] private bool enableWhileInside = true;

    [Tooltip("If true, vertical follow will snap immediately when entering.")]
    [SerializeField] private bool snapOnEnter = true;

    [Tooltip("If true, the zone will enable vertical follow on enter and disable on exit. If false, it enables once and keeps it.")]
    [SerializeField] private bool oneShot = false;

    private CameraFollow camFollow;

    private void Awake()
    {
        // find the CameraFollow on the Main Camera
        if (Camera.main != null)
            camFollow = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (camFollow == null) return;
        if (!other.CompareTag("Player")) return;

        if (enableWhileInside)
        {
            //camFollow.SetVerticalFollow(true, snapOnEnter);
            if (oneShot) enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (camFollow == null) return;
        if (!other.CompareTag("Player")) return;

        if (enableWhileInside && !oneShot)
        {
            //camFollow.SetVerticalFollow(false, false);
        }
    }
}