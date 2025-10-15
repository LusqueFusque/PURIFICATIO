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

    // NOVO: Componente de efeito typewriter
    private TypewriterEffect typewriterEffect;

    void Awake()
    {
        // NOVO: Obtém ou adiciona o componente TypewriterEffect
        if (dialogueText != null)
        {
            typewriterEffect = dialogueText.GetComponent<TypewriterEffect>();
            
            if (typewriterEffect == null)
            {
                typewriterEffect = dialogueText.gameObject.AddComponent<TypewriterEffect>();
                Debug.Log("[DialogueUIManager] TypewriterEffect adicionado automaticamente.");
            }
        }
    }

    public void UpdateDialogueUI(DialogueLine line)
    {
        charNameText.text = line.character;
        
        // MODIFICADO: Usa o efeito typewriter ao invés de atribuir diretamente
        if (typewriterEffect != null)
        {
            typewriterEffect.ShowText(line.text);
        }
        else
        {
            // Fallback se não houver typewriter
            dialogueText.text = line.text;
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

    /// <summary>
    /// Mostra o prompt "Pressione ESPAÇO para continuar..."
    /// </summary>
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
            // Fallback: adiciona ao texto de diálogo
            // Mas espera o typewriter terminar
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

    /// <summary>
    /// Aguarda o typewriter terminar antes de mostrar o prompt
    /// </summary>
    private System.Collections.IEnumerator WaitForTypingThenShowPrompt(string message)
    {
        while (typewriterEffect != null && typewriterEffect.IsTyping())
        {
            yield return null;
        }
        
        dialogueText.text += $"\n\n<color=yellow>{message}</color>";
    }

    /// <summary>
    /// Esconde o prompt de continuar
    /// </summary>
    public void HideContinuePrompt()
    {
        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }
    }

    /// <summary>
    /// NOVO: Verifica se o texto está sendo digitado
    /// Útil para o DialogueManager saber se deve aguardar
    /// </summary>
    public bool IsTextTyping()
    {
        return typewriterEffect != null && typewriterEffect.IsTyping();
    }

    /// <summary>
    /// NOVO: Pula a animação de digitação
    /// </summary>
    public void SkipTyping()
    {
        if (typewriterEffect != null)
        {
            typewriterEffect.SkipTyping();
        }
    }
}