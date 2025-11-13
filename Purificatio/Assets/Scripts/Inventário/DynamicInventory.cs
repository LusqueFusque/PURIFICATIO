using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicInventory : MonoBehaviour
{
    [Header("Slots fixos do inventário (arraste os 3 botões)")]
    public List<Button> slots = new List<Button>();

    [Header("Itens no inventário")]
    public List<ItemData> items = new List<ItemData>();

    public bool AddItem(ItemData item)
    {
        // se inventário cheio
        if (items.Count >= slots.Count)
        {
            Debug.Log("Inventário cheio!");
            return false;
        }

        // encontra o primeiro slot inativo
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].gameObject.activeSelf)
            {
                ActivateSlot(slots[i], item);
                items.Add(item);
                Debug.Log($"Item {item.itemName} adicionado ao slot {i}");
                return true;
            }
        }

        return false;
    }

    private void ActivateSlot(Button slot, ItemData item)
    {
        slot.gameObject.SetActive(true);

        Image icon = slot.GetComponent<Image>();
        if (icon != null && item.icon != null)
            icon.sprite = item.icon;

        slot.onClick.RemoveAllListeners();

        // ============================================
        // ITENS ESPECIAIS COM LÓGICA NA CENA
        // ============================================
        
        // CROWBAR: Usa CrowbarItem na cena
        if (item.itemName == "Crowbar")
        {
            Debug.Log("[DynamicInventory] Configurando botão para Crowbar (usando CrowbarItem na cena)");
            CrowbarItem crowbar = FindObjectOfType<CrowbarItem>();
            if (crowbar != null)
            {
                slot.onClick.AddListener(() => {
                    Debug.Log("[DynamicInventory] Botão Crowbar clicado! Chamando CrowbarItem.Toggle()");
                    crowbar.Toggle();
                });
            }
            else
            {
                Debug.LogWarning("[DynamicInventory] Nenhum CrowbarItem encontrado na cena!");
            }
        }
        // GRAMPEADOR: Usa StaplerItem na cena
        else if (item.itemName == "Grampeador")
        {
            Debug.Log("[DynamicInventory] Configurando botão para Grampeador (usando StaplerItem na cena)");
            StaplerItem stapler = FindObjectOfType<StaplerItem>();
            if (stapler != null)
            {
                slot.onClick.AddListener(() => {
                    Debug.Log("[DynamicInventory] Botão Grampeador clicado! Chamando StaplerItem.Toggle()");
                    stapler.Toggle();
                });
            }
            else
            {
                Debug.LogWarning("[DynamicInventory] Nenhum StaplerItem encontrado na cena!");
            }
        }
        // OUTROS ITENS: Usa o método Use() do ScriptableObject
        else
        {
            Debug.Log($"[DynamicInventory] Configurando botão para {item.itemName} (usando ItemData.Use())");
            slot.onClick.AddListener(() => {
                Debug.Log($"[DynamicInventory] Botão {item.itemName} clicado! Chamando ItemData.Use()");
                item.Use();
            });
        }
    }

    public void ClearInventory()
    {
        items.Clear();
        foreach (var s in slots)
        {
            s.gameObject.SetActive(false);
            Image img = s.GetComponent<Image>();
            if (img != null) img.sprite = null;
            s.onClick.RemoveAllListeners();
        }
    }
}