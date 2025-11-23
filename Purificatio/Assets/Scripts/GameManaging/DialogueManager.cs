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
    public string startDialogueId = "inicio1";
    public DialogueUIManager uiManager;
    
    [Header("Mission Handler da Fase")]
    public MissionHandlerBase missionHandler;

    private DialogueData dialogueData;
    private Dictionary<string, DialogueLine> dialogueDict;
    private DialogueLine currentLine;
    private bool isPausedForMission = false;
    
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
        
        if (dialogueDict != null && dialogueDict.ContainsKey(startDialogueId))
        {
            StartCoroutine(StartDialogueDelayed());
        }
        else
        {
            Debug.LogError($"ID '{startDialogueId}' não encontrado no JSON. Verifique o campo 'Start Dialogue Id' no Inspector.");
        }
    }
    
    private System.Collections.IEnumerator StartDialogueDelayed()
    {
        Debug.Log("[DialogueManager] StartDialogueDelayed iniciado.");
        
        if (uiManager != null)
        {
            uiManager.ShowDialogueHideHUD();
            Debug.Log("[DialogueManager] Painel de diálogo ativado.");
        }
        
        yield return null;
        
        if (dialogueDict.TryGetValue(startDialogueId, out currentLine))
        {
            Debug.Log($"[DialogueManager] Mostrando primeira linha: '{currentLine.text}'");
            ShowLine(currentLine);
        }
    }

    private void Update()
    {
        if (isPausedForMission || currentLine == null) return;
        
        // Aguardando mudança de cena
        if (waitingForSceneChange)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"[DialogueManager] Jogador pressionou ESPAÇO. Carregando cena: {sceneToLoad}");
                SceneManager.LoadScene(sceneToLoad);
                return;
            }
            return;
        }
        
        // Avança diálogo com Espaço
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Se texto está sendo digitado, pula a animação
            if (uiManager.IsTextTyping())
            {
                uiManager.SkipTyping();
                return;
            }
            
            // Se não tem opções, avança para próximo diálogo
            if (currentLine.options == null || currentLine.options.Count == 0)
            {
                ShowNextLine();
            }
        }
    }

    private void LoadDialogue()
    {
        string fileName = dialogueFileName;
        if (!fileName.EndsWith(".json"))
        {
            fileName += ".json";
        }

        string path = Path.Combine(Application.streamingAssetsPath, "Dialogues", fileName);
        
        if (!File.Exists(path))
        {
            Debug.LogError($"Arquivo JSON não encontrado: {path}\nVerifique se o arquivo existe e está na pasta StreamingAssets/Dialogues/");
            return;
        }

        try
        {
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

            Debug.Log($"[DialogueManager] JSON carregado com sucesso: {dialogueDict.Count} linhas de diálogo.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao carregar JSON: {e.Message}");
        }
    }

    private void ShowLine(DialogueLine line)
    {
        currentLine = line;
        
        // Mostra painel de diálogo
        uiManager.ShowDialogueHideHUD();
        
        // Controla visibilidade de fantasmas
        HandleGhostVisibility(line.character);
        
        // Atualiza UI
        uiManager.UpdateDialogueUI(line);
        uiManager.ClearOptions();
        
        // Cria botões de opções
        if (line.options != null && line.options.Count > 0)
        {
            foreach (var option in line.options)
                uiManager.CreateOptionButton(option.optionText, () => OnOptionSelected(option.nextId));
        }
        
        // Verifica se é ponto final (volta ao menu)
        if (goToMenuPoints.Contains(line.id))
        {
            Debug.Log($"[DialogueManager] Diálogo final detectado ('{line.id}'). Aguardando ESPAÇO para ir ao menu...");
            waitingForSceneChange = true;
            sceneToLoad = "02. Menu";
            uiManager.ShowContinuePrompt("Pressione ESPAÇO para continuar...");
            return;
        }
        
        // Se houver missão, pausa o diálogo
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

    private void StartMission(string missionId)
    {
        isPausedForMission = true;
        uiManager.HideDialogueShowHUD();

        if (MissionManager.Instance == null)
        {
            Debug.LogWarning("[DialogueManager] MissionManager não encontrado!");
            return;
        }

        Debug.Log($"[DialogueManager] Iniciando missão {missionId}");
        
        // Registra listener para quando a missão for completada
        MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedInternal;
        MissionManager.Instance.OnMissionCompleted += OnMissionCompletedInternal;
        
        // Inicia a missão
        MissionManager.Instance.StartMission(missionId);
        
        // Chama o handler específico da fase
        if (missionHandler != null)
        {
            missionHandler.HandleMission(missionId);
        }
    }

    private void OnMissionCompletedInternal(string completedId)
    {
        if (MissionManager.Instance != null &&
            MissionManager.Instance.IsCompleted(completedId))
        {
            Debug.Log($"[DialogueManager] Missão '{completedId}' completada, retomando diálogo!");
            OnMissionComplete();
        }
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
        // Se a opção não tem nextId, fecha o diálogo
        if (string.IsNullOrEmpty(nextId))
        {
            Debug.Log("[DialogueManager] Opção sem nextId - fechando diálogo");
            uiManager.HideDialogueShowHUD();
            currentLine = null;
            return;
        }
    
        // Vai para o próximo diálogo
        if (dialogueDict.TryGetValue(nextId, out var nextLine))
            ShowLine(nextLine);
        else
            Debug.LogWarning("Próximo ID não encontrado: " + nextId);
    }

    public void GoToNode(string nodeId)
    {
        if (dialogueDict.TryGetValue(nodeId, out var line))
            ShowLine(line);
        else
            Debug.LogWarning("ID de diálogo não encontrado: " + nodeId);
    }
    
    private void HandleGhostVisibility(string characterName)
    {
        string[] ghostCharacters = { "Eveline", "Djinn", "Mazzi" };
        
        bool isGhost = false;
        foreach (string ghost in ghostCharacters)
        {
            if (characterName.Equals(ghost, System.StringComparison.OrdinalIgnoreCase))
            {
                isGhost = true;
                break;
            }
        }
        
        if (isGhost)
        {
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
            HideAllGhosts();
        }
    }
    
    private void HideAllGhosts()
    {
        GhostSpriteManager[] allGhosts = FindObjectsOfType<GhostSpriteManager>();
        
        foreach (var ghost in allGhosts)
        {
            ghost.Hide();
        }
    }

    public DialogueLine CurrentLine => currentLine;

    private void OnDestroy()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedInternal;
        }
    }
}