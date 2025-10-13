using UnityEngine;

public class SaltMissionChecker : MonoBehaviour
{
    [Header("Item amaldiçoado")]
    public CursedItem cursedItem;          // Componente do item que será purificado
    [Header("Missão do tutorial")]
    public string missionId = "saltCursedObject";

    private bool missionCompleted = false;

    void Update()
    {
        if (missionCompleted || cursedItem == null) return;

        // ✅ Passo 1: item purificado
        if (!cursedItem.isCursed)
        {
            // ✅ Passo 2: câmera usada (missão "useCamera" completa)
            if (MissionManager.Instance != null &&
                MissionManager.Instance.IsCompleted("useCamera"))
            {
                // ✅ Completa a missão do sal
                MissionManager.Instance.CompleteMission(missionId);
                Debug.Log("[SaltMissionChecker] Missão 'saltCursedObject' completada!");

                missionCompleted = true;
                enabled = false; // desativa o checker
            }
        }
    }
}
