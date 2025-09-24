using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public string nextId;
}

[System.Serializable]
public class DialogueLine
{
    public string id;
    public string character;
    public string sprite;
    public string text;
    public string nextId;
    public string mission; // Opcional, se houver missão associada
    public List<DialogueOption> options;
}

[System.Serializable]
public class DialogueData
{
    public List<DialogueLine> dialogue;
}

public class DialogueManager : MonoBehaviour
{
    public string dialogueFileName;
    public DialogueUIManager uiManager;

    private DialogueData dialogueData;
    private Dictionary<string, DialogueLine> dialogueDict;
    private DialogueLine currentLine;

    public DialogueLine CurrentLine => currentLine; // Exposto para PhoneItem

    private bool isPaused = false;

    private HashSet<string> goToMenuPoints = new HashSet<string>()
    {
        "pula_tutorial", "rude1", "calmo3"
    };

    void Start()
    {
        LoadDialogue();

        if (dialogueDict != null && dialogueDict.TryGetValue("inicio1", out currentLine))
            ShowLine(currentLine);
        else
            Debug.LogError("dialogueDict não inicializado ou ID inicio1 não encontrado.");
    }

    void Update()
    {
        if (isPaused) return;

        if (Input.GetKeyDown(KeyCode.Space) && currentLine != null)
        {
            if (currentLine.options == null || currentLine.options.Count == 0)
                ShowNextLine();
        }
    }

    void LoadDialogue()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogues", dialogueFileName);
        if (File.Exists(path))
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
        }
        else
        {
            Debug.LogError("Arquivo JSON não encontrado: " + path);
        }
    }

    void ShowLine(DialogueLine line)
    {
        currentLine = line;

        // Atualiza UI
        uiManager.UpdateDialogueUI(line);

        // Remove opções antigas e cria novas
        uiManager.ClearOptions();
        if (line.options != null && line.options.Count > 0)
        {
            foreach (var opt in line.options)
            {
                var localOpt = opt;
                uiManager.CreateOptionButton(localOpt.optionText, () => OnOptionSelected(localOpt.nextId));
            }
        }

        // atalhos de menu
        if (goToMenuPoints.Contains(line.id))
        {
            SceneManager.LoadScene("02. Menu");
            return;
        }

        // Se houver missão, pausa e escuta a conclusão
        if (!string.IsNullOrEmpty(line.mission))
        {
            PauseForMission(line.mission);
        }
        else
        {
            isPaused = false;
            uiManager.ShowDialogueHideHUD();
        }
    }

    void ShowNextLine()
    {
        if (currentLine == null) return;

        string nextId = currentLine.nextId;

        if (!string.IsNullOrEmpty(nextId) && dialogueDict.TryGetValue(nextId, out var nextLine))
        {
            ShowLine(nextLine);
        }
        else
        {
            uiManager.ShowEndText("Fim do diálogo.");
            currentLine = null;
        }
    }

    void OnOptionSelected(string nextId)
    {
        if (dialogueDict.TryGetValue(nextId, out var nextLine))
            ShowLine(nextLine);
        else
            Debug.LogWarning("Próximo ID não encontrado: " + nextId);
    }

    void PauseForMission(string missionId)
    {
        if (MissionManager.Instance != null)
        {
            if (MissionManager.Instance.IsCompleted(missionId))
            {
                isPaused = false;
                uiManager.ShowDialogueHideHUD();
                return;
            }

            isPaused = true;
            uiManager.HideDialogueShowHUD();

            Action<string> onComplete = null;
            onComplete = (completedId) =>
            {
                if (completedId == missionId)
                {
                    MissionManager.Instance.OnMissionCompleted -= onComplete;
                    ResumeDialogue();
                }
            };

            MissionManager.Instance.OnMissionCompleted += onComplete;
            MissionManager.Instance.StartMission(missionId);
            return;
        }

        if (MissionChecker.Instance != null)
        {
            isPaused = true;
            uiManager.HideDialogueShowHUD();
            MissionChecker.Instance.StartMission(missionId, () => { ResumeDialogue(); });
            return;
        }

        isPaused = false;
        uiManager.ShowDialogueHideHUD();
    }

    void ResumeDialogue()
    {
        isPaused = false;
        uiManager.ShowDialogueHideHUD();
        ShowNextLine();
    }

    public void ContinueDialogue()
    {
        if (isPaused)
            return;

        uiManager.ShowDialogueHideHUD();
        ShowNextLine();
    }

    public void GoToNode(string nodeId)
    {
        if (dialogueDict.TryGetValue(nodeId, out var line))
            ShowLine(line);
        else
            Debug.LogWarning("ID de diálogo não encontrado: " + nodeId);
    }

    public static DialogueManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowDialogueById(string id)
    {
        if (dialogueDict.TryGetValue(id, out var line))
            ShowLine(line);
        else
            Debug.LogWarning("ID de diálogo não encontrado: " + id);
    }
}
