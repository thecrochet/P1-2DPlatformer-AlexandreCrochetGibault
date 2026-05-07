using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text itemText;

    void Awake()
    {
        if (icon == null)
            Debug.LogError("InventorySlotUI: icon not assigned.", this);
        if (itemText == null)
            Debug.LogError("InventorySlotUI: itemText not assigned.", this);
    }


    public void SetData(ItemData itemData)
    {
        if (itemData == null)
        {
            icon.sprite = null;
            itemText.text = "";
            return;
        }

        icon.sprite = itemData.Icon();
        itemText.text = itemData.ItemName();
    }
}
