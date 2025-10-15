using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    public GameObject panelDialogue;
    public GameObject panelHUD;

    public TextMeshProUGUI charNameText;
    public TextMeshProUGUI dialogueText;

    public Image characterImage; // só um espaço de imagem agora

    public Transform optionsContainer;
    public Button optionButtonPrefab;

    public Sprite defaultSprite;

    public void UpdateDialogueUI(DialogueLine line)
    {
        charNameText.text = line.character;
        dialogueText.text = line.text;

        ShowImage(line.sprite);
    }

    public void CreateOptionButton(string text, UnityEngine.Events.UnityAction action)
    {
        Button btn = Instantiate(optionButtonPrefab, optionsContainer);
        btn.GetComponentInChildren<TextMeshProUGUI>().text = text;
        btn.onClick.AddListener(action);
    }

    public void ClearOptions()
    {
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowEndText(string msg)
    {
        dialogueText.text = msg;
    }

    public void HideDialogueShowHUD()
    {
        panelDialogue.SetActive(false);
        panelHUD.SetActive(true);
    }

    public void ShowDialogueHideHUD()
    {
        panelDialogue.SetActive(true);
        panelHUD.SetActive(false);
    }

    private void ShowImage(string spriteName)
    {
        Sprite s = Resources.Load<Sprite>(spriteName);
        characterImage.sprite = s != null ? s : defaultSprite;
        characterImage.gameObject.SetActive(true);
    }
}
