using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente para o objeto físico coletável da fita na cena
/// </summary>
public class CollectibleFita : MonoBehaviour, IPointerClickHandler
{
    void Start()
    {
        // Verifica se FitaItem existe
        if (FitaItem.Instance == null)
        {
            Debug.LogError("[CollectibleFita] FitaItem.Instance não encontrado! Crie um GameObject com FitaItem na cena.");
        }
        
        if (FitaItem.Instance != null && FitaItem.Instance.fitaData == null)
        {
            Debug.LogError("[CollectibleFita] FitaData não configurado no FitaItem!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[CollectibleFita] Fita clicada!");
        
        if (FitaItem.Instance == null || FitaItem.Instance.fitaData == null)
        {
            Debug.LogError("[CollectibleFita] FitaItem não configurado corretamente!");
            return;
        }

        // Adiciona ao inventário
        var inventory = FindObjectOfType<DynamicInventory>();
        if (inventory != null)
        {
            if (inventory.AddItem(FitaItem.Instance.fitaData))
            {
                Debug.Log("[CollectibleFita] ✅ Fita adicionada ao inventário!");
                
                // Completa missão
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.CompleteMission("findTape");
                }
                
                // Destrói objeto físico
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("[CollectibleFita] Inventário cheio!");
            }
        }
        else
        {
            Debug.LogError("[CollectibleFita] DynamicInventory não encontrado!");
        }
    }
}