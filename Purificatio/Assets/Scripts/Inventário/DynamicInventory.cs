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

// Se o item for o crowbar, liga o botão ao script na cena
        if (item.itemName == "Crowbar")
        {
            CrowbarItem crowbar = FindObjectOfType<CrowbarItem>();
            if (crowbar != null)
                slot.onClick.AddListener(() => crowbar.OnItemClicked());
            else
                Debug.LogWarning("Nenhum ItemCrowbar encontrado na cena!");
        }
        else
        {
            slot.onClick.AddListener(() => item.Use());
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