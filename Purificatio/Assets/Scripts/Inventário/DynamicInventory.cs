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
        // FASE 1
        
        // CROWBAR
        if (item.itemName == "Crowbar")
        {
            Debug.Log("[DynamicInventory] Configurando botão para Crowbar");
            CrowbarItem crowbar = FindObjectOfType<CrowbarItem>();
            if (crowbar != null)
            {
                slot.onClick.AddListener(() => {
                    Debug.Log("[DynamicInventory] Botão Crowbar clicado!");
                    crowbar.Toggle();
                });
            }
            else
            {
                Debug.LogWarning("[DynamicInventory] CrowbarItem não encontrado!");
            }
        }
        
        // GRAMPEADOR
        else if (item.itemName == "Grampeador")
        {
            Debug.Log("[DynamicInventory] Configurando botão para Grampeador");
            StaplerItem stapler = FindObjectOfType<StaplerItem>();
            if (stapler != null)
            {
                slot.onClick.AddListener(() => {
                    Debug.Log("[DynamicInventory] Botão Grampeador clicado!");
                    stapler.Toggle();
                });
            }
            else
            {
                Debug.LogWarning("[DynamicInventory] StaplerItem não encontrado!");
            }
        }
        
        // ============================================
        // FASE 2
        
        // MARTELO
        else if (item.itemName == "Martelo" || item.itemName == "Hammer")
        {
            Debug.Log("[DynamicInventory] Configurando botão para Martelo");
            HammerItem hammer = FindObjectOfType<HammerItem>();
            if (hammer != null)
            {
                slot.onClick.AddListener(() => {
                    Debug.Log("[DynamicInventory] Botão Martelo clicado!");
                    hammer.Toggle();
                });
            }
            else
            {
                Debug.LogWarning("[DynamicInventory] HammerItem não encontrado!");
            }
        }
        
        // CHAVE
        else if (item.itemName == "Chave" || item.itemName == "Key")
        {
            Debug.Log("[DynamicInventory] Configurando botão para Chave");
            KeyItem key = FindObjectOfType<KeyItem>();
            if (key != null)
            {
                // Notifica KeyItem que foi coletada
                key.OnKeyCollected();
                
                slot.onClick.AddListener(() => {
                    Debug.Log("[DynamicInventory] Botão Chave clicado!");
                    key.Toggle();
                });
            }
            else
            {
                Debug.LogWarning("[DynamicInventory] KeyItem não encontrado!");
            }
        }
        
        // CHICLETE
        else if (item.itemName == "Chiclete" || item.itemName == "Gum")
        {
            Debug.Log("[DynamicInventory] Configurando botão para Chiclete");
            GumItem gum = FindObjectOfType<GumItem>();
            if (gum != null)
            {
                slot.onClick.AddListener(() => {
                    Debug.Log("[DynamicInventory] Botão Chiclete clicado!");
                    gum.Toggle(); // ← MUDOU AQUI: de UseGum() para Toggle()
                });
            }
            else
            {
                Debug.LogWarning("[DynamicInventory] GumItem não encontrado!");
            }
        }
        
        // LÂMPADA
        else if (item.itemName == "Lâmpada" || item.itemName == "Lamp")
        {
            Debug.Log("[DynamicInventory] Configurando botão para Lâmpada");
            LampItem lamp = FindObjectOfType<LampItem>();
            if (lamp != null)
            {
                // Notifica LampItem que foi coletada
                lamp.OnLampCollected();
                // Lâmpada não tem ação de clique (não é item usável)
            }
            else
            {
                Debug.LogWarning("[DynamicInventory] LampItem não encontrado!");
            }
        }
        
        // ============================================
        // OUTROS ITENS
        else
        {
            Debug.Log($"[DynamicInventory] Configurando botão para {item.itemName} (ItemData.Use())");
            slot.onClick.AddListener(() => {
                Debug.Log($"[DynamicInventory] Botão {item.itemName} clicado!");
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