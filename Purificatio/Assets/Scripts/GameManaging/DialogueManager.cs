using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class DialogueOption
{
    public string optionText;
    public string nextId;
}

[Serializable]
public class DialogueLine
{
    public string id;
    public string character;
    public string sprite;
    public string text;
    public string nextId;
    public string mission;
    public List<DialogueOption> options;
}

[Serializable]
public class DialogueData
{
    public List<DialogueLine> dialogue;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Configuração de diálogo")]
    public string dialogueFileName;
    public DialogueUIManager uiManager;
    
    [Header("Mission Handler da Fase")]
    public MissionHandlerBase missionHandler; // ← NOVO: Configurável por fase

    private DialogueData dialogueData;
    private Dictionary<string, DialogueLine> dialogueDict;
    private DialogueLine currentLine;
    private bool isPausedForMission = false;
    
    // NOVO: Flag para indicar que está aguardando input antes de mudar cena
    private bool waitingForSceneChange = false;
    private string sceneToLoad = "";

    private HashSet<string> goToMenuPoints = new HashSet<string>()
    {
        "pula_tutorial", "rude1", "calmo3"
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadDialogue();
        
        // MELHORADO: Usa Invoke para dar tempo do UI ativar completamente
        if (dialogueDict != null && dialogueDict.ContainsKey("inicio1"))
        {
            StartCoroutine(StartDialogueDelayed());
        }
        else
        {
            Debug.LogError("ID 'inicio1' não encontrado no JSON.");
        }
    }

    // NOVO: Inicia o diálogo com um pequeno delay
    private System.Collections.IEnumerator StartDialogueDelayed()
    {
        Debug.Log("[DialogueManager] StartDialogueDelayed iniciado.");
        
        // Garante que o painel está ativo
        if (uiManager != null)
        {
            uiManager.ShowDialogueHideHUD();
            Debug.Log("[DialogueManager] Painel de diálogo ativado.");
        }
        
        // Aguarda 1 frame para garantir que tudo está ativo
        yield return null;
        
        // Agora mostra a primeira linha
        if (dialogueDict.TryGetValue("inicio1", out currentLine))
        {
            Debug.Log($"[DialogueManager] Mostrando primeira linha: '{currentLine.text}'");
            ShowLine(currentLine);
        }
    }

    private void Update()
    {
        if (isPausedForMission || currentLine == null) return;

        // NOVO: Se está aguardando mudança de cena
        if (waitingForSceneChange)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"[DialogueManager] Jogador pressionou ESPAÇO. Carregando cena: {sceneToLoad}");
                SceneManager.LoadScene(sceneToLoad);
                return;
            }
            // Não processa mais nada enquanto aguarda
            return;
        }

        // Avança diálogo com Espaço, apenas se não houver opções
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // NOVO: Se o texto está sendo digitado, pula a animação primeiro
            if (uiManager.IsTextTyping())
            {
                uiManager.SkipTyping();
                return; // Não avança ainda, só pula a animação
            }

            // Se não tem opções, avança para o próximo diálogo
            if (currentLine.options == null || currentLine.options.Count == 0)
            {
                ShowNextLine();
            }
        }
    }

    private void LoadDialogue()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogues", dialogueFileName);
        if (!File.Exists(path))
        {
            Debug.LogError("Arquivo JSON não encontrado: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        dialogueData = JsonUtility.FromJson<DialogueData>(json);
        dialogueDict = new Dictionary<string, DialogueLine>();

        foreach (var line in dialogueData.dialogue)
        {
            if (!dialogueDict.ContainsKey(line.id))
                dialogueDict[line.id] = line;
            else
                Debug.LogWarning($"ID duplicado no JSON: {line.id}");
        }
    }

    private void ShowLine(DialogueLine line)
    {
        currentLine = line;

        // NOVO: Garante que o painel está ativo ANTES de mostrar texto
        uiManager.ShowDialogueHideHUD();

        // NOVO: Controla visibilidade do fantasma na cena
        HandleGhostVisibility(line.character);

        // Atualiza a UI do diálogo
        uiManager.UpdateDialogueUI(line);
        uiManager.ClearOptions();

        // Cria opções se houver
        if (line.options != null && line.options.Count > 0)
        {
            foreach (var option in line.options)
                uiManager.CreateOptionButton(option.optionText, () => OnOptionSelected(option.nextId));
        }

        // MODIFICADO: Se for ponto de menu especial, AGUARDA input
        if (goToMenuPoints.Contains(line.id))
        {
            Debug.Log($"[DialogueManager] Diálogo final detectado ('{line.id}'). Aguardando ESPAÇO para ir ao menu...");
            
            waitingForSceneChange = true;
            sceneToLoad = "02. Menu";
            
            // NOVO: Mostra indicador visual (opcional)
            uiManager.ShowContinuePrompt("Pressione ESPAÇO para continuar...");
            
            return; // Não continua o diálogo
        }

        // Se houver missão, pausa diálogo até completar
        if (!string.IsNullOrEmpty(line.mission))
        {
            StartMission(line.mission);
        }
        else
        {
            isPausedForMission = false;
            uiManager.ShowDialogueHideHUD();
        }
    }

    private void OnMissionCompletedHandler(string completedId)
    {
        if (MissionManager.Instance != null &&
            MissionManager.Instance.IsCompleted(completedId))
        {
            Debug.Log($"[DialogueManager] Missão {completedId} completada, retomando diálogo!");
            OnMissionComplete();
        }
    }

    private void StartMission(string missionId)
    {
        isPausedForMission = true;
        uiManager.HideDialogueShowHUD();

        if (MissionManager.Instance == null)
        {
            Debug.LogWarning("[DialogueManager] Nenhum MissionManager encontrado para missão: " + missionId);
            return;
        }

        // Se já foi completada, segue normalmente
        if (MissionManager.Instance.IsCompleted(missionId))
        {
            Debug.Log($"[DialogueManager] Missão {missionId} já completa. Continuando diálogo.");
            isPausedForMission = false;
            uiManager.ShowDialogueHideHUD();
            return;
        }

        Debug.Log($"[DialogueManager] Iniciando missão {missionId}");

        // Remove possíveis listeners duplicados
        MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;
        // Adiciona o listener novo
        MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;

        // Garante que a missão começa ativa
        MissionManager.Instance.StartMission(missionId);
    }

    private void OnMissionComplete()
    {
        isPausedForMission = false;
        uiManager.ShowDialogueHideHUD();
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (currentLine == null) return;

        if (!string.IsNullOrEmpty(currentLine.nextId) &&
            dialogueDict.TryGetValue(currentLine.nextId, out var nextLine))
        {
            ShowLine(nextLine);
        }
        else
        {
            uiManager.ShowEndText("Fim do diálogo.");
            currentLine = null;
        }
    }

    public void OnOptionSelected(string nextId)
    {
        if (dialogueDict.TryGetValue(nextId, out var nextLine))
            ShowLine(nextLine);
        else
            Debug.LogWarning("Próximo ID não encontrado: " + nextId);
    }

    public void ContinueDialogue()
    {
        if (isPausedForMission)
        {
            Debug.Log("[DialogueManager] ContinueDialogue chamado, mas aguardando missão.");
            return;
        }

        ShowNextLine();
    }

    public void GoToNode(string nodeId)
    {
        if (dialogueDict.TryGetValue(nodeId, out var line))
            ShowLine(line);
        else
            Debug.LogWarning("ID de diálogo não encontrado: " + nodeId);
    }

    /// <summary>
    /// NOVO: Controla a visibilidade do sprite do fantasma na cena
    /// </summary>
    private void HandleGhostVisibility(string characterName)
    {
        // Lista de personagens fantasmas (deve bater com DialogueUIManager)
        string[] ghostCharacters = { "Eveline", "Djinn", "Mazikkin" };
        
        bool isGhost = false;
        foreach (string ghost in ghostCharacters)
        {
            if (characterName.Equals(ghost, System.StringComparison.OrdinalIgnoreCase))
            {
                isGhost = true;
                break;
            }
        }

        // Se é fantasma, tenta encontrar e mostrar o sprite na cena
        if (isGhost)
        {
            // Procura o GameObject do fantasma na cena
            GameObject ghostObject = GameObject.Find(characterName);
            
            if (ghostObject != null)
            {
                GhostSpriteManager ghostManager = ghostObject.GetComponent<GhostSpriteManager>();
                
                if (ghostManager != null)
                {
                    ghostManager.Show();
                    Debug.Log($"[DialogueManager] Mostrando sprite de {characterName} na cena.");
                }
                else
                {
                    Debug.LogWarning($"[DialogueManager] {characterName} não tem GhostSpriteManager!");
                }
            }
            else
            {
                Debug.LogWarning($"[DialogueManager] GameObject '{characterName}' não encontrado na cena!");
            }
        }
        else
        {
            // Se não é fantasma, esconde todos os fantasmas
            HideAllGhosts();
        }
    }

    /// <summary>
    /// Esconde todos os fantasmas da cena
    /// </summary>
    private void HideAllGhosts()
    {
        GhostSpriteManager[] allGhosts = FindObjectsOfType<GhostSpriteManager>();
        
        foreach (var ghost in allGhosts)
        {
            ghost.Hide();
        }
    }

    public DialogueLine CurrentLine => currentLine;

    // NOVO: Cleanup ao destruir
    private void OnDestroy()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;
        }
    }
}