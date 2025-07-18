using UnityEngine;

public class MissionChecker : MonoBehaviour
{
    [Header("Referências")]
    public DialogueManager dialogueManager;   // arrasta o DialogueManager da cena aqui
    public InventoryManager inventoryManager; // arrasta o InventoryManager aqui

    [Header("Nome do item necessário (como configurado no CollectibleItem)")]
    public string requiredItemName = "camera";

    [Header("Painel que representa o cômodo principal")]
    public GameObject mainRoomPanel; // ex: PanelBGescritório

    [Header("Missão foi cumprida? (debug)")]
    public bool missionCompleted = false;

    void Update()
    {
        // Só checa se a missão ainda não foi cumprida
        if (!missionCompleted)
        {
            // Verifica se o item foi coletado
            bool hasItem = inventoryManager.HasItem(requiredItemName);

            // Verifica se o jogador está no cômodo principal
            bool inMainRoom = mainRoomPanel != null && mainRoomPanel.activeSelf;

            if (hasItem && inMainRoom)
            {
                missionCompleted = true;
                Debug.Log("Missão cumprida! Continuando diálogo...");

                // Chama o DialogueManager pra seguir
                if (dialogueManager != null)
                {
                    dialogueManager.ContinueDialogue();
                }
            }
        }
    }
}
