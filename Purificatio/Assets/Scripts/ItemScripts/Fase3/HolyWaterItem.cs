using UnityEngine;

/// <summary>
/// Item coletável: Água Benta (sem função por enquanto)
/// </summary>
public class HolyWaterItem : MonoBehaviour
{
    public static HolyWaterItem Instance;

    private bool isActive = false;
    
    [Header("ItemData (ScriptableObject)")]
    public ItemData HolyWaterData; 

    void Awake()
    {
        if (Instance != null && Instance != this) 
        { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    public void OnHolyWaterCollected()
    {
        Debug.Log("[HolyWaterItem] AguaBenta coletado!");
        var inv = FindObjectOfType<DynamicInventory>();
        if (inv != null && HolyWaterData != null)
        {
            inv.AddItem(HolyWaterData);
        }
        else
        {
            Debug.LogError("[HolyWaterItem] Inventory ou ItemData não configurados!");
        }
        gameObject.SetActive(false);
    }
}