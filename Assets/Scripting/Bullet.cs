using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 10f;
    private float lifeTime = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void Awake()
    {
        Destroy(gameObject, lifeTime); // Destroy the bullet after its lifetime expires
    }

    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Target target))
            {
                target.SendMessage("TakeDamage", 1); // Send a message to the target to take damage
                Destroy(gameObject); // Destroy the bullet after hitting the target
            }
    }


    public void Initialize(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed; // Set the bullet's velocity based on the direction and speed
        }
    }
}
