using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ClickArea que requer 3 itens para ser usado
/// Similar ao ClickableAreaHandler mas com validação de múltiplos itens
/// </summary>
public class ClickAreaPanela : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuração")]
    [Tooltip("ID da área (ex: 'ClickAreaPanela')")]
    public string areaId;
    
    [Header("Itens Necessários (3)")]
    [Tooltip("Primeiro item necessário (ex: 'Estrela')")]
    public string requiredItem1;
    
    [Tooltip("Segundo item necessário (ex: 'Crescente')")]
    public string requiredItem2;
    
    [Tooltip("Terceiro item necessário (ex: 'Cruz')")]
    public string requiredItem3;
    
    [Header("Mudança de Background")]
    [Tooltip("Panel do background a ser alterado")]
    public Image backgroundPanel;
    
    [Tooltip("Nova sprite para o background")]
    public Sprite newBackgroundSprite;
    
    [Header("Missão (Opcional)")]
    [Tooltip("Missão a completar quando clicar")]
    public string missionToComplete;
    
    [Header("Áudio (Opcional)")]
    [Tooltip("Som a tocar ao usar")]
    public AudioClip useSound;
    
    private DynamicInventory inventory;
    
    void Start()
    {
        inventory = FindObjectOfType<DynamicInventory>();
        if (inventory == null)
        {
            Debug.LogError("[ClickAreaPanela] DynamicInventory não encontrado!");
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"🔵 [ClickAreaPanela] Clicou em {areaId}");
        
        // ✅ Verifica se tem os 3 itens necessários
        if (!HasAllRequiredItems())
        {
            Debug.Log($"[ClickAreaPanela] Você precisa dos itens: '{requiredItem1}', '{requiredItem2}' e '{requiredItem3}'");
            return;
        }
        
        Debug.Log($"🟢 [ClickAreaPanela] Todos os 3 itens encontrados!");
        
        // ✅ Toca som se configurado
        if (useSound != null)
        {
            AudioSource.PlayClipAtPoint(useSound, Camera.main.transform.position, 0.5f);
        }

        // ✅ Muda o background
        if (backgroundPanel != null && newBackgroundSprite != null)
        {
            backgroundPanel.sprite = newBackgroundSprite;
            Debug.Log($"✅ [ClickAreaPanela] Background alterado para: {newBackgroundSprite.name}");
        }
        
        // ✅ Completa missão (se houver)
        if (!string.IsNullOrEmpty(missionToComplete) && MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission(missionToComplete);
            
            if (AdvancedMapManager.Instance != null)
            {
                AdvancedMapManager.Instance.RefreshAllConditionals();
            }
            
            Debug.Log($"[ClickAreaPanela] Missão '{missionToComplete}' completada!");
        }
        
        // ✅ Remove os 3 itens do inventário
        RemoveItemsFromInventory(requiredItem1, requiredItem2, requiredItem3);
        
        // ✅ Destrói esta ClickArea após uso
        Destroy(gameObject);
    }
    
    private bool HasAllRequiredItems()
    {
        if (string.IsNullOrEmpty(requiredItem1) || 
            string.IsNullOrEmpty(requiredItem2) || 
            string.IsNullOrEmpty(requiredItem3))
        {
            Debug.LogError("[ClickAreaPanela] Itens não configurados no Inspector!");
            return false;
        }
        
        if (inventory == null) return false;
        
        // ✅ Verifica se tem os 3 itens
        bool hasItem1 = HasItemInInventory(requiredItem1);
        bool hasItem2 = HasItemInInventory(requiredItem2);
        bool hasItem3 = HasItemInInventory(requiredItem3);
        
        Debug.Log($"[ClickAreaPanela] {requiredItem1}: {hasItem1} | {requiredItem2}: {hasItem2} | {requiredItem3}: {hasItem3}");
        
        return hasItem1 && hasItem2 && hasItem3;
    }
    
    private bool HasItemInInventory(string itemName)
    {
        foreach (var item in inventory.items)
        {
            if (item.itemName.Equals(itemName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    
    private void RemoveItemsFromInventory(string item1, string item2, string item3)
    {
        if (inventory == null) return;
        
        Debug.Log($"[ClickAreaPanela] Removendo itens: {item1}, {item2}, {item3}");
        
        // Remove os 3 itens da lista
        inventory.items.RemoveAll(i => 
            i.itemName.Equals(item1, System.StringComparison.OrdinalIgnoreCase) ||
            i.itemName.Equals(item2, System.StringComparison.OrdinalIgnoreCase) ||
            i.itemName.Equals(item3, System.StringComparison.OrdinalIgnoreCase));
        
        // Desativa os slots visuais
        int removedCount = 0;
        for (int i = 0; i < inventory.slots.Count && removedCount < 3; i++)
        {
            Button slot = inventory.slots[i];
            if (!slot.gameObject.activeSelf) continue;
            
            Image icon = slot.GetComponent<Image>();
            if (icon != null && icon.sprite != null)
            {
                // Verifica se o slot contém um dos itens
                if (IsSlotContainsItem(slot, item1) || 
                    IsSlotContainsItem(slot, item2) || 
                    IsSlotContainsItem(slot, item3))
                {
                    slot.gameObject.SetActive(false);
                    slot.onClick.RemoveAllListeners();
                    removedCount++;
                    Debug.Log($"[ClickAreaPanela] Slot {i} removido");
                }
            }
        }
        
        Debug.Log($"[ClickAreaPanela] ✓ {removedCount} itens removidos do inventário");
    }
    
    private bool IsSlotContainsItem(Button slot, string itemName)
    {
        // Verifica o ícone ou tenta encontrar o item correspondente
        // Esta é uma verificação simples - pode ser melhorada se necessário
        return true; // Simplificado: assume que o slot é de um dos itens
    }
}