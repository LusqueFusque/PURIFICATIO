using UnityEngine;

/// <summary>
/// Sistema SIMPLES de save para decisões de exorcismo.
/// NÃO interfere com o GameManager existente.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    [Header("Decisões do Jogador")]
    public bool fase1_exorcizou;
    public bool fase2_exorcizou;
    public bool fase3_exorcizou;
    public bool fase4_exorcizou;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CarregarSave();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Salvar()
    {
        PlayerPrefs.SetInt("decisao_fase1", fase1_exorcizou ? 1 : 0);
        PlayerPrefs.SetInt("decisao_fase2", fase2_exorcizou ? 1 : 0);
        PlayerPrefs.SetInt("decisao_fase3", fase3_exorcizou ? 1 : 0);
        PlayerPrefs.SetInt("decisao_fase4", fase4_exorcizou ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Decisões salvas!");
    }

    void CarregarSave()
    {
        fase1_exorcizou = PlayerPrefs.GetInt("decisao_fase1", 0) == 1;
        fase2_exorcizou = PlayerPrefs.GetInt("decisao_fase2", 0) == 1;
        fase3_exorcizou = PlayerPrefs.GetInt("decisao_fase3", 0) == 1;
        fase4_exorcizou = PlayerPrefs.GetInt("decisao_fase4", 0) == 1;
        Debug.Log("[SaveSystem] Decisões carregadas!");
    }

    public void ResetarSave()
    {
        fase1_exorcizou = false;
        fase2_exorcizou = false;
        fase3_exorcizou = false;
        fase4_exorcizou = false;
        PlayerPrefs.DeleteKey("decisao_fase1");
        PlayerPrefs.DeleteKey("decisao_fase2");
        PlayerPrefs.DeleteKey("decisao_fase3");
        PlayerPrefs.DeleteKey("decisao_fase4");
        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Decisões resetadas!");
    }

    public int ContarExorcismos()
    {
        int total = 0;
        if (fase1_exorcizou) total++;
        if (fase2_exorcizou) total++;
        if (fase3_exorcizou) total++;
        if (fase4_exorcizou) total++;
        return total;
    }

    // ✅ Reset automático ao sair do jogo
    void OnApplicationQuit()
    {
        ResetarSave();
        Debug.Log("[SaveSystem] Reset automático ao sair do jogo.");
    }
}
