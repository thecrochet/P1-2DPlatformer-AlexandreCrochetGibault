using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotsParent;

    private void Awake()
    {
        if (_playerInventory == null)
            Debug.LogError("InventoryUI: _playerInventory not assigned.", this);
        if (_slotPrefab == null)
            Debug.LogError("InventoryUI: _slotPrefab not assigned.", this);
        if (_slotsParent == null)
            Debug.LogError("InventoryUI: _slotsParent not assigned.", this);
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in _slotsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemData item in _playerInventory.HeldItems)
        {
            Debug.Log($"InventoryUI: Adding item to UI: {item.ItemName()}", this);
            GameObject slot = Instantiate(_slotPrefab, _slotsParent);
            InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.SetData(item);
            }
            else
            {
                Debug.LogError("InventoryUI: Slot prefab does not have an InventorySlotUI component.", this);
            }

        }
    }
}