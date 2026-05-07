using UnityEngine;

public class GunPickup : MonoBehaviour
{

    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip pickupClip2;
    [SerializeField][Range(0f, 10f)] private float pickupVolume = 9f;
    [SerializeField][Range(0f, 10f)] private float pickupVolume2 = 10f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            if (pickupClip != null)
                AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);
            AudioSource.PlayClipAtPoint(pickupClip2, transform.position, pickupVolume2);

            player.GiveGun();
            Destroy(gameObject);
        }
    }
}