using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [System.Serializable]
    public class Slot
    {
        public string slotName;
        public Image icon;
        public Button button;
    }

    public Slot[] slots;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItemToHUD(ItemData item)
    {
        foreach (var slot in slots)
        {
            if (slot.slotName == item.itemName)
            {
                slot.icon.sprite = item.icon;
                slot.icon.gameObject.SetActive(true);
                slot.button.gameObject.SetActive(true);
            }
        }
    }

    public void RemoveItemFromHUD(string itemName)
    {
        foreach (var slot in slots)
        {
            if (slot.slotName == itemName)
            {
                slot.icon.gameObject.SetActive(false);
                slot.button.gameObject.SetActive(false);
            }
        }
    }
}
