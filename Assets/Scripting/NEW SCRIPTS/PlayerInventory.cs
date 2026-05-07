
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    // ── Inspector Setup ─────────────────────────────────────
    [Header("Settings")]
    [SerializeField] private int _maxCapacity = 10;

    [Header("Events")]
    [SerializeField] private UnityEvent _onInventoryChanged;

    // ── Private State ────────────────────────────────────────
    private List<ItemData> m_heldItems;

    // ── Public Read-Only ─────────────────────────────────────
    public int Count => m_heldItems.Count;
    public bool IsFull => m_heldItems.Count >= _maxCapacity;

    public List<ItemData> HeldItems => m_heldItems;

    // ── Lifecycle ────────────────────────────────────────────
    private void Awake()
    {
        m_heldItems = new List<ItemData>();
    }

    // ── Public API ───────────────────────────────────────────

    /// <summary>
    /// Add an item to the inventory.
    /// Returns false if inventory is full.
    /// </summary>
    public bool PickUp(ItemData type)
    {
        // Guard: full inventory
        if (IsFull)
        {
            Debug.Log("[Inventory] Full — cannot pick up more items.");
            return false;
        }

        m_heldItems.Add(type);
        _onInventoryChanged?.Invoke();

        // Look up data from database for confirmation log
        /*if (ItemDatabase.Instance != null &&
            ItemDatabase.Instance.TryGetItemData(type, out ItemData data))
        {
            Debug.Log($"[Inventory] Picked up: {data.name})");
        }*/

        return true;
    }

    /// <summary>
    /// Use and remove the first item of a given type.
    /// Returns false if not found.
    /// </summary>
   
    public bool UseItem(ItemData type)
    {
        // Guard: item not in inventory
        if (!m_heldItems.Contains(type))
        {
            Debug.Log($"[Inventory] {type} not in inventory.");
            return false;
        }

        m_heldItems.Remove(type);
        _onInventoryChanged?.Invoke();
        Debug.Log($"[Inventory] Used: {type}");
        return true;
    }

    /// <summary>
    /// Use and remove the first item in the list (index 0).
    /// Returns false if inventory is empty.
    /// </summary>
    /// 
    [ContextMenu("Use first item")]
    public bool UseFirst()
    {
        // Guard: empty
        if (m_heldItems.Count == 0)
        {
            Debug.Log("[Inventory] Nothing to use.");
            return false;
        }

        ItemData first = m_heldItems[0];
        m_heldItems.RemoveAt(0);
        _onInventoryChanged?.Invoke();
        Debug.Log($"[Inventory] Used first item: {first}");
        return true;
    }

    /// <summary>
    /// Returns true if the player holds at least one of this type.
    /// </summary>
    public bool Has(ItemData type) => m_heldItems.Contains(type);

    /// <summary>
    /// Debug helper — logs all held items to the console.
    /// </summary>
    public void LogContents()
    {
        if (m_heldItems.Count == 0)
        {
            Debug.Log("[Inventory] Empty.");
            return;
        }

        // foreach loops over a List cleanly — no index needed
        foreach (ItemData item in m_heldItems)
            Debug.Log($"  - {item}");
    }

    public System.Collections.Generic.IReadOnlyList<ItemData> GetItems()
    {
        return m_heldItems;
    }
}