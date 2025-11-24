using UnityEngine;
using System.Collections;

public class TutorialMissionHandler : MissionHandlerBase
{
    [Header("UI do Tutorial")]
    public GameObject dialoguePanel;
    public GameObject hudPanel;

    [Header("Componente de Verificação")]
    public SaltMissionChecker saltChecker;

    void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;
    }

    void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;
    }

    // Escuta todas as missões completadas
    private void OnMissionCompletedHandler(string missionId)
    {
        Debug.Log($"[TutorialMissionHandler] Missão completada: {missionId}");

        // Retoma o diálogo após missão completada
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }

    public override void HandleMission(string missionId)
    {
        Debug.Log($"[TutorialMissionHandler] Processando missão: {missionId}");

        switch (missionId)
        {
            case "useCamera":
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.StartMission("useCamera");
                    Debug.Log("[Tutorial] Aguardando jogador usar câmera...");
                }
                break;

            case "usePhone":
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.StartMission("usePhone");
                    Debug.Log("[Tutorial] Aguardando jogador usar telefone...");
                }
                break;

            case "saltCursedObject":
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.StartMission("saltCursedObject");
                    Debug.Log("[Tutorial] Aguardando jogador usar sal E depois verificar com câmera...");
                    
                    // ✅ Ativa o SaltMissionChecker para monitorar a missão
                    if (saltChecker != null)
                    {
                        saltChecker.enabled = true;
                        Debug.Log("[Tutorial] ✓ SaltMissionChecker ativado!");
                    }
                    else
                    {
                        Debug.LogError("[Tutorial] ❌ SaltMissionChecker não atribuído no Inspector!");
                    }
                }
                break;

            default:
                Debug.LogWarning($"[TutorialMissionHandler] Missão desconhecida: {missionId}");
                break;
        }
    }
}