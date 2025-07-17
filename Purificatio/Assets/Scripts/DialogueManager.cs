using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Unity.VisualScripting;

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
    public TextMeshProUGUI CharNameText;
    public TextMeshProUGUI dialogueText;
    public Image CharSprite;
    public Sprite DefaultSprite;

    public Button optionButtonPrefab;
    public Transform optionsContainer;

    private DialogueData dialogueData;
    private Dictionary<string, DialogueLine> dialogueDict;
    private DialogueLine currentLine;

   void Start()
{
    LoadDialogue();

    if (dialogueDict != null && dialogueDict.TryGetValue("inicio1", out currentLine))
    {
        ShowLine(currentLine);
    }
    else
    {
        Debug.LogError("dialogueDict não foi inicializado ou ID inicio1 não encontrado.");
    }
}


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentLine != null && (currentLine.options == null || currentLine.options.Count == 0))
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
            Debug.LogError("Arquivo JSON não encontrado em: " + path);
        }
    }

    void ShowLine(DialogueLine line)
    {
        CharNameText.text = line.character;
        dialogueText.text = line.text;

        Sprite newSprite = Resources.Load<Sprite>(line.sprite);
        CharSprite.sprite = newSprite != null ? newSprite : DefaultSprite;

        ClearOptions();

        if (line.options != null && line.options.Count > 0)
        {
            foreach (var option in line.options)
            {
                Button btn = Instantiate(optionButtonPrefab, optionsContainer);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = option.optionText;


                string nextId = option.nextId;
                btn.onClick.AddListener(() => OnOptionSelected(nextId));
            }
        }

    }

    void ShowNextLine()
    {
        string nextId = CalcularProximoId(currentLine.id);
        if (nextId != null && dialogueDict.TryGetValue(nextId, out var nextLine))
        {
            currentLine = nextLine;
            ShowLine(currentLine);
        }
        else
        {
            dialogueText.text = "Fim do diálogo.";
            currentLine = null;
        }
    }

    void OnOptionSelected(string nextId)
    {
        if (dialogueDict.TryGetValue(nextId, out var nextLine))
        {
            currentLine = nextLine;
            ShowLine(currentLine);
        }
        else
        {
            Debug.LogWarning("Próximo ID não encontrado: " + nextId);
        }
    }

    void ClearOptions()
    {
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    string CalcularProximoId(string atualId)
    {
        for (int i = atualId.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(atualId[i]))
            {
                int j = i;
                while (j >= 0 && char.IsDigit(atualId[i])) j--;

                string prefixo = atualId.Substring(0, j + 1);
                string numeroStr = atualId.Substring(j + 1);
                if (int.TryParse(numeroStr, out int numero))
                {
                    string proximoId = prefixo + (numero + 1);
                    return proximoId;
                }
                break;
            }
        }
        return null;
    }

}