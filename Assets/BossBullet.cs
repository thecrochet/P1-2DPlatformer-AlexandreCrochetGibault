

using UnityEngine;
using UnityEngine.SceneManagement;

public class BossBullet : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private LayerMask wall;

    // owner to ignore collisions with and to avoid damaging owner's targets
    private GameObject owner;

    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        // ignore collisions with owner and owner's children
        if (owner != null && (collision.gameObject == owner || collision.transform.IsChildOf(owner.transform)))
            return;

        if (collision.CompareTag("Player"))
        {
            // Player hit by bullet -> restart level
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (collision.TryGetComponent(out Target target))
        {
            // ignore owner's eye targets
            if (owner != null && target.gameObject.transform.IsChildOf(owner.transform))
                return;

            target.SendMessage("TakeDamage", 1);
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent(out Puzzle ordered))
        {
            ordered.TakeDamage(1);
            Destroy(gameObject);
            return;
        }

        if (IsInLayerMask(collision.gameObject, wall))
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.collider == null) return;

        // ignore collisions with owner and owner's children
        if (owner != null && (collision.collider.gameObject == owner || collision.collider.transform.IsChildOf(owner.transform)))
            return;

        if (collision.collider.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (collision.gameObject.TryGetComponent(out Target target))
        {
            if (owner != null && target.gameObject.transform.IsChildOf(owner.transform))
                return;

            target.SendMessage("TakeDamage", 1);
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.TryGetComponent(out Puzzle ordered))
        {
            ordered.TakeDamage(1);
            Destroy(gameObject);
            return;
        }

        if (IsInLayerMask(collision.gameObject, wall))
        {
            Destroy(gameObject);
            return;
        }
    }

    // Initialize with direction and optional owner so bullet ignores the owner
    public void Initialize(Vector2 direction, GameObject owner = null)
    {
        this.owner = owner;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D myCol = GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.gravityScale = 0f;
        }

        // ignore collisions with owner's colliders (if provided)
        if (owner != null && myCol != null)
        {
            var ownerCols = owner.GetComponentsInChildren<Collider2D>();
            foreach (var oc in ownerCols)
            {
                if (oc != null)
                    Physics2D.IgnoreCollision(myCol, oc, true);
            }
        }
    }

    private bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) != 0;
    }
}