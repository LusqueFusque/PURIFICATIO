using UnityEngine;

public class PhoneItem : MonoBehaviour
{
    public PhoneUI phoneUI;

    // Chamado ao clicar no botão do item celular
    public void OnPhoneButtonClicked()
    {
        // Marca a missão como completa
        if (MissionManager.Instance != null)
            MissionManager.Instance.CompleteMission("usePhone");

        // Mostra as opções do JSON associadas ao diálogo atual
        if (DialogueManager.Instance != null && DialogueManager.Instance.CurrentLine != null)
        {
            phoneUI.ShowPhoneOptions(DialogueManager.Instance.CurrentLine.options);
        }
        else
        {
            Debug.LogWarning("[PhoneItem] Não há diálogo atual para mostrar opções.");
        }
    }
}