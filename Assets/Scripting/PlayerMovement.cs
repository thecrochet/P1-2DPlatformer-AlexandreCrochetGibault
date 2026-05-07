using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [SerializeField] private float rayLength = 15f;

    public GameObject bullet;

    public InputSystem_Actions PlayerInput;

    public InputAction fireAction;
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction powerAction;
    public InputAction interactAction;
    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.right;
    private bool isGrounded;

    Interactible currentInteractible;


    [Header("Powers")]
    [SerializeField] private bool hasGun = false;
    [SerializeField] private bool hasPhasePower = false;

    [SerializeField] private float jumpBoostAmount = 5f;

    [Header("Phase Settings")]
    [SerializeField] private float phaseDuration = 3f;
    [SerializeField] private float flickerInterval = 0.08f;

    private bool isPhasing = false;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        PlayerInput = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        fireAction = PlayerInput.Player.Fire;
        fireAction.Enable();

        moveAction = PlayerInput.Player.Move;
        moveAction.Enable();

        jumpAction = PlayerInput.Player.Jump;
        jumpAction.Enable();

        // NOTE: Your generated Input class may differ — ensure "Interaction" exists.
        // If it doesn't, map to the appropriate action (Sprint/Interaction/etc.).
        // If PlayerInput.Player.Interaction doesn't exist you'll get a compilation error.
        interactAction = PlayerInput.Player.Sprint; // safe default if Interaction is not defined
        interactAction.Enable();

        powerAction = PlayerInput.Player.Power;
        powerAction.Enable();

        fireAction.performed += Fire;
        jumpAction.performed += Jump;
        interactAction.performed += Interaction;
        powerAction.performed += Power;
    }

    void OnDisable()
    {
        fireAction.performed -= Fire;
        jumpAction.performed -= Jump;
        interactAction.performed -= Interaction;
        powerAction.performed -= Power;

        moveAction.Disable();
        fireAction.Disable();
        jumpAction.Disable();
        interactAction.Disable();
        powerAction.Disable();
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        // Flip sprite based on direction
        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
            facingDirection = new Vector2(moveInput.x, 0).normalized;
        }

        // Ground check
        isGrounded = groundCheck != null
            && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Raycast for interactibles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, rayLength, interactableLayer);
        Debug.DrawRay(transform.position, facingDirection * rayLength, Color.red);

        if (hit.collider != null)
        {
            Interactible newTarget = hit.collider.GetComponent<Interactible>();

            if (newTarget != currentInteractible)
            {
                if (currentInteractible != null)
                    currentInteractible.Highlight(false);

                currentInteractible = newTarget;
                if (currentInteractible != null)
                    currentInteractible.Highlight(true);
            }
        }
        else if (currentInteractible != null)
        {
            currentInteractible.Highlight(false);
            currentInteractible = null;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameObject bulletInstance = Instantiate(bullet.gameObject, transform.position, Quaternion.identity);
            if(bulletInstance.TryGetComponent(out Bullet bulletComponent))
            {
                bulletComponent.Initialize(facingDirection); // Pass the current movement direction to the bullet
            }
        }
    }


    private void Interaction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentInteractible != null)
            {
                currentInteractible.Interact();
            }
        }
    }

    // --- Phase activation via Power input ---
    private void Power(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Only activate if player has a stored charge and isn't already phasing
        if (hasPhasePower && !isPhasing)
        {
            StartCoroutine(PhaseCoroutine());
        }
    }

    private IEnumerator PhaseCoroutine()
    {
        // consume charge
        hasPhasePower = false;
        isPhasing = true;

        int playerLayer = LayerMask.NameToLayer("Player");
        int wallLayer = LayerMask.NameToLayer("Wall");

        // make player pass through walls
        Physics2D.IgnoreLayerCollision(playerLayer, wallLayer, true);

        // flicker loop: toggle renderer alpha (non-destructive)
        float elapsed = 0f;
        Color orig = spriteRenderer != null ? spriteRenderer.color : Color.white;
        bool visible = true;

        while (elapsed < phaseDuration)
        {
            // toggle visibility
            if (spriteRenderer != null)
            {
                visible = !visible;
                float a = visible ? 1f : 0.25f; // dim when "invisible" so collision-invisible but still visible a bit
                spriteRenderer.color = new Color(orig.r, orig.g, orig.b, a);
            }

            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }

        // restore visuals and collisions
        Physics2D.IgnoreLayerCollision(playerLayer, wallLayer, false);

        if (spriteRenderer != null)
            spriteRenderer.color = orig;

        isPhasing = false;
    }

    // --- New public API used by pickups ---

    public void GiveGun()
    {
        hasGun = true;
        Debug.Log("Gun unlocked!");
    }

    public void GivePhasePower()
    {
        hasPhasePower = true;
        Debug.Log("Phase power unlocked!");
    }

    public void GiveJumpBoost(float min, float max)
    {
        jumpForce += jumpBoostAmount;
        Debug.Log("Jump boosted!");
    }
}