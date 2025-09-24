using UnityEngine;

public class PhoneItem : MonoBehaviour
{
    public PhoneUI phoneUI;

    public void OnPhoneButtonClicked()
    {
        // Marca a missão como completa
        MissionManager.Instance.CompleteMission("usePhone");

        // Pega as opções da linha de diálogo atual do DialogueManager
        var currentLine = DialogueManager.Instance.CurrentLine;
        if (currentLine != null && currentLine.options != null && currentLine.options.Count > 0)
        {
            phoneUI.ShowPhoneOptions(currentLine.options);
        }
        else
        {
            Debug.LogWarning("Não há opções para mostrar no telefone.");
        }
    }
}