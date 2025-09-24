using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject phonePanel;           // Container do celular
    public Transform optionsParent;         // Parent dos botões
    public GameObject optionButtonPrefab;   // Prefab do botão

    private List<GameObject> currentButtons = new List<GameObject>();

    // Mostra as opções do JSON para o diálogo atual
    public void ShowPhoneOptions(List<DialogueOption> options)
    {
        if (options == null || options.Count == 0)
        {
            Debug.LogWarning("Nenhuma opção para mostrar no celular.");
            return;
        }

        ClearOptions();
        phonePanel.SetActive(true);

        foreach (var opt in options)
        {
            GameObject btnGO = Instantiate(optionButtonPrefab, optionsParent);
            Button btn = btnGO.GetComponent<Button>();
            btn.GetComponentInChildren<Text>().text = opt.optionText;
            btn.onClick.AddListener(() =>
            {
                DialogueManager.Instance.GoToNode(opt.nextId);
                HidePhoneOptions();
            });
            currentButtons.Add(btnGO);
        }

        // Sempre adiciona botão de fechar celular
        GameObject closeBtn = Instantiate(optionButtonPrefab, optionsParent);
        Button closeButton = closeBtn.GetComponent<Button>();
        closeButton.GetComponentInChildren<Text>().text = "Fechar";
        closeButton.onClick.AddListener(HidePhoneOptions);
        currentButtons.Add(closeBtn);
    }

    public void HidePhoneOptions()
    {
        phonePanel.SetActive(false);
        ClearOptions();
    }

    private void ClearOptions()
    {
        foreach (var btn in currentButtons)
            Destroy(btn);
        currentButtons.Clear();
    }
}