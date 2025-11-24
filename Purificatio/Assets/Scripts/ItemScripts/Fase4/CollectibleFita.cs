using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente para coletar a fita no cenário
/// Anexe este script ao GameObject da fita coletável (Image/Sprite)
/// </summary>
public class CollectibleFita : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuração")]
    public ItemData fitaItemData; // Arraste o ScriptableObject da fita aqui
    
    private DynamicInventory inventory;

    void Start()
    {
        inventory = FindObjectOfType<DynamicInventory>();
        
        if (inventory == null)
        {
            Debug.LogError("[CollectibleFita] DynamicInventory não encontrado na cena!");
        }

        if (fitaItemData == null)
        {
            Debug.LogError("[CollectibleFita] ItemData da fita não configurado no Inspector!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[CollectibleFita] Clique detectado!");

        if (fitaItemData == null || inventory == null)
        {
            Debug.LogWarning("[CollectibleFita] ItemData ou Inventory não configurado!");
            return;
        }

        // Adiciona ao inventário
        if (inventory.AddItem(fitaItemData))
        {
            Debug.Log($"[CollectibleFita] ✓ {fitaItemData.itemName} adicionado ao inventário!");
            
            // Notifica o FitaItem
            if (FitaItem.Instance != null)
            {
                FitaItem.Instance.OnFitaCollected();
            }
            
            // Completa a missão findTape
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.CompleteMission("findTape");
                Debug.Log("[CollectibleFita] ✓ Missão 'findTape' completada!");
            }
            
            // Destrói o objeto da cena
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("[CollectibleFita] Inventário cheio!");
        }
    }
}