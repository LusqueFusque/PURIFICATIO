using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        // Singleton
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

    private IEnumerator SplashCoroutine()
    {
        SceneManager.LoadScene("01. Splash");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("02. Menu");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // --- NOVO SISTEMA DE SAVE ---
    // Atualiza decisão de exorcismo em determinada fase
    public void UpdateExorcismDecision(int faseID, bool exorcizou)
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("[GameManager] ❌ SaveSystem não encontrado!");
            return;
        }

        switch (faseID)
        {
            case 1: SaveSystem.Instance.fase1_exorcizou = exorcizou; break;
            case 2: SaveSystem.Instance.fase2_exorcizou = exorcizou; break;
            case 3: SaveSystem.Instance.fase3_exorcizou = exorcizou; break;
            case 4: SaveSystem.Instance.fase4_exorcizou = exorcizou; break;
            default:
                Debug.LogWarning($"[GameManager] Fase {faseID} inválida para salvar decisão.");
                return;
        }

        SaveSystem.Instance.Salvar();
        Debug.Log($"[GameManager] Decisão da Fase {faseID} registrada: exorcizou = {exorcizou}");
    }

    // Retorna se o jogador exorcizou em determinada fase
    public bool GetExorcismDecision(int faseID)
    {
        if (SaveSystem.Instance == null) return false;

        return faseID switch
        {
            1 => SaveSystem.Instance.fase1_exorcizou,
            2 => SaveSystem.Instance.fase2_exorcizou,
            3 => SaveSystem.Instance.fase3_exorcizou,
            4 => SaveSystem.Instance.fase4_exorcizou,
            _ => false
        };
    }

    // Conta quantos exorcismos o jogador já fez
    public int GetTotalExorcisms()
    {
        if (SaveSystem.Instance == null) return 0;
        return SaveSystem.Instance.ContarExorcismos();
    }

    // Resetar todas as decisões
    public void ResetAllDecisions()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.ResetarSave();
            Debug.Log("[GameManager] Todas as decisões resetadas.");
        }
    }
}
