using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private ItemType type;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int value;

    public ItemType Type() { return type; }
    public string ItemName() { return itemName; }
    public Sprite Icon() { return icon; }
    public int Value() { return value; }
}



