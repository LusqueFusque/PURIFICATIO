using UnityEngine;

public class FitaItem : MonoBehaviour
{
    public static FitaItem Instance;

    [Header("ItemData (ScriptableObject)")]
    public ItemData FitaData; // referência ao ScriptableObject da fita

    private bool isActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) 
            Instance = null;
    }

    void Update()
    {
        // Clique direito para desativar
        if (isActive && Input.GetMouseButtonDown(1))
        {
            Deactivate();
        }
    }

    // ============================================
    // CONTROLE DE ATIVAÇÃO
    // ============================================
    public void Activate()
    {
        Debug.Log("[FitaItem] Fita ATIVADA");
        isActive = true;
    }

    public void Deactivate()
    {
        Debug.Log("[FitaItem] Fita DESATIVADA");
        isActive = false;
    }

    public void Toggle()
    {
        if (isActive)
            Deactivate();
        else
            Activate();
    }

    public bool IsActive() => isActive;

    // ============================================
    // COLETA DA FITA
    // ============================================
    /// <summary>
    /// Chamado quando o jogador coleta a fita no cenário.
    /// </summary>
    public void OnFitaCollected()
    {
        Debug.Log("[FitaItem] Fita coletada!");

        var inv = FindObjectOfType<DynamicInventory>();
        if (inv != null && FitaData != null)
        {
            inv.AddItem(FitaData);
        }
        else
        {
            Debug.LogError("[FitaItem] Inventory ou ItemData não configurados!");
        }

        // Opcional: marca missão concluída
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission("findTape");
        }
    }
}