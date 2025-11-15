using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla a interação com a lâmpada do Djinn na Fase 2.
/// Similar ao padrão de itens coletáveis da Fase 1.
/// </summary>
public class LampItem : MonoBehaviour
{
    public static LampItem Instance;

    [Header("Referências")]
    [Tooltip("GameObject da lâmpada na cena (com sprite)")]
    public GameObject lampSceneObject;
    
    [Tooltip("Sprite/Image da lâmpada que aparece quando encontrada")]
    public Image lampFoundIndicator;

    private bool lampFound = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[LampItem] Destruindo duplicata.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log($"[LampItem] Instance configurado. ID: {GetInstanceID()}");
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        // Garante que o indicador começa invisível
        if (lampFoundIndicator != null)
        {
            lampFoundIndicator.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Marca que a lâmpada foi encontrada pelo jogador.
    /// Chamado quando o jogador clica na lâmpada na cena.
    /// </summary>
    public void FindLamp()
    {
        Debug.Log("[LampItem] ========================================");
        Debug.Log("[LampItem] FindLamp chamado!");

        if (lampFound)
        {
            Debug.Log("[LampItem] Lâmpada já foi encontrada anteriormente.");
            Debug.Log("[LampItem] ========================================");
            return;
        }

        lampFound = true;

        // Mostra indicador visual se houver
        if (lampFoundIndicator != null)
        {
            lampFoundIndicator.gameObject.SetActive(true);
            Debug.Log("[LampItem] ✓ Indicador visual ativado.");
        }

        // Desativa o objeto da lâmpada na cena (foi coletada)
        if (lampSceneObject != null)
        {
            lampSceneObject.SetActive(false);
            Debug.Log("[LampItem] ✓ Lâmpada removida da cena.");
        }

        // Completa a missão FindLamp
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission("FindLamp");
            Debug.Log("[LampItem] ✓✓ Missão 'FindLamp' completada!");
        }
        else
        {
            Debug.LogError("[LampItem] ❌ MissionManager não encontrado!");
        }

        // Seta flag global
        if (AdvancedMapManager.Instance != null)
        {
            AdvancedMapManager.Instance.SetGlobalFlag("LampFound", true);
            Debug.Log("[LampItem] ✓ Flag global 'LampFound' setada.");
        }

        Debug.Log("[LampItem] ========================================");
    }

    /// <summary>
    /// Verifica se a lâmpada já foi encontrada.
    /// </summary>
    public bool IsLampFound()
    {
        return lampFound;
    }

    /// <summary>
    /// Usado quando o jogador clica na lâmpada na cena.
    /// Método público para ser chamado por ClickableAreaUI ou similar.
    /// </summary>
    public void OnLampClicked()
    {
        Debug.Log("[LampItem] OnLampClicked chamado.");
        
        if (!lampFound)
        {
            FindLamp();
        }
        else
        {
            Debug.Log("[LampItem] Lâmpada já foi encontrada. Sem efeito.");
        }
    }
}