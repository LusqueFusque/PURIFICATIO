using UnityEngine;

public class FitaItem : MonoBehaviour
{
    public static FitaItem Instance;

    [Header("Dados do Item")]
    public ItemData fitaData;

    private bool isActive = false;

    void Awake()
    {
        // Singleton simples
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
    public void OnFitaCollected()
    {
        Debug.Log("[FitaItem] Fita coletada!");

        // Adiciona ao inventário
        var inv = FindObjectOfType<DynamicInventory>();
        if (inv != null && fitaData != null)
        {
            inv.AddItem(fitaData);
        }
        else
        {
            Debug.LogError("[FitaItem] Inventory ou ItemData não configurados!");
        }

        // Completa missão
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission("findTape");
        }
    }
}