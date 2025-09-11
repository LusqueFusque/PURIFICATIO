using UnityEngine;

[System.Serializable]
public class FreeSlot
{
    public ItemData item;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Itens Fixos (sempre presentes)")]
    public ItemData cameraItem;
    public ItemData phoneItem;
    public ItemData saltItem;

    [Header("3 Slots Livres (variáveis por fase)")]
    public FreeSlot slot1 = new FreeSlot();
    public FreeSlot slot2 = new FreeSlot();
    public FreeSlot slot3 = new FreeSlot();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Adiciona item em slot livre
    public bool AddItem(ItemData newItem)
    {
        if (newItem.isFixedItem)
        {
            Debug.Log($"{newItem.itemID} já é fixo.");
            return false;
        }

        if (slot1.item == null)
        {
            slot1.item = newItem;
            Debug.Log($"Adicionou {newItem.itemID} ao slot 1.");
            return true;
        }
        else if (slot2.item == null)
        {
            slot2.item = newItem;
            Debug.Log($"Adicionou {newItem.itemID} ao slot 2.");
            return true;
        }
        else if (slot3.item == null)
        {
            slot3.item = newItem;
            Debug.Log($"Adicionou {newItem.itemID} ao slot 3.");
            return true;
        }

        Debug.Log("Inventário cheio!");
        return false;
    }

    // Remove item de um slot livre
    public void RemoveItem(string itemID)
    {
        if (slot1.item != null && slot1.item.itemID == itemID) { slot1.item = null; return; }
        if (slot2.item != null && slot2.item.itemID == itemID) { slot2.item = null; return; }
        if (slot3.item != null && slot3.item.itemID == itemID) { slot3.item = null; return; }
    }

    // Verifica se possui item
    public bool HasItem(string itemID)
    {
        if (cameraItem != null && cameraItem.itemID == itemID) return true;
        if (phoneItem != null && phoneItem.itemID == itemID) return true;
        if (saltItem != null && saltItem.itemID == itemID) return true;

        if (slot1.item != null && slot1.item.itemID == itemID) return true;
        if (slot2.item != null && slot2.item.itemID == itemID) return true;
        if (slot3.item != null && slot3.item.itemID == itemID) return true;

        return false;
    }
}
