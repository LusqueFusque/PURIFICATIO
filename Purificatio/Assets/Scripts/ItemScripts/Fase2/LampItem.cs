using UnityEngine;

/// <summary>
/// Lógica da LÂMPADA - Apenas dispara missão quando coletada
/// NÃO TEM Toggle/Activate (não é item usável)
/// </summary>
public class LampItem : MonoBehaviour
{
    public static LampItem Instance;

    private bool lampFound = false;

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

    // ============================================
    // ENCONTRAR LÂMPADA (Chamado por CollectibleItem)
    // ============================================
    public void OnLampCollected()
    {
        Debug.Log("[LampItem] OnLampCollected chamado!");

        if (lampFound)
        {
            Debug.Log("[LampItem] Lâmpada já foi encontrada.");
            return;
        }

        lampFound = true;

        // Completa missão FindLamp
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission("FindLamp");
            Debug.Log("[LampItem] ✓✓ Missão 'FindLamp' completada!");
        }
        else
        {
            Debug.LogError("[LampItem] ❌ MissionManager não encontrado!");
        }
    }

    public bool IsLampFound() => lampFound;
}