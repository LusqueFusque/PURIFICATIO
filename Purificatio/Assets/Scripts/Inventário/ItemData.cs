using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;    
    public Sprite icon;         
    public MonoScript itemLogicScript; 


    public virtual void Use()
    {
        if (itemLogicScript != null)
        {
            Debug.Log("Usando o script: " + itemLogicScript.name + " para o item: " + itemName);

        }
        else
        {
            Debug.Log("Usando item: " + itemName);
        }
    }
}
