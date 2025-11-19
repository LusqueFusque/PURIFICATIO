using UnityEngine;

/// <summary>
/// Lógica da LÂMPADA - Coletável que abre diálogo ao coletar
/// </summary>
public class LampItem : MonoBehaviour
{
    public static LampItem Instance;

    [Header("Configuração do Diálogo")]
    [Tooltip("ID do diálogo a abrir ao coletar (ex: 'nambulampada2')")]
    public string dialogueNodeId = "nambulampada2";

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
    // COLETAR LÂMPADA
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
            Debug.Log("[LampItem] ✓ Missão 'FindLamp' completada!");
        }

        // ✅ Abre o diálogo de opções
        if (DialogueManager.Instance != null)
        {
            Debug.Log($"[LampItem] Abrindo diálogo '{dialogueNodeId}'");
            DialogueManager.Instance.GoToNode(dialogueNodeId);
        }
    }

    public bool IsLampFound() => lampFound;
}