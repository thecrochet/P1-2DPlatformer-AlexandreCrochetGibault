using UnityEngine;


public class PhasePickup : MonoBehaviour
{
    [SerializeField] private AudioClip pickupClip;
    [SerializeField][Range(0f, 1f)] private float pickupVolume = 1f;
    [SerializeField] private AudioClip pickupClip2;
    [SerializeField][Range(0f, 2f)] private float pickupVolume2 = 2f;



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {

            if (pickupClip != null)
                AudioSource.PlayClipAtPoint(pickupClip2, transform.position, pickupVolume2);
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);

            player.GivePhasePower();
            Destroy(gameObject);
        }
    }
}