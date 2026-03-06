using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    TextMeshPro healthText;

    void Awake()
    {
        TryGetComponent(out healthText);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void UpdateHealthDisplay(int currentHealth)
    {
        healthText.text = $"Health: {currentHealth}";
    }
}


