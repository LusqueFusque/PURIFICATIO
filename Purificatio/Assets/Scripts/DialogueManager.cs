using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System;

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
    public List<DialogueOption> options;
    public string mission; // Novo campo para miss�es
}

[System.Serializable]
public class DialogueData
{
    public List<DialogueLine> dialogue;
}

public class DialogueManager : MonoBehaviour
{
    public string dialogueFileName;
    public DialogueUIManager uiManager; // Link para a UI
    private DialogueData dialogueData;
    private Dictionary<string, DialogueLine> dialogueDict;
    private DialogueLine currentLine;

    private bool waitingMission = false;

    private HashSet<string> goToMenuPoints = new HashSet<string>()
    {
        "pula_tutorial", "rude1", "calmo3"
    };

    void Start()
    {
        LoadDialogue();

        if (dialogueDict != null && dialogueDict.TryGetValue("inicio1", out currentLine))
        {
            ShowLine(currentLine);
        }
        else
        {
            Debug.LogError("dialogueDict n�o inicializado ou ID inicio1 n�o encontrado.");
        }
    }

    void Update()
    {
        if (waitingMission) return; // trava o di�logo at� miss�o ser completada

        if (Input.GetKeyDown(KeyCode.Space) && currentLine != null)
        {
            if (currentLine.options == null || currentLine.options.Count == 0)
            {
                ShowNextLine();
            }
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
            Debug.LogError("Arquivo JSON n�o encontrado: " + path);
        }
    }

    void ShowLine(DialogueLine line)
    {
        currentLine = line;

        // Atualiza UI
        uiManager.UpdateDialogueUI(line);

        // Trata di�logos especiais de menu
        if (goToMenuPoints.Contains(line.id))
        {
            SceneManager.LoadScene("02. Menu");
            return;
        }

        // Se tiver miss�o, trava o di�logo
        if (!string.IsNullOrEmpty(line.mission))
        {
            waitingMission = true;
            MissionChecker.Instance.StartMission(line.mission, OnMissionComplete);
        }
        else
        {
            waitingMission = false;
        }

        // Cria op��es na UI se houver
        uiManager.ClearOptions();
        if (line.options != null && line.options.Count > 0)
        {
            foreach (var option in line.options)
            {
                uiManager.CreateOptionButton(option.optionText, () => OnOptionSelected(option.nextId));
            }
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
            uiManager.ShowEndText("Fim do di�logo.");
            currentLine = null;
        }
    }

    void OnOptionSelected(string nextId)
    {
        if (dialogueDict.TryGetValue(nextId, out var nextLine))
        {
            ShowLine(nextLine);
        }
        else
        {
            Debug.LogWarning("Pr�ximo ID n�o encontrado: " + nextId);
        }
    }

    void OnMissionComplete()
    {
        waitingMission = false;

        if (!string.IsNullOrEmpty(currentLine.nextId))
            ShowNextLine();
        else
            uiManager.ShowEndText("Fim do di�logo.");
    }

    // Chamado por UI ou por triggers externos para continuar di�logos travados
    public void ContinueDialogue()
    {
        if (!waitingMission)
            ShowNextLine();
    }
}
