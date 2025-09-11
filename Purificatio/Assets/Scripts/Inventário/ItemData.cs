using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemID; // usado como referência única
    public Sprite icon;
    public bool isFixedItem; // true = sempre aparece nos slots fixos
    public int maxUses = 1;  // quantas vezes pode ser usado (ex: sal = 3, câmera = infinito)
    public string itemName;

    public ItemBehaviour behaviour; // script que define como o item é usado
}
