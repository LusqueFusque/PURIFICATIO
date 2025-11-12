using UnityEngine;
using UnityEngine.EventSystems;

public class CollectibleItem : MonoBehaviour, IPointerClickHandler
{
    public ItemData itemData;
    public DynamicInventory inventory;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemData == null || inventory == null)
        {
            Debug.LogWarning("ItemData ou Inventory não configurado!");
            return;
        }

        if (inventory.AddItem(itemData))
        {
            Debug.Log($"{itemData.itemName} coletado!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventário cheio!");
        }
    }
}