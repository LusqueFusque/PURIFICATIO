using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Tooltip("Nome exato do item, como no InventoryManager ('camera', 'celular', 'sal', etc)")]
    public string itemName;

    private bool isCollected = false;

    // Esse m�todo ser� chamado pelo On Click() do Button
    public void OnCollect()
    {
        if (isCollected) return;

        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        if (inventory != null)
        {
            inventory.AddItem(itemName);
            isCollected = true;

            // Desativa o objeto na cena ap�s coletar
            gameObject.SetActive(false);

            Debug.Log("Item coletado via bot�o: " + itemName);
        }
        else
        {
            Debug.LogError("InventoryManager n�o encontrado na cena!");
        }
    }
}
