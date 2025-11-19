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
    
    [Header("Objetos a Ativar (Opcional)")]
    [Tooltip("GameObject a ativar após usar (ex: KeyImage, LampImage)")]
    public GameObject objectToActivate;
    
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
    
        // ✅ Verifica se tem o item necessário
        if (!HasRequiredItem())
        {
            Debug.Log($"[ClickArea] Você precisa do item '{requiredItemName}' para interagir com {areaId}");
            return;
        }
    
        Debug.Log($"🟢 [ClickArea] Item '{requiredItemName}' encontrado!");
    
        // ✅ Para chest: verifica se precisa de item anterior primeiro
        if (!string.IsNullOrEmpty(requiredPreviousItem))
        {
            if (!WasItemUsedBefore(requiredPreviousItem))
            {
                Debug.Log($"[ClickArea] Você precisa usar '{requiredPreviousItem}' antes do '{requiredItemName}'!");
                return;
            }
        }
    
        // ✅ Muda o background
        Debug.Log($"🟡 [ClickArea] Tentando trocar sprite...");
        
        if (backgroundPanel != null && newBackgroundSprite != null)
        {
            backgroundPanel.sprite = newBackgroundSprite;
            Debug.Log($"✅ [ClickArea] Background alterado para: {newBackgroundSprite.name}");
        }
        else
        {
            Debug.LogError("❌ [ClickArea] Panel ou Sprite não configurados no Inspector!");
        }
    
        // ✅ Completa missão (se houver)
        if (!string.IsNullOrEmpty(missionToComplete) && MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission(missionToComplete);
            Debug.Log($"[ClickArea] ✅ Missão '{missionToComplete}' completada!");
    
            if (AdvancedMapManager.Instance != null)
            {
                Debug.Log("[ClickArea] Chamando RefreshAllConditionals...");
                AdvancedMapManager.Instance.RefreshAllConditionals();
                Debug.Log("[ClickArea] RefreshAllConditionals concluído!");
            }
        }
    
        // ✅ Marca flag para lâmpada (se for chiclete)
        if (areaId == "ClickAreaGum")
        {
            if (AdvancedMapManager.Instance != null)
            {
                AdvancedMapManager.Instance.SetGlobalFlag("ChestOpened", true);
                AdvancedMapManager.Instance.RefreshAllConditionals();
                Debug.Log("[ClickArea] Flag 'ChestOpened' ativada! Lâmpada liberada.");
            }
        }
    
        // ✅ Marca que esse item foi usado
        MarkItemAsUsed(requiredItemName);
    
        // ✅ Remove item do inventário se necessário
        if (consumeItem)
        {
            RemoveItemFromInventory(requiredItemName);
        }
    
        // ✅ Chama métodos específicos dos itens
        CallItemMethod();
    
        // ✅ Destrói esta ClickArea após uso
        if (destroyAfterUse)
        {
            Debug.Log($"[ClickArea] Destruindo {areaId}");
            Destroy(gameObject);
        }
        
        // ✅ Ativa objeto se configurado
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log($"[ClickArea] ✓ Ativou {objectToActivate.name}");
        }
        
        // ✅ Destrói esta ClickArea após uso
        if (destroyAfterUse)
        {
            Debug.Log($"[ClickArea] Destruindo {areaId}");
            Destroy(gameObject);
        }
    }

    // ✅ Chama a lógica específica de cada item
    private void CallItemMethod()
    {
        // MARTELO: Quebra vidro e ativa chave
        if (areaId.Equals("ClickAreaGlass", System.StringComparison.OrdinalIgnoreCase))
        {
            HammerItem hammer = FindObjectOfType<HammerItem>();
            if (hammer != null)
            {
                hammer.UseHammer();
                Debug.Log("[ClickArea] ✓ HammerItem.UseHammer() chamado!");
            }
        }
        
        // CHAVE: Quebra no baú
        else if (areaId.Equals("ClickAreaChest", System.StringComparison.OrdinalIgnoreCase))
        {
            KeyItem key = FindObjectOfType<KeyItem>();
            if (key != null)
            {
                key.UseKey();
                Debug.Log("[ClickArea] ✓ KeyItem.UseKey() chamado!");
            }
        }
        
        // CHICLETE: Conserta chave e abre baú
        else if (areaId.Equals("ClickAreaGum", System.StringComparison.OrdinalIgnoreCase))
        {
            GumItem gum = FindObjectOfType<GumItem>();
            if (gum != null)
            {
                gum.UseGum();
                Debug.Log("[ClickArea] ✓ GumItem.UseGum() chamado!");
            }
        }
    }

    // ✅ Remove item do inventário
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
    
        // ✅ Verifica se o CHICLETE está ativo
        else if (requiredItemName.Equals("Chiclete", System.StringComparison.OrdinalIgnoreCase) ||
                 requiredItemName.Equals("Gum", System.StringComparison.OrdinalIgnoreCase))
        {
            GumItem gum = FindObjectOfType<GumItem>();
            if (gum != null)
            {
                bool isActive = gum.IsActive();
                Debug.Log($"[ClickArea] Chiclete ativo? {isActive}");
                return isActive;
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