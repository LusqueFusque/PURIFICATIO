using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    public GameObject panelDialogue;
    public GameObject panelHUD;
    public TextMeshProUGUI charNameText;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;
    public Transform optionsContainer;
    public Button optionButtonPrefab;
    public Sprite defaultSprite;

    [Header("Continue Prompt")]
    public GameObject continuePrompt;

    // Componente de efeito typewriter
    private TypewriterEffect typewriterEffect;

    void Awake()
    {
        // IMPORTANTE: Obtém referência ao TypewriterEffect
        if (dialogueText != null)
        {
            typewriterEffect = dialogueText.GetComponent<TypewriterEffect>();

            if (typewriterEffect == null)
            {
                Debug.LogWarning("[DialogueUIManager] TypewriterEffect não encontrado no dialogueText. Adicionando automaticamente...");
                typewriterEffect = dialogueText.gameObject.AddComponent<TypewriterEffect>();
            }
        }
        else
        {
            Debug.LogError("[DialogueUIManager] dialogueText não está atribuído!");
        }
    }

    void Start()
    {
        // NOVO: Garante que o texto está limpo ao iniciar
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    public void UpdateDialogueUI(DialogueLine line)
    {
        charNameText.text = line.character;

        // Usa o efeito typewriter
        if (typewriterEffect != null)
        {
            typewriterEffect.ShowText(line.text);
        }
        else
        {
            // Fallback se não houver typewriter
            dialogueText.text = line.text;
            Debug.LogWarning("[DialogueUIManager] TypewriterEffect não disponível. Mostrando texto instantaneamente.");
        }

        ShowImage(line.sprite);
        HideContinuePrompt();
    }

    public void CreateOptionButton(string text, UnityEngine.Events.UnityAction action)
    {
        Button btn = Instantiate(optionButtonPrefab, optionsContainer);
        btn.GetComponentInChildren<TextMeshProUGUI>().text = text;
        btn.onClick.AddListener(action);
    }

    public void ClearOptions()
    {
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowEndText(string msg)
    {
        if (typewriterEffect != null)
        {
            typewriterEffect.ShowText(msg);
        }
        else
        {
            dialogueText.text = msg;
        }
    }

    public void HideDialogueShowHUD()
    {
        panelDialogue.SetActive(false);
        panelHUD.SetActive(true);
        HideContinuePrompt();
    }

    public void ShowDialogueHideHUD()
    {
        panelDialogue.SetActive(true);
        panelHUD.SetActive(false);
    }

    private void ShowImage(string spriteName)
    {
        Sprite s = Resources.Load<Sprite>(spriteName);
        characterImage.sprite = s != null ? s : defaultSprite;
        characterImage.gameObject.SetActive(true);
    }

    public void ShowContinuePrompt(string message = "Pressione ESPAÇO para continuar...")
    {
        if (continuePrompt != null)
        {
            continuePrompt.SetActive(true);

            var tmp = continuePrompt.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = message;

            var text = continuePrompt.GetComponent<Text>();
            if (text != null)
                text.text = message;
        }
        else
        {
            // Fallback: aguarda typewriter terminar
            if (typewriterEffect != null && typewriterEffect.IsTyping())
            {
                StartCoroutine(WaitForTypingThenShowPrompt(message));
            }
            else
            {
                dialogueText.text += $"\n\n<color=yellow>{message}</color>";
            }
        }
    }

    private System.Collections.IEnumerator WaitForTypingThenShowPrompt(string message)
    {
        while (typewriterEffect != null && typewriterEffect.IsTyping())
        {
            yield return null;
        }

        dialogueText.text += $"\n\n<color=yellow>{message}</color>";
    }

    public void HideContinuePrompt()
    {
        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }
    }

    public bool IsTextTyping()
    {
        return typewriterEffect != null && typewriterEffect.IsTyping();
    }

    public void SkipTyping()
    {
        if (typewriterEffect != null)
        {
            typewriterEffect.SkipTyping();
        }
    }
}