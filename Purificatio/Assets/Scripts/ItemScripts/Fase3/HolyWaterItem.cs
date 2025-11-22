using UnityEngine;

/// <summary>
/// Item coletável: Água Benta (sem função por enquanto)
/// </summary>
public class HolyWaterItem : MonoBehaviour
{
    public static HolyWaterItem Instance;

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
        // Botão direito desativa
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
        Debug.Log("[HolyWaterItem] Água Benta ATIVADA");
        isActive = true;
    }

    public void Deactivate()
    {
        Debug.Log("[HolyWaterItem] Água Benta DESATIVADA");
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

    public void OnHolyWaterCollected()
    {
        Debug.Log("[HolyWaterItem] Água Benta coletada!");
        // Função a ser implementada
    }
}