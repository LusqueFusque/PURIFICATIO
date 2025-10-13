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
    public string mission; // Se houver missão associada
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

    private DialogueData dialogueData;
    private Dictionary<string, DialogueLine> dialogueDict;
    private DialogueLine currentLine;

    private bool isPausedForMission = false;

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

        if (dialogueDict != null && dialogueDict.TryGetValue("inicio1", out currentLine))
            ShowLine(currentLine);
        else
            Debug.LogError("ID 'inicio1' não encontrado no JSON.");
    }

    private void Update()
    {
        if (isPausedForMission || currentLine == null) return;

        // Avança diálogo com Espaço, apenas se não houver opções
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentLine.options == null || currentLine.options.Count == 0)
                ShowNextLine();
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

        // Atualiza a UI do diálogo
        uiManager.UpdateDialogueUI(line);
        uiManager.ClearOptions();

        // Cria opções se houver
        if (line.options != null && line.options.Count > 0)
        {
            foreach (var option in line.options)
                uiManager.CreateOptionButton(option.optionText, () => OnOptionSelected(option.nextId));
        }

        // Se linha for menu especial, muda de cena
        if (goToMenuPoints.Contains(line.id))
        {
            SceneManager.LoadScene("02. Menu");
            return;
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
        // Evita nulls e continua o diálogo se for a missão certa
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

        if (!string.IsNullOrEmpty(currentLine.nextId) && dialogueDict.TryGetValue(currentLine.nextId, out var nextLine))
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

    // Chamado externamente (UI buttons, triggers, PhoneItem, etc.)
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
    
    public DialogueLine CurrentLine => currentLine;

}
