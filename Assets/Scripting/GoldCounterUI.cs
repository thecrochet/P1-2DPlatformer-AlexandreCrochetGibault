using TMPro;
using UnityEngine;

public class GoldCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private string prefix = "Gold: ";


    private void Awake()
    {
        if(goldText == null && TryGetComponent(out goldText))
        {
            Debug.LogWarning($"GoldCounterUI on {gameObject.name} has no reference to a TMP_Text component. Attempting to use one on the same GameObject.");
            enabled = false;
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGoldChanged += UpdateGoldText;
        }
        UpdateGoldText(0); // Initialize text
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGoldChanged -= UpdateGoldText;
        }
    }

    private void UpdateGoldText(int newGoldCount)
    {
        goldText.text = prefix + newGoldCount;
    }
}
