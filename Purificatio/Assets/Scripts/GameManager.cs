using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        // Singleton: garante que só exista um GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "00. Initialization")
        {
            LoadSplashSequence();
        }
    }

    private void LoadSplashSequence()
    {
        StartCoroutine(SplashCoroutine());
    }

    private System.Collections.IEnumerator SplashCoroutine()
    {
        // Carrega a splash screen
        SceneManager.LoadScene("01. Splash");
        // Espera 3 segundos
        yield return new WaitForSeconds(3f);
        // Carrega o menu
        SceneManager.LoadScene("02. Menu");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
