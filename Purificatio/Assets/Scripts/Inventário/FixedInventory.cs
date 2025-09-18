using System.Collections.Generic;
using UnityEngine;

public class FixedInventory : MonoBehaviour
{
    [Header("Itens fixos")]
    public ItemData cameraItem;
    public ItemData phoneItem;
    public ItemData saltItem;

    public List<ItemData> GetAllFixedItems()
    {
        return new List<ItemData> { cameraItem, phoneItem, saltItem };
    }

    public bool HasItem(string itemName)
    {
        if (cameraItem != null && cameraItem.itemName == itemName) return true;
        if (phoneItem != null && phoneItem.itemName == itemName) return true;
        if (saltItem != null && saltItem.itemName == itemName) return true;
        return false;
    }
}
