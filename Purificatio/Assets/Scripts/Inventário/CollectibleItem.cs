using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemData itemData;

    public void Collect()
    {
        if (InventoryManager.Instance.AddItem(itemData))
        {
            Debug.Log($"{itemData.itemID} coletado!");
            Destroy(gameObject);
        }
    }
}