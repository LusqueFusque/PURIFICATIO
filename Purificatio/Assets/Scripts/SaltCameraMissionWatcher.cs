using UnityEngine;

public class SaltCameraMissionWatcher : MonoBehaviour
{
    [Header("Referências")]
    public CursedItem cursedItem;       // arraste o objeto amaldiçoado
    public CamItemTut cameraItem;       // arraste o script da câmera (renomeado ou não)

    private bool itemPurified = false;
    private bool cameraCycleAfterPurify = false;
    private bool missionCompleted = false;

    void Update()
    {
        if (missionCompleted) return;

        if (cursedItem == null || cameraItem == null)
        {
            Debug.LogError("[SaltCameraMissionWatcher] ❌ Referências não atribuídas!");
            enabled = false;
            return;
        }

        // 1️⃣ Detecta purificação
        if (!itemPurified && !cursedItem.isCursed)
        {
            itemPurified = true;
            Debug.Log("[SaltCameraMissionWatcher] ✅ Item purificado detectado. Agora o jogador deve usar a câmera.");
        }

        // 2️⃣ Detecta ciclo da câmera (abrir → fechar) após purificação
        if (itemPurified && !cameraCycleAfterPurify)
        {
            if (cameraItem.WasOpened && cameraItem.WasClosed)
            {
                cameraCycleAfterPurify = true;
                CompleteMission();
            }
        }
    }

    private void CompleteMission()
    {
        if (missionCompleted) return;

        missionCompleted = true;
        Debug.Log("[SaltCameraMissionWatcher] 🎉 Missão 'useSalt' COMPLETA!");

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission("useSalt");
        }
        else
        {
            Debug.LogError("[SaltCameraMissionWatcher] ❌ MissionManager.Instance é NULL!");
        }
    }
}
