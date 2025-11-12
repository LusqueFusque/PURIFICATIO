using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Itens/Novo Item")]
public class ItemData1 : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
}