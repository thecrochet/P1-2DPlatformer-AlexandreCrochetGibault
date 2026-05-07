using UnityEngine;

public class FloorTrigger : BaseTrigger
{
    

    [Header("Visual")]
    [SerializeField] private Color activatedColor = Color.green;

    // cached renderers
    private SpriteRenderer spriteRenderer;
    private Renderer meshRenderer;
    private Color originalColor;
    private bool originalColorCached = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            originalColorCached = true;
        }
        else
        {
            meshRenderer = GetComponent<Renderer>();
            if (meshRenderer != null && meshRenderer.material != null)
            {
                originalColor = meshRenderer.material.color;
                originalColorCached = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            activated = true;
            ApplyActivatedColor();
        }
    }

    private void ApplyActivatedColor()
    {
        if (!originalColorCached)
            return;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = activatedColor;
        }
        else if (meshRenderer != null)
        {
            // use material instance so we don't modify shared material unexpectedly
            meshRenderer.material = new Material(meshRenderer.material);
            meshRenderer.material.color = activatedColor;
        }
    }
}