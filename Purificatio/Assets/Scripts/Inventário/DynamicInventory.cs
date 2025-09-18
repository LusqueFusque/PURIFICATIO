using System.Collections.Generic;
using UnityEngine;

public class DynamicInventory : MonoBehaviour
{
    [Header("Slots dinâmicos")]
    public int maxSlots = 3;
    public List<ItemData> freeItems = new List<ItemData>();

    public bool AddItem(ItemData item)
    {
        if (freeItems.Count >= maxSlots)
        {
            Debug.Log("Inventário cheio!");
            return false;
        }
        freeItems.Add(item);
        Debug.Log($"Adicionado {item.itemName}");
        return true;
    }

    public bool RemoveItem(ItemData item)
    {
        return freeItems.Remove(item);
    }

    public bool HasItem(string itemName)
    {
        foreach (var i in freeItems)
            if (i.itemName == itemName) return true;
        return false;
    }
}
