using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 10f;
    private float lifeTime = 1f;
    [SerializeField] private LayerMask wall;


    private void Awake()
    {
        Destroy(gameObject, lifeTime); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Target target))
            {
                target.SendMessage("TakeDamage", 1); 
                Destroy(gameObject); 
            }

        
    }


    public void Initialize(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed; 
        }
    }
}
