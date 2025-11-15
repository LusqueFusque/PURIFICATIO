using UnityEngine;

/// <summary>
/// Controla o telefone na Fase 2.
/// Segue o padrão do PhoneItem existente, mas com lógica específica para as escolhas da Fase 2.
/// </summary>
public class Fase2PhoneItem : MonoBehaviour
{
    public static Fase2PhoneItem Instance;

    [Header("Configuração")]
    [Tooltip("Se true, mostra as opções automaticamente quando o nó de escolha é atingido")]
    public bool autoShowOptionsOnChoiceNode = true;

    private bool optionsShown = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[Fase2PhoneItem] Destruindo duplicata.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("[Fase2PhoneItem] Instance configurado.");
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Chamado quando o jogador clica no botão do telefone.
    /// Mostra as opções de diálogo se houver.
    /// </summary>
    public void OnPhoneButtonClicked()
    {
        Debug.Log("[Fase2PhoneItem] ========================================");
        Debug.Log("[Fase2PhoneItem] Botão do telefone clicado!");

        var dialogueManager = DialogueManager.Instance;
        
        if (dialogueManager == null || dialogueManager.CurrentLine == null)
        {
            Debug.LogWarning("[Fase2PhoneItem] Nenhum diálogo ativo para o telefone.");
            Debug.Log("[Fase2PhoneItem] ========================================");
            return;
        }

        var currentLine = dialogueManager.CurrentLine;
        Debug.Log($"[Fase2PhoneItem] Diálogo atual: {currentLine.id}");

        if (currentLine.options == null || currentLine.options.Count == 0)
        {
            Debug.LogWarning("[Fase2PhoneItem] O diálogo atual não possui opções de telefone.");
            Debug.Log("[Fase2PhoneItem] ========================================");
            return;
        }

        // Mostra as opções usando o DialogueUIManager
        ShowPhoneOptions(currentLine);
    }

    /// <summary>
    /// Mostra as opções do telefone na UI.
    /// </summary>
    private void ShowPhoneOptions(DialogueLine line)
    {
        if (optionsShown)
        {
            Debug.Log("[Fase2PhoneItem] Opções já foram mostradas.");
            return;
        }

        optionsShown = true;

        var dialogueManager = DialogueManager.Instance;
        
        Debug.Log($"[Fase2PhoneItem] Mostrando {line.options.Count} opções:");
        
        dialogueManager.uiManager.ClearOptions(); // Limpa opções antigas

        foreach (var option in line.options)
        {
            Debug.Log($"[Fase2PhoneItem]  - {option.optionText} → {option.nextId}");
            
            dialogueManager.uiManager.CreateOptionButton(option.optionText, () =>
            {
                Debug.Log($"[Fase2PhoneItem] Opção selecionada: {option.optionText}");
                optionsShown = false; // Reset para próxima escolha
                dialogueManager.OnOptionSelected(option.nextId);
            });
        }

        Debug.Log("[Fase2PhoneItem] ✓ Opções criadas com sucesso!");
        Debug.Log("[Fase2PhoneItem] ========================================");
    }

    /// <summary>
    /// Verifica se o diálogo atual é um dos nós de escolha da Fase 2.
    /// </summary>
    public bool IsChoiceNode(string nodeId)
    {
        // Nós onde o telefone mostra opções
        return nodeId == "nambulampada2" || 
               nodeId == "escolha_djinn" || 
               nodeId == "escolha_timbu" || 
               nodeId == "escolha_carta";
    }

    /// <summary>
    /// Reset para permitir mostrar opções novamente.
    /// </summary>
    public void ResetOptionsShown()
    {
        optionsShown = false;
        Debug.Log("[Fase2PhoneItem] Estado resetado - opções podem ser mostradas novamente.");
    }
}