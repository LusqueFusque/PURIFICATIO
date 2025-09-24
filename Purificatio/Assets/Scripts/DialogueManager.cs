using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    public List<DialogueOption> options;
    public string mission; // campo novo no JSON
}

[Serializable]
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
        // não avança se estiver pausado aguardando missão
        if (isPaused) return;

        if (Input.GetKeyDown(KeyCode.Space) && currentLine != null)
        {
            // se não tem opções, avança; se tem, deixa os botões resolverem
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

        // Remove opções antigas e cria as novas
        uiManager.ClearOptions();
        if (line.options != null && line.options.Count > 0)
        {
            foreach (var opt in line.options)
                uiManager.CreateOptionButton(opt.optionText, () => OnOptionSelected(opt.nextId));
        }

        // atalhos de menu
        if (goToMenuPoints.Contains(line.id))
        {
            SceneManager.LoadScene("02. Menu");
            return;
        }

        // Se houver missão associada, pausa e escuta a conclusão
        if (!string.IsNullOrEmpty(line.mission))
        {
            PauseForMission(line.mission);
        }
        else
        {
            // linha normal: mostra diálogo e permite avançar com espaço
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

    // -------- MISSÃO / PAUSA ----------
    void PauseForMission(string missionId)
    {
        // Se MissionManager (o novo com eventos) existe — usa ele (melhor opção)
        if (MissionManager.Instance != null)
        {
            // Se já foi completada antes, não pausar
            if (MissionManager.Instance.IsCompleted(missionId))
            {
                Debug.Log($"[DialogueManager] Missão '{missionId}' já completada anteriormente -> não pausar.");
                isPaused = false;
                uiManager.ShowDialogueHideHUD();
                return;
            }

            isPaused = true;
            uiManager.HideDialogueShowHUD();

            // Handler local que se remove após disparar
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

            Debug.Log($"[DialogueManager] Missão iniciada (MissionManager): {missionId}");
            return;
        }

        // Fallback: se existir o MissionChecker antigo (callback style)
        if (MissionChecker.Instance != null)
        {
            isPaused = true;
            uiManager.HideDialogueShowHUD();

            MissionChecker.Instance.StartMission(missionId, () =>
            {
                ResumeDialogue();
            });

            Debug.Log($"[DialogueManager] Missão iniciada (MissionChecker): {missionId}");
            return;
        }

        // Se não tem manager nenhum, continua normalmente (debug)
        Debug.LogWarning("[DialogueManager] Nenhum MissionManager ou MissionChecker encontrado; não foi possível pausar por missão.");
        isPaused = false;
        uiManager.ShowDialogueHideHUD();
    }

    // Chamado quando a missão é completada (por evento ou callback)
    void ResumeDialogue()
    {
        isPaused = false;
        uiManager.ShowDialogueHideHUD();
        ShowNextLine();
    }

    // Mantive ContinueDialogue pública para compatibilidade com UI buttons
    public void ContinueDialogue()
    {
        if (isPaused)
        {
            Debug.Log("[DialogueManager] ContinueDialogue chamado, mas estamos pausados aguardando missão.");
            return;
        }

        uiManager.ShowDialogueHideHUD();
        ShowNextLine();
    }
}
