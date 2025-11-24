using UnityEngine;

public class FitaItem : MonoBehaviour
{
    public static FitaItem Instance;
    
    [Header("Dados do Item")]
    public ItemData fitaData;
    
    private bool isActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // ✅ Persiste entre cenas
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

    public void Toggle()
    {
        isActive = !isActive;
        Debug.Log($"[FitaItem] Toggle: Fita {(isActive ? "ATIVADA" : "DESATIVADA")}");
        
        if (isActive)
        {
            Debug.Log("[FitaItem] ✅ Clique na tela para assistir a fita!");
        }
    }

    public void Deactivate()
    {
        isActive = false;
        Debug.Log("[FitaItem] ❌ Fita desativada");
    }

    public bool IsActive() => isActive;
}