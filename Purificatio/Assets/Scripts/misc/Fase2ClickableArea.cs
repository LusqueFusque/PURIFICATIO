using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Sistema preguiçoso de click areas que muda backgrounds
/// </summary>
public class ClickableAreaHandler : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuração")]
    [Tooltip("ID da área (ex: 'ClickAreaGlass', 'ClickAreaChest')")]
    public string areaId;
    
    [Header("Item Necessário")]
    [Tooltip("Nome do item necessário (ex: 'Martelo', 'Chave', 'Chiclete')")]
    public string requiredItemName;
    
    [Header("Mudança de Background")]
    [Tooltip("Panel do background a ser alterado")]
    public Image backgroundPanel;
    
    [Tooltip("Nova sprite para o background")]
    public Sprite newBackgroundSprite;
    
    [Header("Missão (Opcional)")]
    [Tooltip("Missão a completar quando clicar (ex: 'glassBreak', 'chestOpen')")]
    public string missionToComplete;
    
    [Header("Sequência (Para Chest)")]
    [Tooltip("Se requer item anterior (ex: 'Chave' antes do 'Chiclete')")]
    public string requiredPreviousItem;
        
    [Header("Comportamento")]
    [Tooltip("Destruir esta ClickArea após uso?")]
    public bool destroyAfterUse = false;

    [Tooltip("Remover item do inventário após uso?")]
    public bool consumeItem = false;
    
    private DynamicInventory inventory;
    
    void Start()
    {
        inventory = FindObjectOfType<DynamicInventory>();
        if (inventory == null)
        {
            Debug.LogError("[ClickArea] DynamicInventory não encontrado!");
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"🔵 [ClickArea] Clicou em {areaId}");
    
        // ✅ Verifica se tem o item necessário no inventário
        if (!HasRequiredItem())
        {
            Debug.Log($"[ClickArea] Você precisa do item '{requiredItemName}' para interagir com {areaId}");
            return;
        }
    
        Debug.Log($"🟢 [ClickArea] Item '{requiredItemName}' encontrado!");
    
        // ✅ Para chest: verifica se precisa de item anterior primeiro
        // ✅ Completa missão (se houver)
        if (!string.IsNullOrEmpty(missionToComplete) && MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission(missionToComplete);
    
            Debug.Log($"[ClickArea] ✅ Missão '{missionToComplete}' completada!");
    
            // ✅ FORÇA atualização dos condicionais
            if (AdvancedMapManager.Instance != null)
            {
                Debug.Log("[ClickArea] Chamando RefreshAllConditionals...");
                AdvancedMapManager.Instance.RefreshAllConditionals();
                Debug.Log("[ClickArea] RefreshAllConditionals concluído!");
            }
            else
            {
                Debug.LogError("[ClickArea] AdvancedMapManager.Instance é NULL!");
            }
        }
    
        // ✅ Muda o background
        Debug.Log($"🟡 [ClickArea] Tentando trocar sprite...");
        Debug.Log($"🟡 [ClickArea] backgroundPanel null? {backgroundPanel == null}");
        Debug.Log($"🟡 [ClickArea] newBackgroundSprite null? {newBackgroundSprite == null}");
    
        if (backgroundPanel != null && newBackgroundSprite != null)
        {
            backgroundPanel.sprite = newBackgroundSprite;
            Debug.Log($"✅ [ClickArea] Background alterado para: {newBackgroundSprite.name}");
        }
        else
        {
            Debug.LogError("❌ [ClickArea] Panel ou Sprite não configurados no Inspector!");
        }
    
        // ✅ Marca que esse item foi usado
        MarkItemAsUsed(requiredItemName);
    
        // ✅ NOVO: Remove item do inventário se necessário
        if (consumeItem)
        {
            RemoveItemFromInventory(requiredItemName);
        }
    
        // ✅ NOVO: Destrói esta ClickArea após uso
        if (destroyAfterUse)
        {
            Debug.Log($"[ClickArea] Destruindo {areaId}");
            Destroy(gameObject);
        }
        // ✅ Marca flag para ativar a lâmpada
        if (areaId == "ClickAreaGum") // ou o nome que você deu
        {
            if (AdvancedMapManager.Instance != null)
            {
                AdvancedMapManager.Instance.SetGlobalFlag("ChestOpened", true);
                AdvancedMapManager.Instance.RefreshAllConditionals();
                Debug.Log("[ClickArea] Flag 'ChestOpened' ativada! Lâmpada liberada.");
            }
        }
    }

// ✅ NOVO: Método para remover item do inventário
    private void RemoveItemFromInventory(string itemName)
    {
        if (inventory == null) return;
    
        Debug.Log($"[ClickArea] Tentando remover '{itemName}' do inventário");
    
        // Remove o ItemData da lista
        ItemData itemToRemove = null;
        foreach (var item in inventory.items)
        {
            if (item.itemName.Equals(itemName, System.StringComparison.OrdinalIgnoreCase))
            {
                itemToRemove = item;
                break;
            }
        }
    
        if (itemToRemove == null)
        {
            Debug.LogWarning($"[ClickArea] Item '{itemName}' não encontrado no inventário!");
            return;
        }
    
        // Remove da lista
        inventory.items.Remove(itemToRemove);
    
        // Encontra o slot correspondente pela sprite do ícone
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            Button slot = inventory.slots[i];
            if (!slot.gameObject.activeSelf) continue;
        
            Image icon = slot.GetComponent<Image>();
            if (icon != null && icon.sprite == itemToRemove.icon)
            {
                slot.gameObject.SetActive(false);
                slot.onClick.RemoveAllListeners();
                Debug.Log($"[ClickArea] ✓ Item '{itemName}' removido do slot {i}");
                return;
            }
        }
    
        Debug.LogWarning($"[ClickArea] Slot visual do item '{itemName}' não encontrado!");
    }
    
    private bool HasRequiredItem()
    {
        if (string.IsNullOrEmpty(requiredItemName)) return true;
    
        // ✅ Verifica se o MARTELO está ativo
        if (requiredItemName.Equals("Martelo", System.StringComparison.OrdinalIgnoreCase) ||
            requiredItemName.Equals("Hammer", System.StringComparison.OrdinalIgnoreCase))
        {
            HammerItem hammer = FindObjectOfType<HammerItem>();
            if (hammer != null)
            {
                bool isActive = hammer.IsActive();
                Debug.Log($"[ClickArea] Martelo ativo? {isActive}");
                return isActive;
            }
        }
    
        // ✅ Verifica se a CHAVE está ativa
        else if (requiredItemName.Equals("Chave", System.StringComparison.OrdinalIgnoreCase) ||
                 requiredItemName.Equals("Key", System.StringComparison.OrdinalIgnoreCase))
        {
            KeyItem key = FindObjectOfType<KeyItem>();
            if (key != null)
            {
                bool isActive = key.IsActive();
                Debug.Log($"[ClickArea] Chave ativa? {isActive}");
                return isActive;
            }
        }
    
        // ✅ Verifica se o CHICLETE está no inventário (não precisa ativar)
        else if (requiredItemName.Equals("Chiclete", System.StringComparison.OrdinalIgnoreCase) ||
                 requiredItemName.Equals("Gum", System.StringComparison.OrdinalIgnoreCase))
        {
            if (inventory != null)
            {
                foreach (var item in inventory.items)
                {
                    if (item.itemName.Equals("Chiclete", System.StringComparison.OrdinalIgnoreCase) ||
                        item.itemName.Equals("Gum", System.StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }
    
        return false;
    }
    
    private void MarkItemAsUsed(string itemName)
    {
        PlayerPrefs.SetInt($"Used_{itemName}", 1);
        PlayerPrefs.Save();
    }
    
    private bool WasItemUsedBefore(string itemName)
    {
        return PlayerPrefs.GetInt($"Used_{itemName}", 0) == 1;
    }
}