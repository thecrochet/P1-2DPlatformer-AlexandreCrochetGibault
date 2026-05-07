
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private bool startFacingRight = false;

    [Header("Detection")]
    [Tooltip("Horizontal distance at which enemy starts chasing the player")]
    [SerializeField] private float detectionRange = 4f;
    [Tooltip("Max vertical difference to consider player on same platform")]
    [SerializeField] private float maxChaseYDiff = 1.5f;

    [Header("Ground & Wall checks")]
    [SerializeField] private Transform groundCheck; // point at feet/front to test for ground ahead
    [SerializeField] private float edgeCheckDistance = 0.6f;
    [SerializeField] private float wallCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Optional")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool showDebugRays = false;

    private Rigidbody2D rb;
    private Transform player;
    private int moveDir = -1; // -1 left, +1 right
    private bool chasing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        moveDir = startFacingRight ? 1 : -1;
    }

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void FixedUpdate()
    {
        float speed = patrolSpeed;

        // check for player and decide chase
        if (player != null)
        {
            float dx = player.position.x - transform.position.x;
            float dy = Mathf.Abs(player.position.y - transform.position.y);

            if (Mathf.Abs(dx) <= detectionRange && dy <= maxChaseYDiff)
            {
                chasing = true;
                moveDir = dx > 0 ? 1 : -1;
                speed = chaseSpeed;
            }
            else
            {
                chasing = false;
                speed = patrolSpeed;
            }
        }

        // when not chasing, do simple edge/wall patrol flip
        if (!chasing)
        {
            // wall ahead?
            Vector2 origin = transform.position;
            Vector2 wallOrigin = origin + Vector2.up * 0.1f; // slightly above feet
            RaycastHit2D wallHit = Physics2D.Raycast(wallOrigin, Vector2.right * moveDir, wallCheckDistance, groundLayer);
            bool wallAhead = wallHit.collider != null;

            // ground ahead?
            bool groundAhead = true;
            if (groundCheck != null)
            {
                Vector2 checkOrigin = groundCheck.position;
                RaycastHit2D hit = Physics2D.Raycast(checkOrigin, Vector2.down, edgeCheckDistance, groundLayer);
                groundAhead = hit.collider != null;
            }

            if (showDebugRays)
            {
                Debug.DrawRay(wallOrigin, Vector2.right * moveDir * wallCheckDistance, wallAhead ? Color.red : Color.green);
                if (groundCheck != null)
                    Debug.DrawRay(groundCheck.position, Vector2.down * edgeCheckDistance, groundAhead ? Color.green : Color.red);
            }

            if (wallAhead || !groundAhead)
            {
                // flip direction
                moveDir *= -1;
            }
        }

        // apply horizontal velocity (use rb.velocity, not linearVelocity)
        Vector2 v = rb.linearVelocity;
        v.x = moveDir * speed;
        rb.linearVelocity = v;

        // flip sprite based on movement direction
        if (spriteRenderer != null)
            spriteRenderer.flipX = (v.x < 0);
    }

    // Damage / contact with player: restart scene
    private void KillPlayerAndReload()
    {
        // You can play a death sound or animation here before reload.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Handle collision (non-trigger) with player and flip on wall collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.CompareTag("Player"))
        {
            KillPlayerAndReload();
            return;
        }

        // If we hit an obstacle (wall), check contact normals and flip horizontally
        if (collision.contacts != null && collision.contacts.Length > 0)
        {
            foreach (var contact in collision.contacts)
            {
                Vector2 n = contact.normal;
                // If normal has a strong horizontal component, we hit a vertical surface -> flip
                if (Mathf.Abs(n.x) > 0.5f)
                {
                    moveDir *= -1;
                    // push slightly away to avoid sticking (helps physics resolve)
                    rb.position = rb.position + new Vector2(-Mathf.Sign(n.x) * 0.02f, 0f);
                    return;
                }
            }
        }
    }

    // Handle trigger overlaps (in case player collider is a trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            KillPlayerAndReload();
        }
    }

    // simple visual helpers in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (groundCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * edgeCheckDistance);
        }

        Vector3 wallOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallOrigin, wallOrigin + Vector3.right * wallCheckDistance);
        Gizmos.DrawLine(wallOrigin, wallOrigin - Vector3.right * wallCheckDistance);
    }
}