using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Slots fixos")]
    public GameObject SlotCamera;
    public GameObject SlotCelular;
    public GameObject SlotSal;

    [Header("Slots de coletáveis")]
    public GameObject Slot1;
    public GameObject Slot2;
    public GameObject Slot3;

    // Adiciona um item ao inventário
    public void AddItem(string itemName)
    {
        switch (itemName.ToLower())
        {
            case "camera":
                SlotCamera.SetActive(true);
                break;
            case "celular":
                SlotCelular.SetActive(true);
                break;
            case "sal":
                SlotSal.SetActive(true);
                break;
            case "item1":
                Slot1.SetActive(true);
                break;
            case "item2":
                Slot2.SetActive(true);
                break;
            case "item3":
                Slot3.SetActive(true);
                break;
            default:
                Debug.LogWarning("Item desconhecido: " + itemName);
                break;
        }
    }

    // Verifica se já possui o item pelo nome
    public bool HasItem(string itemName)
    {
        switch (itemName.ToLower())
        {
            case "camera":
                return SlotCamera != null && SlotCamera.activeSelf;
            case "celular":
                return SlotCelular != null && SlotCelular.activeSelf;
            case "sal":
                return SlotSal != null && SlotSal.activeSelf;
            case "item1":
                return Slot1 != null && Slot1.activeSelf;
            case "item2":
                return Slot2 != null && Slot2.activeSelf;
            case "item3":
                return Slot3 != null && Slot3.activeSelf;
            default:
                Debug.LogWarning("Item desconhecido no HasItem: " + itemName);
                return false;
        }
    }
}
