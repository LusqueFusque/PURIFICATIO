using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject Panel_MenuPrincipal;
    public GameObject Panel_LevelSelect;
    public GameObject Panel_MenuOptions;

    // Botoes - Panel_MenuPrincipal:
    public Button ButtonLevelSelect;
    public Button ButtonOptions;
    public Button ButtonCredits;
    public Button ButtonExit;

    // Botoes - Panel_LevelSelect:
    public Button ButtonFaseTutorial;
    public Button ButtonVoltar_Fases;

    // Botoes - Panel_MenuOptions:
    public Button ButtonVoltar_Options;

    void Start()
    {
        // - MENU PRINCIPAL -
        ButtonLevelSelect.onClick.AddListener(MostraLevelSelect);
        ButtonOptions.onClick.AddListener(MostraMenuOp);
        // ButtonCredits.onClick.AddListener(MostraCreditos); - TO DO
        ButtonExit.onClick.AddListener(OnQuitClick);

        // - SELEÇÃO DE FASES
        ButtonFaseTutorial.onClick.AddListener(IrTutorial);
        ButtonVoltar_Fases.onClick.AddListener(MostraMenuPrincipal);

        // - MENU DE OPÇÕES -
        ButtonVoltar_Options.onClick.AddListener(MostraMenuPrincipal);
    }

    // Aqui pra a administração dos PANELS
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
        Panel_MenuOptions.SetActive(true);
    }

    // Aqui TODO O RESTO
    public void OnQuitClick()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void IrTutorial()
    {
        // Null check
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene("05. Tutorial");
        }
        else
        {
            Debug.LogError("[MenuManager] GameManager não encontrado!");
            SceneManager.LoadScene("05. Tutorial"); // Fallback
        }
    }
}