// VERSÃO COM LOGS EXTRAS PARA DEBUG
// Use esta versão se a missão ainda não funcionar

using UnityEngine;

public class SaltMissionChecker : MonoBehaviour
{
    [Header("Referências")]
    public CursedItem cursedSprite;

    private bool itemWasPurified = false;
    private bool cameraWasUsedAfterPurify = false;
    private bool missionCompleted = false;

    void OnEnable()
    {
        PhotoCameraItem.OnCameraClosed += OnCameraClosedHandler;
        Debug.Log("========================================");
        Debug.Log("[SaltMissionChecker] 🟢 HABILITADO - Listener registrado.");
        Debug.Log($"[SaltMissionChecker] CursedSprite atribuído? {cursedSprite != null}");
        if (cursedSprite != null)
            Debug.Log($"[SaltMissionChecker] Item está amaldiçoado? {cursedSprite.isCursed}");
        Debug.Log("========================================");
    }

    void OnDisable()
    {
        PhotoCameraItem.OnCameraClosed -= OnCameraClosedHandler;
        
        Debug.Log("========================================");
        Debug.Log("[SaltMissionChecker] 🔴 DESABILITADO - Resetando estados.");
        Debug.Log($"[SaltMissionChecker] Estado final - Purificado: {itemWasPurified}, Câmera usada: {cameraWasUsedAfterPurify}, Completo: {missionCompleted}");
        Debug.Log("========================================");
        
        itemWasPurified = false;
        cameraWasUsedAfterPurify = false;
        missionCompleted = false;
    }

    void Update()
    {
        if (missionCompleted) return;

        if (cursedSprite == null)
        {
            Debug.LogError("[SaltMissionChecker] ❌ cursedSprite é NULL!");
            enabled = false;
            return;
        }

        // Detecta purificação
        if (!itemWasPurified && !cursedSprite.isCursed)
        {
            itemWasPurified = true;
            Debug.Log("========================================");
            Debug.Log("[SaltMissionChecker] ✅ ITEM PURIFICADO!");
            Debug.Log("[SaltMissionChecker] Aguardando jogador usar a câmera...");
            Debug.Log("========================================");
        }
    }

    private void OnCameraClosedHandler()
    {
        Debug.Log("========================================");
        Debug.Log("[SaltMissionChecker] 📷 Evento OnCameraClosed recebido!");
        Debug.Log($"[SaltMissionChecker] Item foi purificado? {itemWasPurified}");
        Debug.Log($"[SaltMissionChecker] Câmera já foi usada antes? {cameraWasUsedAfterPurify}");
        Debug.Log($"[SaltMissionChecker] Missão já completa? {missionCompleted}");

        if (itemWasPurified && !cameraWasUsedAfterPurify)
        {
            cameraWasUsedAfterPurify = true;
            Debug.Log("[SaltMissionChecker] ✅ Câmera usada APÓS purificação!");
            CompleteSaltMission();
        }
        else if (!itemWasPurified)
        {
            Debug.Log("[SaltMissionChecker] ⚠️ Item ainda não foi purificado. Aguardando...");
        }
        else if (cameraWasUsedAfterPurify)
        {
            Debug.Log("[SaltMissionChecker] ℹ️ Câmera já foi usada anteriormente.");
        }
        
        Debug.Log("========================================");
    }

    private void CompleteSaltMission()
    {
        if (missionCompleted)
        {
            Debug.LogWarning("[SaltMissionChecker] ⚠️ Tentativa de completar missão já completa!");
            return;
        }

        missionCompleted = true;

        Debug.Log("========================================");
        Debug.Log("[SaltMissionChecker] 🎉🎉🎉 MISSÃO COMPLETA! 🎉🎉🎉");
        Debug.Log("========================================");

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission("saltCursedObject");
            Debug.Log("[SaltMissionChecker] ✅ MissionManager.CompleteMission() chamado!");
        }
        else
        {
            Debug.LogError("[SaltMissionChecker] ❌ MissionManager.Instance é NULL!");
        }
    }

    // MÉTODO DE DEBUG - Chame no Inspector ou via outro script
    [ContextMenu("Forçar Completar Missão (DEBUG)")]
    public void ForceCompleteMission()
    {
        Debug.Log("[SaltMissionChecker] 🔧 DEBUG: Forçando conclusão da missão!");
        itemWasPurified = true;
        cameraWasUsedAfterPurify = true;
        CompleteSaltMission();
    }

    // MÉTODO DE DEBUG - Mostra estado atual
    [ContextMenu("Mostrar Estado Atual (DEBUG)")]
    public void ShowCurrentState()
    {
        Debug.Log("========================================");
        Debug.Log("[SaltMissionChecker] 🔍 ESTADO ATUAL:");
        Debug.Log($"  - CursedSprite atribuído: {cursedSprite != null}");
        if (cursedSprite != null)
            Debug.Log($"  - Item amaldiçoado: {cursedSprite.isCursed}");
        Debug.Log($"  - Item purificado detectado: {itemWasPurified}");
        Debug.Log($"  - Câmera usada após purificar: {cameraWasUsedAfterPurify}");
        Debug.Log($"  - Missão completa: {missionCompleted}");
        Debug.Log($"  - MissionManager existe: {MissionManager.Instance != null}");
        Debug.Log("========================================");
    }
}