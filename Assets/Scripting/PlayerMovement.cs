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

    public ScriptingBinding PlayerInput;

    public InputAction fireAction;
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction interactAction;
    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.right;
    private bool isGrounded;

    Interactible currentInteractible;


    [Header("Powers")]
    [SerializeField] private bool hasGun = false;
    [SerializeField] private bool hasPhasePower = false;

    [SerializeField] private float jumpBoostAmount = 5f;
    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        PlayerInput = new ScriptingBinding();
    }

    private void OnEnable()
    {
        fireAction = PlayerInput.Player.Fire;
        fireAction.Enable();

        moveAction = PlayerInput.Player.Move;
        moveAction.Enable();

        jumpAction = PlayerInput.Player.Jump;
        jumpAction.Enable();

        interactAction = PlayerInput.Player.Interaction;
        interactAction.Enable();

        fireAction.performed += Fire;
        jumpAction.performed += Jump;
        interactAction.performed += Interaction; 
    }

    void OnDisable()
    {
        fireAction.performed -= Fire;
        jumpAction.performed -= Jump;
        interactAction.performed -= Interaction;

        moveAction.Disable();
        fireAction.Disable();
        jumpAction.Disable();
        interactAction.Disable();
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