using UnityEngine;

/// <summary>
/// Monitora o DollCursedAura. Se for desativado (sal jogado) e a boneca NÃO foi consertada, dispara exorcismo.
/// </summary>
public class DollExorcismTrigger : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("O GameObject 'DollCursedAura' que será desativado pelo sal")]
    public GameObject dollCursedAura;

    private bool wasActive = false;
    private bool exorcismTriggered = false;

    void Start()
    {
        if (dollCursedAura != null)
        {
            wasActive = dollCursedAura.activeSelf;
            Debug.Log($"[DollExorcismTrigger] Inicializado. DollCursedAura ativo: {wasActive}");
        }
        else
        {
            Debug.LogError("[DollExorcismTrigger] ❌ DollCursedAura não atribuído no Inspector!");
        }
    }

    void Update()
    {
        if (exorcismTriggered || dollCursedAura == null) return;

        // Detecta quando DollCursedAura foi desativado
        if (wasActive && !dollCursedAura.activeSelf)
        {
            Debug.Log("========================================");
            Debug.Log("[DollExorcismTrigger] 🧂 DollCursedAura foi DESATIVADO!");

            // Verifica se a boneca foi consertada
            bool dollWasFixed = MissionManager.Instance != null && 
                               MissionManager.Instance.IsCompleted("findDoll");

            Debug.Log($"[DollExorcismTrigger] Boneca foi consertada? {dollWasFixed}");

            if (!dollWasFixed)
            {
                // ✅ SAL JOGADO SEM CONSERTAR - DISPARA EXORCISMO
                Debug.Log("[DollExorcismTrigger] ⚡ Sal jogado SEM consertar! Disparando exorcismo!");
                TriggerExorcism();
            }
            else
            {
                Debug.Log("[DollExorcismTrigger] ℹ️ Boneca já foi consertada. Exorcismo não disparado.");
            }

            wasActive = false;
            Debug.Log("========================================");
        }
    }

    private void TriggerExorcism()
    {
        exorcismTriggered = true;

        // Esconde o painel de diálogo se estiver visível
        if (DialogueManager.Instance != null && DialogueManager.Instance.uiManager != null)
        {
            DialogueManager.Instance.uiManager.HideDialogueShowHUD();
            Debug.Log("[DollExorcismTrigger] Painel de diálogo escondido.");
        }

        // Chama o exorcismo via Fase1MissionHandler
        var missionHandler = FindObjectOfType<Fase1MissionHandler>();
        if (missionHandler != null)
        {
            Debug.Log("[DollExorcismTrigger] 🔥 Chamando HandleMission('exorcismoDaBoneca')");
            missionHandler.HandleMission("exorcismoDaBoneca");
        }
        else
        {
            Debug.LogError("[DollExorcismTrigger] ❌ Fase1MissionHandler não encontrado!");
        }
    }
}