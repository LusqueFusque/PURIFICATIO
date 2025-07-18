using UnityEngine;

public class MissionChecker : MonoBehaviour
{
    [Header("Refer�ncias")]
    public DialogueManager dialogueManager;   // arrasta o DialogueManager da cena aqui
    public InventoryManager inventoryManager; // arrasta o InventoryManager aqui

    [Header("Nome do item necess�rio (como configurado no CollectibleItem)")]
    public string requiredItemName = "camera";

    [Header("Painel que representa o c�modo principal")]
    public GameObject mainRoomPanel; // ex: PanelBGescrit�rio

    [Header("Miss�o foi cumprida? (debug)")]
    public bool missionCompleted = false;

    void Update()
    {
        // S� checa se a miss�o ainda n�o foi cumprida
        if (!missionCompleted)
        {
            // Verifica se o item foi coletado
            bool hasItem = inventoryManager.HasItem(requiredItemName);

            // Verifica se o jogador est� no c�modo principal
            bool inMainRoom = mainRoomPanel != null && mainRoomPanel.activeSelf;

            if (hasItem && inMainRoom)
            {
                missionCompleted = true;
                Debug.Log("Miss�o cumprida! Continuando di�logo...");

                // Chama o DialogueManager pra seguir
                if (dialogueManager != null)
                {
                    dialogueManager.ContinueDialogue();
                }
            }
        }
    }
}
