using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Referências")]
    public Transform inventoryPanel; // Onde os botões aparecem
    public GameObject itemButtonPrefab; // Prefab do botão de item

    void Awake()
    {
        Instance = this;
    }

    public void AddItemButton(ItemData item)
    {
        GameObject newButton = Instantiate(itemButtonPrefab, inventoryPanel);
        newButton.name = $"Button_{item.itemName}";
        newButton.GetComponentInChildren<Text>().text = item.itemName;

        Image icon = newButton.GetComponentInChildren<Image>();
        if (icon != null && item.icon != null)
            icon.sprite = item.icon;
    }
}