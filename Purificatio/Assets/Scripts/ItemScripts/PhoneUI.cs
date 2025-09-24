using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUI : MonoBehaviour
{
    public GameObject phonePanel;           // Panel do telefone
    public GameObject optionButtonPrefab;   // Prefab do botão de opção
    public Transform optionContainer;       // Onde os botões serão instanciados

    private List<Button> currentButtons = new List<Button>();

    public void ShowPhoneOptions(List<DialogueOption> options)
    {
        phonePanel.SetActive(true);
        ClearOptions();

        foreach (var opt in options)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, optionContainer);
            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btnObj.GetComponentInChildren<Text>();
            btnText.text = opt.optionText;

            btn.onClick.AddListener(() =>
            {
                OnOptionSelected(opt.nextId);
            });

            currentButtons.Add(btn);
        }

        // Botão de fechar
        GameObject closeBtnObj = Instantiate(optionButtonPrefab, optionContainer);
        Button closeBtn = closeBtnObj.GetComponent<Button>();
        closeBtn.GetComponentInChildren<Text>().text = "Fechar";
        closeBtn.onClick.AddListener(ClosePhone);
        currentButtons.Add(closeBtn);
    }

    private void OnOptionSelected(string nextId)
    {
        ClosePhone();

        // Chama o DialogueManager para continuar o diálogo
        DialogueManager.Instance.ShowDialogueById(nextId);
    }

    private void ClearOptions()
    {
        foreach (var btn in currentButtons)
        {
            Destroy(btn.gameObject);
        }
        currentButtons.Clear();
    }

    private void ClosePhone()
    {
        phonePanel.SetActive(false);
        ClearOptions();
    }
}