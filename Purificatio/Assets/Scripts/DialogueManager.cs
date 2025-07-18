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
    public DialogueUIManager uiManager; // Novo: link para UI Manager

    private DialogueData dialogueData;
    private Dictionary<string, DialogueLine> dialogueDict;
    private DialogueLine currentLine;

    private bool isPaused = false;

    private HashSet<string> pausePoints = new HashSet<string>()
    {
        "tutorial9", "tutorial13", "tutorial23", "tutorial25", "tutorial31", "tutorial32"
    };

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
            Debug.LogError("dialogueDict não inicializado ou ID inicio1 não encontrado.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isPaused && currentLine != null && (currentLine.options == null || currentLine.options.Count == 0))
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
                dialogueDict[line.id] = line;
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

        HandleSpecialDialogue(line.id);

        // Se tiver opções, cria na UI
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
            uiManager.ShowEndText("Fim do diálogo.");
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
            Debug.LogWarning("Próximo ID não encontrado: " + nextId);
        }
    }

    void HandleSpecialDialogue(string id)
    {
        if (goToMenuPoints.Contains(id))
        {
            SceneManager.LoadScene("02. Menu");
            return;
        }

        if (pausePoints.Contains(id))
        {
            isPaused = true;
            uiManager.HideDialoguePanelShowHUD();
        }
        else
        {
            isPaused = false;
            uiManager.ShowDialoguePanelHideHUD();
        }
    }

    public void ContinueDialogue() // Chama do HUD quando quer continuar
    {
        isPaused = false;
        ShowNextLine();
    }
}
