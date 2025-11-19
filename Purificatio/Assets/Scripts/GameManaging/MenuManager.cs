using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Panels
    public GameObject Panel_MenuPrincipal;
    public GameObject Panel_LevelSelect;
    public GameObject Panel_MenuOptions;
    public GameObject Panel_Creditos;

    // Botões - Panel_MenuPrincipal:
    public Button ButtonPlay;
    public Button ButtonOpt;
    public Button ButtonCredits;
    public Button ButtonExit;

    // Botões - Panel_LevelSelect:
    public Button ButtonFaseTutorial;
    public Button ButtonFase1;
    public Button ButtonFase2;
    public Button ButtonFase3;
    public Button ButtonFase4;
    public Button ButtonVoltar_Fases;

    // Botões - Panel_MenuOptions:
    public Button OptMenuButtonSair;
    public Button OptMenuButtonVoltar;
    
    // Botões - Panel_Creditos:
    public Button OptCreditsButtonVoltar;

    // Componentes adicionais
    public Image Image; // Referência à imagem no Panel_MenuOptions

    void Start()
    {
        // - MENU PRINCIPAL -
        ButtonPlay.onClick.AddListener(MostraLevelSelect);
        ButtonOpt.onClick.AddListener(MostraMenuOp);
        ButtonCredits.onClick.AddListener(MostraCreditos);
        ButtonExit.onClick.AddListener(OnQuitClick);

        // - SELEÇÃO DE FASES -
        ButtonFaseTutorial.onClick.AddListener(IrTutorial);
        ButtonFase1.onClick.AddListener(IrFase1);
        ButtonFase2.onClick.AddListener(IrFase2);
        ButtonFase3.onClick.AddListener(IrFase3);
        ButtonFase4.onClick.AddListener(IrFase4);
        ButtonVoltar_Fases.onClick.AddListener(MostraMenuPrincipal);

        // - MENU DE OPÇÕES -
        OptMenuButtonVoltar.onClick.AddListener(MostraMenuPrincipal);
        OptMenuButtonSair.onClick.AddListener(OnQuitClick);
        
        // - CRÉDITOS -
        OptCreditsButtonVoltar.onClick.AddListener(MostraMenuPrincipal);

        // Inicializa mostrando o menu principal
        MostraMenuPrincipal();
    }

    // Administração dos PANELS
    public void MostraMenuPrincipal()
    {
        Panel_MenuPrincipal.SetActive(true);
        Panel_LevelSelect.SetActive(false);
        Panel_MenuOptions.SetActive(false);
        Panel_Creditos.SetActive(false);
    }

    public void MostraLevelSelect()
    {
        Panel_MenuPrincipal.SetActive(false);
        Panel_LevelSelect.SetActive(true);
        Panel_MenuOptions.SetActive(false);
        Panel_Creditos.SetActive(false);
    }

    public void MostraMenuOp()
    {
        Panel_MenuPrincipal.SetActive(false);
        Panel_LevelSelect.SetActive(false);
        Panel_MenuOptions.SetActive(true);
        Panel_Creditos.SetActive(false);
    }

    public void MostraCreditos()
    {
        Panel_MenuPrincipal.SetActive(false);
        Panel_LevelSelect.SetActive(false);
        Panel_MenuOptions.SetActive(false);
        Panel_Creditos.SetActive(true);
    }

    // Navegação de cenas
    public void OnQuitClick()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void IrTutorial()
    {
        CarregarCena("05. Tutorial");
    }

    public void IrFase1()
    {
        CarregarCena("06. Fase1"); // Ajuste o nome da cena conforme necessário
    }

    public void IrFase2()
    {
        CarregarCena("07. Fase2"); // Ajuste o nome da cena conforme necessário
    }

    public void IrFase3()
    {
        CarregarCena("08. Fase3"); // Ajuste o nome da cena conforme necessário
    }

    public void IrFase4()
    {
        CarregarCena("09. Fase4"); // Ajuste o nome da cena conforme necessário
    }

    // Método auxiliar para carregar cenas
    private void CarregarCena(string nomeCena)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(nomeCena);
        }
        else
        {
            Debug.LogError("[MenuManager] GameManager não encontrado!");
            SceneManager.LoadScene(nomeCena); // Fallback
        }
    }
}