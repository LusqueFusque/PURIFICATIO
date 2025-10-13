using UnityEngine;

public class SaltMissionChecker : MonoBehaviour
{
    [Header("Referências")]
    public CursedItem cursedSprite;      // o objeto amaldiçoado
    public PhotoCameraItem cameraItem;   // a câmera usada pelo jogador

    private bool purified = false;
    // NOVO: Flag para garantir que a câmera foi aberta APÓS a purificação.
    private bool cameraWasOpenedAfterPurify = false;
    private bool cameraClosedAfterPurify = false;
    private bool missionCompleted = false;

    void Update()
    {
        // Se a missão já foi completada, ou se faltam referências, saia.
        if (missionCompleted || cursedSprite == null || cameraItem == null) return;

        // 1️⃣ Detecta quando o item é purificado (Funciona)
        if (!purified && !cursedSprite.isCursed)
        {
            purified = true;
            Debug.Log("[SaltMissionChecker] Item foi purificado.");
        }

        // 2️⃣ Se purificado, verifica se a câmera foi ABERTA
        if (purified && !cameraWasOpenedAfterPurify)
        {
            // Se a máscara está ativa, a câmera foi aberta APÓS a purificação
            if (cameraItem.photoMask.activeSelf)
            {
                cameraWasOpenedAfterPurify = true;
                Debug.Log("[SaltMissionChecker] Câmera foi aberta após purificação.");
            }
        }

        // 3️⃣ Se a câmera foi aberta após purificação, verifica se foi FECHADA
        if (cameraWasOpenedAfterPurify && !cameraClosedAfterPurify)
        {
            // Se a máscara NÃO está ativa, a câmera foi fechada.
            if (!cameraItem.photoMask.activeSelf)
            {
                cameraClosedAfterPurify = true;
                Debug.Log("[SaltMissionChecker] Câmera fechada (máscara inativa) após purificação.");
            }
        }

        // 4️⃣ Completa a missão
        if (purified && cameraClosedAfterPurify && !missionCompleted)
        {
            missionCompleted = true;
            MissionManager.Instance?.CompleteMission("saltCursedObject");
            Debug.Log("[SaltMissionChecker] Missão 'saltCursedObject' COMPLETA!");
        }
    }

    private void OnDisable()
    {
        // Reseta todos os estados ao desativar/trocar de sala, garantindo que a lógica reinicie.
        purified = false;
        cameraWasOpenedAfterPurify = false;
        cameraClosedAfterPurify = false;
        missionCompleted = false;
    }
}