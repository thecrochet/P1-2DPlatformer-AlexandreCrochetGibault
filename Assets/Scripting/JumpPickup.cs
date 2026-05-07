
using UnityEngine;

public class JumpPickup : MonoBehaviour
{
    [Tooltip("Multiplier applied to player's base jump force (1.5 = 50% higher)")]
    [SerializeField] private float multiplier = 1.5f;

    [Tooltip("Duration in seconds. If <= 0, the boost is permanent.")]
    [SerializeField] private float duration = 0f;

    [SerializeField] private AudioClip pickupClip;
    [SerializeField][Range(0f, 2f)] private float pickupVolume = 2f;
    [SerializeField] private AudioClip pickupClip2;
    [SerializeField][Range(0f, 2f)] private float pickupVolume2 = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            if (pickupClip != null)
                AudioSource.PlayClipAtPoint(pickupClip2, transform.position, pickupVolume2);
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);


            player.GiveJumpBoost(multiplier, duration);
            Destroy(gameObject);
        }
    }
}