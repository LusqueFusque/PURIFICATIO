using UnityEngine;

public class PhoneItem : MonoBehaviour
{
    [Header("Áudio do Celular")]
    public AudioClip phoneDefaultSound;   // som padrão
    public AudioClip phoneSpecialSound;   // som especial (para certas salas)

    [Header("Configuração")]
    public string[] specialRooms; // nomes exatos das salas
    [Tooltip("Fonte de áudio 2D para tocar os sons do celular")]
    public AudioSource audioSource2D; // arraste um AudioSource aqui (spatialBlend = 0)

    void Awake()
    {
        // Garante um AudioSource 2D
        if (audioSource2D == null)
        {
            audioSource2D = gameObject.AddComponent<AudioSource>();
            audioSource2D.playOnAwake = false;
            audioSource2D.spatialBlend = 0f; // 2D
            audioSource2D.volume = 1f;
        }
    }

    public void OnPhoneButtonClicked()
    {
        var dialogueManager = DialogueManager.Instance;
        var missionManager = MissionManager.Instance;

        if (missionManager != null)
            missionManager.CompleteMission("usePhone");

        PlayPhoneSound();

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

        dialogueManager.uiManager.ClearOptions();
        foreach (var option in currentLine.options)
        {
            dialogueManager.uiManager.CreateOptionButton(option.optionText, () =>
            {
                dialogueManager.OnOptionSelected(option.nextId);
            });
        }
    }

    private void PlayPhoneSound()
    {
        string currentRoom = "";
        if (AdvancedMapManager.Instance != null)
            currentRoom = AdvancedMapManager.Instance.GetCurrentRoomName();

        bool isSpecialRoom = false;
        if (!string.IsNullOrEmpty(currentRoom) && specialRooms != null)
        {
            foreach (var room in specialRooms)
            {
                if (!string.IsNullOrEmpty(room) && room == currentRoom)
                {
                    isSpecialRoom = true;
                    break;
                }
            }
        }

        if (isSpecialRoom && phoneSpecialSound != null)
        {
            audioSource2D.PlayOneShot(phoneSpecialSound, 0.9f);
            Debug.Log($"[PhoneItem] 🔊 Som especial tocado na sala '{currentRoom}'");
        }
        else if (phoneDefaultSound != null)
        {
            audioSource2D.PlayOneShot(phoneDefaultSound, 0.9f);
            Debug.Log("[PhoneItem] 🔊 Som padrão do celular tocado");
        }
        else
        {
            Debug.LogWarning("[PhoneItem] Nenhum áudio atribuído.");
        }
    }
}
