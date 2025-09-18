using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
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
    public string mission;                    // <<=== novo campo
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

    private bool isPausedForMission = false;

    void Start()
    {
        LoadDialogue();
        if (dialogueDict != null && dialogueDict.TryGetValue("inicio1", out currentLine))
            ShowLine(currentLine);
        else
            Debug.LogError("ID inicio1 n�o encontrado.");
    }

    void Update()
    {
        // n�o avan�a se estiver pausado aguardando miss�o
        if (isPausedForMission) return;

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
                dialogueDict[line.id] = line;
        }
        else
        {
            Debug.LogError("Arquivo JSON n�o encontrado: " + path);
        }
    }

    void ShowLine(DialogueLine line)
    {
        currentLine = line;
        uiManager.UpdateDialogueUI(line);
        uiManager.ClearOptions();

        if (line.options != null && line.options.Count > 0)
        {
            foreach (var opt in line.options)
                uiManager.CreateOptionButton(opt.optionText, () => OnOptionSelected(opt.nextId));
        }

        // Se essa fala iniciar uma miss�o
        if (!string.IsNullOrEmpty(line.mission))
        {
            StartMissionPause(line.mission);
        }
    }

    void ShowNextLine()
    {
        if (currentLine == null) return;
        if (string.IsNullOrEmpty(currentLine.nextId))
        {
            uiManager.ShowEndText("Fim do di�logo.");
            currentLine = null;
            return;
        }

        if (dialogueDict.TryGetValue(currentLine.nextId, out var next))
            ShowLine(next);
        else
            Debug.LogWarning("Pr�ximo ID n�o encontrado: " + currentLine.nextId);
    }

    void OnOptionSelected(string nextId)
    {
        if (dialogueDict.TryGetValue(nextId, out var next))
            ShowLine(next);
        else
            Debug.LogWarning("Pr�ximo ID n�o encontrado: " + nextId);
    }

    // --- MISS�O ---
    void StartMissionPause(string missionName)
    {
        isPausedForMission = true;
        uiManager.HideDialogueShowHUD(); // fecha o HUD de di�logo
        MissionChecker.Instance.StartMission(missionName, OnMissionComplete);
    }

    void OnMissionComplete()
    {
        isPausedForMission = false;
        uiManager.ShowDialogueHideHUD(); // reabre HUD de di�logo
        ShowNextLine();                  // segue para o pr�ximo di�logo
    }
}
