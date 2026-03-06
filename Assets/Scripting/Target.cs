using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventInt : UnityEvent<int> { }

public class Target : MonoBehaviour
{
    public UnityEventInt OnHealthChanged = new UnityEventInt();

    [SerializeField] private int health = 3;

    private int maxHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        if (!TryGetComponent(out spriteRenderer))
        {
            Debug.LogError("[Target] SpriteRenderer missing on Target. Disabling script.");
            enabled = false;
            return;
        }

        originalColor = spriteRenderer.color;
        maxHealth = Mathf.Max(1, health); 
    }

    private void Start()
    {
        OnHealthChanged.Invoke(health);
    }

    public void TakeDamage(int amount)
    {
        if (health <= 0)
            return;

        health -= amount;
        health = Mathf.Max(0, health);

       
        OnHealthChanged.Invoke(health);

        // Darken sprite progressively each time it's hit.
        
        float damagedCount = (float)(maxHealth - health);
        float t = Mathf.Clamp01(damagedCount / (float)maxHealth);
        spriteRenderer.color = Color.Lerp(originalColor, Color.black, t);

        if (health <= 0)
        {
            
            spriteRenderer.color = Color.black;
            

           
            Destroy(gameObject);
        }
    }
}