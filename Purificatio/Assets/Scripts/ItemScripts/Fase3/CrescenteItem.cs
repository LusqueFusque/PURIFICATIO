using UnityEngine;

public class CrescenteItem : MonoBehaviour
{
    public static CrescenteItem Instance;

    [Header("ItemData (ScriptableObject)")]
    public ItemData crescenteData; // arraste o Items/Crescente.asset no Inspector

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    public void OnCrescenteCollected()
    {
        Debug.Log("[CrescenteItem] Crescente coletado!");
        var inv = FindObjectOfType<DynamicInventory>();
        if (inv != null && crescenteData != null)
        {
            inv.AddItem(crescenteData);
        }
        else
        {
            Debug.LogError("[CrescenteItem] Inventory ou ItemData não configurados!");
        }
        gameObject.SetActive(false);
    }
}