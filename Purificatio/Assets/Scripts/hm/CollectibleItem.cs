using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Tooltip("Nome exato do item, como no InventoryManager ('camera', 'celular', 'sal', etc)")]
    public string itemName;

    private bool isCollected = false;

    // Esse método será chamado pelo On Click() do Button
    public void OnCollect()
    {
        if (isCollected) return;

        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        if (inventory != null)
        {
            inventory.AddItem(itemName);
            isCollected = true;

            // Desativa o objeto na cena após coletar
            gameObject.SetActive(false);

            Debug.Log("Item coletado via botão: " + itemName);
        }
        else
        {
            Debug.LogError("InventoryManager não encontrado na cena!");
        }
    }
}
