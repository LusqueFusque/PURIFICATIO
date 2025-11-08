using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    [Header("UI Diálogo")]
    public GameObject panelDialogue;
    public TextMeshProUGUI charNameText;
    public TextMeshProUGUI dialogueText;
    public Image characterImage; // Sprite do personagem OU seta para fantasma
    public Image dialogueBackground; // Background da caixa de diálogo
    public Image gradientOverlay; // Degradê no fundo da tela (opcional)
    
    [Header("Sprites Indicadores")]
    public Sprite ghostArrowSprite; // Seta apontando para o cenário (UI Sprite)
    
    [Header("Cores")]
    public Color humanDialogueColor = new Color(1f, 1f, 1f, 1f); // Branco #FFFFFF
    public Color ghostDialogueColor = new Color(0.604f, 0.173f, 0.149f, 1f); // Vermelho #9A2C26
    
    [Header("Shared UI")]
    public GameObject panelHUD;
    public Transform optionsContainer;
    public Button optionButtonPrefab;
    public Sprite defaultSprite;
    
    [Header("Continue Prompt")]
    public GameObject continuePrompt;

    [Header("Lista de Personagens Fantasmas")]
    public string[] ghostCharacters = { "Eveline", "Djinn", "Mazikkin" };

    private TypewriterEffect typewriterEffect;

    void Awake()
    {
        if (dialogueText != null)
        {
            typewriterEffect = dialogueText.GetComponent<TypewriterEffect>();
            if (typewriterEffect == null)
                typewriterEffect = dialogueText.gameObject.AddComponent<TypewriterEffect>();
        }
    }

    void Start()
    {
        if (panelDialogue != null)
            panelDialogue.SetActive(false);
        
        if (dialogueText != null)
            dialogueText.text = "";
    }

    public void UpdateDialogueUI(DialogueLine line)
    {
        // Ativa painel
        if (panelDialogue != null)
            panelDialogue.SetActive(true);

        // Verifica se é fantasma
        bool isGhost = IsGhostCharacter(line.character);

        // Atualiza nome
        charNameText.text = line.character;

        // Atualiza texto com typewriter
        if (typewriterEffect != null)
            typewriterEffect.ShowText(line.text);
        else
            dialogueText.text = line.text;

        // NOVO: Muda visual baseado em fantasma ou humano
        if (isGhost)
        {
            SetGhostVisuals();
        }
        else
        {
            SetHumanVisuals(line.sprite);
        }
        
        HideContinuePrompt();
    }

    private bool IsGhostCharacter(string characterName)
    {
        foreach (string ghost in ghostCharacters)
        {
            if (characterName.Equals(ghost, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Configura visuais para diálogo de FANTASMA
    /// </summary>
    private void SetGhostVisuals()
    {
        // 1. Troca sprite para SETA
        if (characterImage != null && ghostArrowSprite != null)
        {
            characterImage.sprite = ghostArrowSprite;
            characterImage.gameObject.SetActive(true);
        }

        // 2. Muda COR do background da caixa
        if (dialogueBackground != null)
        {
            dialogueBackground.color = ghostDialogueColor;
        }

        // 3. Muda COR do degradê de fundo (se existir)
        if (gradientOverlay != null)
        {
            Color ghostGradient = ghostDialogueColor;
            ghostGradient.a = 0.3f; // Mantém transparência do degradê
            gradientOverlay.color = ghostGradient;
            gradientOverlay.gameObject.SetActive(true);
        }

        Debug.Log("[DialogueUIManager] Visuais de FANTASMA aplicados.");
    }

    /// <summary>
    /// Configura visuais para diálogo de HUMANO
    /// </summary>
    private void SetHumanVisuals(string spriteName)
    {
        // 1. Carrega sprite do personagem humano
        if (characterImage != null)
        {
            Sprite s = Resources.Load<Sprite>(spriteName);
            characterImage.sprite = s != null ? s : defaultSprite;
            characterImage.gameObject.SetActive(true);
        }

        // 2. Volta COR BRANCA no background
        if (dialogueBackground != null)
        {
            dialogueBackground.color = humanDialogueColor;
        }

        // 3. Volta COR BRANCA no degradê (ou desativa)
        if (gradientOverlay != null)
        {
            Color humanGradient = humanDialogueColor;
            humanGradient.a = 0.3f; // Mantém transparência
            gradientOverlay.color = humanGradient;
            // Pode desativar o degradê se preferir: gradientOverlay.gameObject.SetActive(false);
        }

        Debug.Log($"[DialogueUIManager] Visuais de HUMANO aplicados (sprite: {spriteName}).");
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
            typewriterEffect.ShowText(msg);
        else
            dialogueText.text = msg;
    }

    public void HideDialogueShowHUD()
    {
        if (panelDialogue != null)
            panelDialogue.SetActive(false);
        
        if (panelHUD != null)
            panelHUD.SetActive(true);
        
        HideContinuePrompt();
    }

    public void ShowDialogueHideHUD()
    {
        if (panelDialogue != null)
            panelDialogue.SetActive(true);
        
        if (panelHUD != null)
            panelHUD.SetActive(false);
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
            if (typewriterEffect != null && typewriterEffect.IsTyping())
                StartCoroutine(WaitForTypingThenShowPrompt(message));
            else
                dialogueText.text += $"\n\n<color=yellow>{message}</color>";
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