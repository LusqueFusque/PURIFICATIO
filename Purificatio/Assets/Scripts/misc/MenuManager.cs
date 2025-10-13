using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject Panel_MenuPrincipal;
    public GameObject Panel_LevelSelect;
    public GameObject Panel_MenuOptions;

    //Botoes - Panel_MenuPrincipal:
    public Button ButtonLevelSelect;
    public Button ButtonOptions;
    public Button ButtonCredits;
    public Button ButtonExit;

    //Botoes - Panel_LevelSelect:
    public Button ButtonFaseTutorial;
    public Button ButtonVoltar_Fases;

    //!!!!!Ainda n�o usado:
    //public Button ButtonFase1;
    //public Button ButtonFase2;
    //public Button ButtonFase3;
    //public Button ButtonFase4;

    //Botoes - Panel_MenuOptions:
    public Button ButtonVoltar_Options;

    //!!!!!Ainda n�o usado:
    //public Button ButtonSom;
    //Slider de audio??;

    void Start()
    {
        // - MENU PRINCIPAL -
        ButtonLevelSelect.onClick.AddListener(MostraLevelSelect);
        ButtonOptions.onClick.AddListener(MostraMenuOp);
       // ButtonCredits.onClick.AddListener(MostraCréditos); - TO DO
        ButtonExit.onClick.AddListener(OnQuitClick);


        // - SELEÇAO DE FASES -
        ButtonFaseTutorial.onClick.AddListener(IrTutorial);
        ButtonVoltar_Fases.onClick.AddListener(MostraMenuPrincipal);
        //public Button ButtonFase1;
        //public Button ButtonFase2;
        //public Button ButtonFase3;
        //public Button ButtonFase4;


        // - MENU DE OP��ES -
        ButtonVoltar_Options.onClick.AddListener(MostraMenuPrincipal);
        //public Button ButtonSom;
        //Slider de audio??;
        //public Button Idioma (n�o faz nada, � s� meme);

    }

    void Update()
    {
        
    }

    //Aqui pra a administra��o dos PANELS

    public void MostraMenuPrincipal()
    {
        Panel_MenuPrincipal.SetActive(true);
        Panel_LevelSelect.SetActive(false);
        Panel_MenuOptions.SetActive(false);
    }
    public void MostraLevelSelect()
    {
        Panel_MenuPrincipal.SetActive(false);
        Panel_LevelSelect.SetActive(true);
        Panel_MenuOptions.SetActive(false);
    }
    public void MostraMenuOp()
    {
        Panel_MenuPrincipal.SetActive(false);
        Panel_LevelSelect.SetActive(false);
        Panel_MenuOptions.SetActive(true);
    }

    // Aqui TODO O RESTO

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void IrTutorial()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.LoadScene("05. Tutorial");
    }

}
