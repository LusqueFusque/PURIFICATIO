using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente para o telefone que permite ligar para Timbu
/// Só funciona quando o diálogo está pausado em "rota_entrega9"
/// </summary>
public class PhoneClickable : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuração")]
    [Tooltip("ID do diálogo que este telefone desbloqueia")]
    public string requiredDialogueId = "rota_entrega9";
    
    [Header("Áudio")]
    [Tooltip("Som de discar telefone (opcional)")]
    public AudioClip phoneDialSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[PhoneClickable] Telefone clicado!");

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[PhoneClickable] DialogueManager.Instance é null!");
            return;
        }

        // Verifica se o diálogo atual é o correto (pausado em rota_entrega9)
        var currentLine = DialogueManager.Instance.CurrentLine;
        
        if (currentLine == null)
        {
            Debug.Log("[PhoneClickable] Nenhum diálogo ativo no momento.");
            return;
        }

        if (currentLine.id == requiredDialogueId)
        {
            Debug.Log($"[PhoneClickable] ✓ Diálogo correto ({requiredDialogueId})! Ligando para Timbu...");

            // Toca som do telefone
            if (phoneDialSound != null)
            {
                AudioSource.PlayClipAtPoint(phoneDialSound, Camera.main.transform.position, 0.6f);
            }

            // Despausa o diálogo
            DialogueManager.Instance.UnpauseDialogue();

            // Vai para o próximo diálogo (ligar_timbu1)
            if (!string.IsNullOrEmpty(currentLine.nextId))
            {
                DialogueManager.Instance.GoToNode(currentLine.nextId);
                Debug.Log($"[PhoneClickable] Indo para: {currentLine.nextId}");
            }
        }
        else
        {
            Debug.Log($"[PhoneClickable] Diálogo incorreto. Atual: {currentLine.id}, Necessário: {requiredDialogueId}");
        }
    }
}