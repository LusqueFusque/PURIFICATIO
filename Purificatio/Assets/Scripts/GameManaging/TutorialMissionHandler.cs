using UnityEngine;

public class TutorialMissionHandler : MissionHandlerBase
{
    [Header("UI do Tutorial")]
    public GameObject dialoguePanel;
    public GameObject hudPanel;

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

        // Avança o diálogo apenas quando o jogador completar de verdade
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }

    public override void HandleMission(string missionId)
    {
        Debug.Log($"[TutorialMissionHandler] Processando missão: {missionId}");

        switch (missionId)
        {
            case "useCamera":
            case "usePhone":
            case "useSalt":
                // Apenas registra que a missão está ativa
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.StartMission(missionId);
                    Debug.Log($"[TutorialMissionHandler] Missão '{missionId}' registrada. Aguardando jogador usar item...");
                }
                else
                {
                    Debug.LogError("[TutorialMissionHandler] MissionManager não encontrado!");
                }
                break;

            default:
                Debug.LogWarning($"[TutorialMissionHandler] Missão desconhecida: {missionId}");
                break;
        }
    }
}