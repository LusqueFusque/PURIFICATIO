using System;
using UnityEngine;

public class MissionChecker : MonoBehaviour
{
    public static MissionChecker Instance;
    private Action onComplete;
    private string currentMission;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartMission(string mission, Action callback)
    {
        currentMission = mission;
        onComplete = callback;

        Debug.Log("Missão iniciada: " + mission);
        // aqui você pode ativar UI de "objetivo" se quiser
    }

    // Chamado quando o jogador cumpre o objetivo
    public void CompleteMission(string mission)
    {
        if (mission != currentMission) return; // ignora se não for a missão ativa

        Debug.Log("Missão concluída: " + mission);
        currentMission = null;

        onComplete?.Invoke();
        onComplete = null;
    }

    public bool IsOnMission(string mission)
    {
        return currentMission == mission;
    }
}
