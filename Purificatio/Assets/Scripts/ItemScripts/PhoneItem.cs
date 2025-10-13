using UnityEngine;

public class PhoneItem : MonoBehaviour
{
    // Chamado ao clicar no botão do item celular
    public void OnPhoneButtonClicked()
    {
        var dialogueManager = DialogueManager.Instance;
        var missionManager = MissionManager.Instance;

        if (missionManager != null)
            missionManager.CompleteMission("usePhone");

        if (dialogueManager == null || dialogueManager.CurrentLine == null)
        {
            Debug.LogWarning("[PhoneItem] Nenhum diálogo ativo para o telefone.");
            return;
        }

        var currentLine = dialogueManager.CurrentLine;

        if (currentLine.options == null || currentLine.options.Count == 0)
        {
            Debug.LogWarning("[PhoneItem] O diálogo atual não possui opções de telefone.");
            return;
        }

        // Mostra as opções usando o DialogueUIManager
        dialogueManager.uiManager.ClearOptions(); // Limpa opções antigas
        foreach (var option in currentLine.options)
        {
            dialogueManager.uiManager.CreateOptionButton(option.optionText, () =>
            {
                dialogueManager.OnOptionSelected(option.nextId);
            });
        }
    }
}
