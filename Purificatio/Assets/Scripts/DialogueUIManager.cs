using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    public GameObject panelDialogue;    // Painel da caixa de diálogo
    public GameObject panelHUD;         // HUD (inventário, mapa etc.)

    public TextMeshProUGUI charNameText;
    public TextMeshProUGUI dialogueText;

    public Image centerCharacterImage;
    public Image sideCharacterImage;

    public Transform optionsContainer;
    public Button optionButtonPrefab;

    public Sprite defaultSprite;

    public void UpdateDialogueUI(DialogueLine line)
    {
        charNameText.text = line.character;
        dialogueText.text = line.text;

        // Decide se é centro ou lado
        if (line.character == "Timbu") // Timbu sempre no centro
        {
            ShowCenterImage(line.sprite);
            HideSideImage();
        }
        else
        {
            ShowSideImage(line.sprite);
            HideCenterImage();
        }
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

    public void ShowEndText(string message)
    {
        dialogueText.text = message;
    }

    public void HideDialoguePanelShowHUD()
    {
        panelDialogue.SetActive(false);
        panelHUD.SetActive(true);
    }

    public void ShowDialoguePanelHideHUD()
    {
        panelDialogue.SetActive(true);
        panelHUD.SetActive(false);
    }

    void ShowCenterImage(string spriteName)
    {
        Sprite s = Resources.Load<Sprite>(spriteName);
        centerCharacterImage.sprite = s != null ? s : defaultSprite;
        centerCharacterImage.gameObject.SetActive(true);
    }

    void ShowSideImage(string spriteName)
    {
        Sprite s = Resources.Load<Sprite>(spriteName);
        sideCharacterImage.sprite = s != null ? s : defaultSprite;
        sideCharacterImage.gameObject.SetActive(true);
    }

    void HideCenterImage()
    {
        centerCharacterImage.gameObject.SetActive(false);
    }

    void HideSideImage()
    {
        sideCharacterImage.gameObject.SetActive(false);
    }
}
