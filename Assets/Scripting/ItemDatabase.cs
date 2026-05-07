using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] private ItemData Potion;
    [SerializeField] private ItemData Shield;
    [SerializeField] private ItemData Sword;

    private Dictionary<ItemType, ItemData> m_items;

    public static ItemDatabase Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of ItemDatabase found! Destroying the new one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BuildDatabase();
    }
    
    private void BuildDatabase()
    {
        m_items = new Dictionary<ItemType, ItemData>
        {
            { ItemType.Potion, Potion },
            { ItemType.Shield, Shield },
            { ItemType.Sword, Sword }
        };
    }

    public bool TryGetItemData(ItemType type, out ItemData itemData)
    {
        return m_items.TryGetValue(type, out itemData);
    }

    public bool ContainsItemType(ItemType type)
    {
        return m_items.ContainsKey(type);
    }
}
