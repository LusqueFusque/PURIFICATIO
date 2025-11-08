using UnityEngine;

/// <summary>
/// Classe base abstrata para handlers de missão de cada fase.
/// Cada fase terá seu próprio handler herdando desta classe.
/// </summary>
public abstract class MissionHandlerBase : MonoBehaviour
{
    /// <summary>
    /// Chamado quando uma missão é iniciada no JSON.
    /// Cada fase implementa sua própria lógica.
    /// </summary>
    public abstract void HandleMission(string missionId);

    /// <summary>
    /// Verifica se uma missão específica está completa.
    /// </summary>
    public virtual bool IsMissionComplete(string missionId)
    {
        if (MissionManager.Instance != null)
        {
            return MissionManager.Instance.IsCompleted(missionId);
        }
        return false;
    }

    /// <summary>
    /// Marca uma missão como completa e notifica o DialogueManager.
    /// </summary>
    protected void CompleteMission(string missionId)
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission(missionId);
            Debug.Log($"[{GetType().Name}] Missão '{missionId}' completada!");
        }
        else
        {
            Debug.LogError($"[{GetType().Name}] MissionManager não encontrado!");
        }
    }

    /// <summary>
    /// Helper para acessar efeitos visuais comuns.
    /// </summary>
    protected VisualEffectsManager GetEffectsManager()
    {
        return FindObjectOfType<VisualEffectsManager>();
    }
}