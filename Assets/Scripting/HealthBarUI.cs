using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider m_HealthSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        if(m_HealthSlider == null && !TryGetComponent(out m_HealthSlider))
        {
            Debug.LogError($"HealthBarUI on {gameObject.name} requires a Slider component.");
        }
       
        m_HealthSlider.value = 1f; // start at full health
        m_HealthSlider.minValue = 0f;
        m_HealthSlider.maxValue = 1f; // normalized health value
        m_HealthSlider.interactable = false; // make sure the slider is not interactable by the player

        if (GameManager.Instance != null)
            GameManager.Instance.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnHealthChanged -= UpdateHealthBar;
    }
    void UpdateHealthBar(float normalizedHealth)
    {
        m_HealthSlider.value = normalizedHealth;
    }
}
