using UnityEngine;

public class CruzItem : MonoBehaviour
{
    public static CruzItem Instance;

    private bool isActive = false;
    
    [Header("ItemData (ScriptableObject)")]
    public ItemData CruzData; 

    void Awake()
    {
        if (Instance != null && Instance != this) 
        { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    public void OnCruzCollected()
    {
        Debug.Log("[CrescenteItem] Crescente coletado!");
        var inv = FindObjectOfType<DynamicInventory>();
        if (inv != null && CruzData != null)
        {
            inv.AddItem(CruzData);
        }
        else
        {
            Debug.LogError("[CrescenteItem] Inventory ou ItemData não configurados!");
        }
        gameObject.SetActive(false);
    }
}