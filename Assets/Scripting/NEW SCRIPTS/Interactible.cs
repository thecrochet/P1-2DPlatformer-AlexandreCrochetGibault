using TMPro;
using UnityEngine;

public class Interactible : MonoBehaviour
{

    [SerializeField] LayerMask interactableMask;

    [SerializeField] private Color highlightColor = Color.yellow;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    TextMeshPro interactionText;

    private void Awake()
    {
        TryGetComponent<SpriteRenderer>(out spriteRenderer);
        originalColor = GetComponent<SpriteRenderer>().color;


        interactionText = gameObject.GetComponentInChildren<TextMeshPro>();
    }

    public void Highlight(bool isActive)
    {
      if (isActive)
        {
            spriteRenderer.color = highlightColor;
            interactionText.text = "Press E to interact"; // Example interaction prompt
        }
        else
        {
            spriteRenderer.color = originalColor;
            interactionText.text = ""; // Clear the interaction prompt
        }
    }

    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        // Implement interaction logic here (e.g., open a door, pick up an item, etc.)
        gameObject.SetActive(false); // Example: Deactivate the object after interaction
    }
}
