using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    [Header("Menu")]
    public GameObject Panel_InGameMenu;

    [Tooltip("Botão de engrenagem que abre o menu")]
    public Button ButtonOpenMenu;

    [Tooltip("Botão que FECHA o menu (novo 'Sair' deste menu)")]
    public Button ButtonCloseMenu;

    [Tooltip("Botão 'Voltar ao Menu Principal'")]
    public Button ButtonVoltarMenu;

    [Header("Panels a serem DESATIVADOS quando o menu abrir")]
    public GameObject[] PanelsParaDesativar;

    [Header("Panels a serem REATIVADOS quando o menu fechar")]
    public GameObject[] PanelsParaReativar;

    private bool menuAberto = false;

    void Start()
    {
        Panel_InGameMenu.SetActive(false);

        ButtonOpenMenu.onClick.AddListener(AbrirMenu);
        ButtonCloseMenu.onClick.AddListener(FecharMenu);      // AGORA FECHA O MENU
        ButtonVoltarMenu.onClick.AddListener(VoltarMenuPrincipal); // AGORA SAI DA FASE
    }

    void AbrirMenu()
    {
        if (menuAberto) return;

        Panel_InGameMenu.SetActive(true);
        menuAberto = true;

        foreach (var panel in PanelsParaDesativar)
        {
            if (panel != null) panel.SetActive(false);
        }

        Time.timeScale = 0f;
    }

    void FecharMenu()
    {
        if (!menuAberto) return;

        Panel_InGameMenu.SetActive(false);
        menuAberto = false;

        foreach (var panel in PanelsParaReativar)
        {
            if (panel != null) panel.SetActive(true);
        }

        Time.timeScale = 1f;
    }

    void VoltarMenuPrincipal()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene("03. MainMenu");
            return;
        }

        SceneManager.LoadScene("03. MainMenu");
    }
}