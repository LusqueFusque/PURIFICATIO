using UnityEngine;

/// Classe base abstrata para handlers de missão de cada fase.
/// Cada fase terá seu próprio handler herdando desta classe.
public abstract class MissionHandlerBase : MonoBehaviour
{
    /// Chamado quando uma missão é iniciada no JSON.
    /// Cada fase implementa sua própria lógica.
    public abstract void HandleMission(string missionId);
    
    /// Verifica se uma missão específica está completa.
    public virtual bool IsMissionComplete(string missionId)
    {
        if (MissionManager.Instance != null)
        {
            return MissionManager.Instance.IsCompleted(missionId);
        }
        return false;
    }
    
    /// Marca uma missão como completa e notifica o DialogueManager.
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
    
    /// Helper para acessar efeitos visuais comuns.
    protected VisualEffectsManager GetEffectsManager()
    {
        return FindObjectOfType<VisualEffectsManager>();
    }
}