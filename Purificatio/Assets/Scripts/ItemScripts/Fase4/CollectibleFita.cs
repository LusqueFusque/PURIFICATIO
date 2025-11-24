using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente para coletar a fita na cena
/// </summary>
public class CollectibleFita : MonoBehaviour, IPointerClickHandler
{
    public ItemData fitaItemData;

    private DynamicInventory inventory;

    void Start()
    {
        inventory = FindObjectOfType<DynamicInventory>();
        if (inventory == null)
            Debug.LogError("[CollectibleFita] DynamicInventory não encontrado!");

        if (fitaItemData == null)
            Debug.LogError("[CollectibleFita] ItemData não configurado!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (fitaItemData == null || inventory == null) return;

        if (inventory.AddItem(fitaItemData))
        {
            // Notifica FitaItem
            if (FitaItem.Instance != null)
                FitaItem.Instance.OnFitaCollected();

            // Destrói objeto na cena
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("[CollectibleFita] Inventário cheio!");
        }
    }
}