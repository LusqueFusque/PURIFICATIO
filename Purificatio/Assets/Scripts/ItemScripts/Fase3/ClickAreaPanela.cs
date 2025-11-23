using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickAreaPanela : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuração")]
    public string areaId;

    [Header("Itens Necessários (4)")]
    public string requiredItem1; // Estrela
    public string requiredItem2; // Crescente
    public string requiredItem3; // Cruz
    public string requiredItem4; // AguaBenta

    [Header("Áudio (Opcional)")]
    public AudioClip useSound;
    public AudioClip completeSound;

    private DynamicInventory inventory;
    private HashSet<string> itemsUsed = new HashSet<string>(); // Rastreia quais itens foram usados

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

        // Verifica qual item o jogador tem que ainda não foi usado
        string nextItem = GetNextRequiredItem();
        
        if (string.IsNullOrEmpty(nextItem))
        {
            Debug.Log($"[ClickAreaPanela] Todos os 4 itens já foram usados!");
            return;
        }

        if (!HasItem(nextItem))
        {
            Debug.Log($"[ClickAreaPanela] Você precisa do item: '{nextItem}'");
            return;
        }

        Debug.Log($"🟢 [ClickAreaPanela] Item '{nextItem}' encontrado!");

        if (useSound != null)
            AudioSource.PlayClipAtPoint(useSound, Camera.main.transform.position, 0.5f);

        // ✅ Remove o item
        RemoveItemFromInventory(nextItem);
        itemsUsed.Add(nextItem); // Marca como usado

        Debug.Log($"[ClickAreaPanela] {itemsUsed.Count}/4 itens usados");

        // ✅ Se todos os 4 itens foram usados, cria ArmaSanta
        if (itemsUsed.Count >= 4)
        {
            Debug.Log("[ClickAreaPanela] ✅ Todos os 4 itens usados! Criando ArmaSanta...");
            
            if (completeSound != null)
                AudioSource.PlayClipAtPoint(completeSound, Camera.main.transform.position, 0.7f);
            
            GiveArmaSanta();
            Destroy(gameObject);
        }
    }

    private string GetNextRequiredItem()
    {
        // Retorna um item que ainda não foi usado e que o jogador tem
        if (!itemsUsed.Contains(requiredItem1) && HasItem(requiredItem1)) 
            return requiredItem1;
        
        if (!itemsUsed.Contains(requiredItem2) && HasItem(requiredItem2)) 
            return requiredItem2;
        
        if (!itemsUsed.Contains(requiredItem3) && HasItem(requiredItem3)) 
            return requiredItem3;
        
        if (!itemsUsed.Contains(requiredItem4) && HasItem(requiredItem4)) 
            return requiredItem4;
        
        return null;
    }

    private bool HasItem(string itemName)
    {
        if (inventory == null) return false;
        
        foreach (var item in inventory.items)
        {
            if (item.itemName.Equals(itemName, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    // ✅ Remove item do inventário
    private void RemoveItemFromInventory(string itemName)
    {
        if (inventory == null) return;

        Debug.Log($"[ClickAreaPanela] Removendo '{itemName}' do inventário");

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
            Debug.LogWarning($"[ClickAreaPanela] Item '{itemName}' não encontrado no inventário!");
            return;
        }

        inventory.items.Remove(itemToRemove);

        for (int i = 0; i < inventory.slots.Count; i++)
        {
            Button slot = inventory.slots[i];
            if (!slot.gameObject.activeSelf) continue;

            Image icon = slot.GetComponent<Image>();
            if (icon != null && icon.sprite == itemToRemove.icon)
            {
                slot.gameObject.SetActive(false);
                slot.onClick.RemoveAllListeners();
                Debug.Log($"[ClickAreaPanela] ✓ Item '{itemName}' removido do slot {i}");
                return;
            }
        }

        Debug.LogWarning($"[ClickAreaPanela] Slot visual do item '{itemName}' não encontrado!");
    }

    private void GiveArmaSanta()
    {
        ItemData armaSanta = Resources.Load<ItemData>("Items/ArmaSanta");
        if (armaSanta != null)
        {
            bool added = inventory.AddItem(armaSanta);
            if (added)
                Debug.Log("[ClickAreaPanela] 🎁 ArmaSanta adicionada ao inventário!");
            else
                Debug.LogWarning("[ClickAreaPanela] Inventário cheio, não foi possível adicionar ArmaSanta!");
        }
        else
        {
            Debug.LogError("[ClickAreaPanela] ItemData 'ArmaSanta' não encontrado em Resources/Items!");
        }
    }
}