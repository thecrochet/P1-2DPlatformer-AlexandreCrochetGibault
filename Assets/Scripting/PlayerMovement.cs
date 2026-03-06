using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Physics approach: Using Rigidbody2D with velocity manipulation
    // Because it'is simple to tune for platformer feel (gravity, collision response, friction).

    [SerializeField] private bool showDebug = true;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float coyoteTime = 0.12f; 
    [SerializeField] private float jumpBufferTime = 0.08f; 
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer; 

    [Header("Interaction Ray")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float rayLength = 15f; 

    [Header("References")]
    public GameObject bullet;
    public @_2DBinding PlayerInput;

    // InputActions (in OnEnable)
    public InputAction fireAction;
    public InputAction moveAction;
    public InputAction jumpAction;

    // Internal state
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.right;
    private Interactible currentInteractible;

    // Jump 
    private float lastGroundedTime = -999f;
    private float lastJumpPressedTime = -999f;
    private bool jumpPressed;
    private bool jumpHeld;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        if (PlayerInput == null)
            PlayerInput = new @_2DBinding();
    }

    private void OnEnable()
    {
        fireAction = PlayerInput.Player.Fire;
        fireAction.Enable();

        moveAction = PlayerInput.Player.Move;
        moveAction.Enable();

        jumpAction = PlayerInput.Player.Jump;
        jumpAction.Enable();

        fireAction.performed += Fire;

        // Use callbacks for jump to capture instant presses/releases making it more responsive
        jumpAction.performed += ctx => { OnJumpPressed(); };
        jumpAction.canceled += ctx => { OnJumpReleased(); };
    }

    private void OnDisable()
    {
        fireAction.performed -= Fire;
        fireAction.Disable();

        moveAction.Disable();

        jumpAction.Disable();
    }

    private void Update()
    {
        
        moveInput = moveAction.ReadValue<Vector2>();

        // Flip sprite and update facing direction
        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
            facingDirection = new Vector2(Mathf.Sign(moveInput.x), 0f).normalized;
        }

        // Interaction raycast 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, rayLength, interactableLayer);

        if (showDebug)
        {
            Debug.DrawRay(transform.position, facingDirection * rayLength, Color.red);
        }

        
        if (jumpPressed)
        {
            lastJumpPressedTime = Time.time;
            jumpPressed = false;
        }

        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            showDebug = !showDebug;
        }

        if (showDebug)
        {
            bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            Debug.Log(
                "Grounded: " + grounded +
                " | CoyoteTime: " + (Time.time - lastGroundedTime <= coyoteTime) +
                " | JumpBuffered: " + (Time.time - lastJumpPressedTime <= jumpBufferTime)
            );
        }
    }

    private void FixedUpdate()
    {
        // Ground check using OverlapCircle
       
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (grounded)
            lastGroundedTime = Time.time;

        // Horizontal movement with acceleration/deceleration
        float targetVelX = moveInput.x * moveSpeed;
        float accel = Mathf.Abs(targetVelX) > 0.01f ? acceleration : deceleration;
        float newVelX = Mathf.MoveTowards(rb.linearVelocity.x, targetVelX, accel * Time.fixedDeltaTime);

       
        rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);

        // Coyote time check
        bool canUseCoyote = (Time.time - lastGroundedTime) <= coyoteTime;
        bool bufferedJump = (Time.time - lastJumpPressedTime) <= jumpBufferTime;

        if (bufferedJump && canUseCoyote)
        {
            // Perform jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpPressedTime = -999f; 
        }

      
        if (rb.linearVelocity.y < 0)
        {
            // Falling faster
            rb.linearVelocity = rb.linearVelocity + (Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime);
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            // shorter jump
            rb.linearVelocity = rb.linearVelocity + (Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime);
        }
    }

    private void OnJumpPressed()
    {
       
        jumpPressed = true;
        jumpHeld = true;
    }

    private void OnJumpReleased()
    {
        jumpHeld = false;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (bullet != null)
            {
                GameObject bulletInstance = Instantiate(bullet, transform.position, Quaternion.identity);
                if (bulletInstance.TryGetComponent(out Bullet bulletComponent))
                {
                    bulletComponent.Initialize(facingDirection);
                }
            }

          
        }
    }

    // visualization of ground detection in editor via Gizmos (Debug.DrawRay used above)
    private void OnDrawGizmosSelected()
    {
        if (!showDebug) return;

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)facingDirection * rayLength);
    }
}