using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventInt : UnityEvent<int> { }

public class Target : MonoBehaviour
{
    public UnityEventInt OnHealthChanged = new UnityEventInt();

    private int health = 3;

    SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        if (!TryGetComponent(out spriteRenderer))
        {
            enabled = false;
        }
    }

    private void Start()
    {
        OnHealthChanged.Invoke(health);
    }

    public void TakeDamage(int amount)
    {
        if (health == 0)
        {
            return;
        }

        health -= amount;
        OnHealthChanged.Invoke(health);
        if (health <= 0)
        {
            spriteRenderer.color = Color.gray; // Indicate that the target is "destroyed"
            TargetManager.Instance.NotifyTargetDestroyed();
            gameObject.SetActive(false); // Deactivate the target
        }
    }
}
