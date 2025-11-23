using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ArmaSanta - Ativa, clica em MazzikinImage para exorcizar
/// </summary>
public class ArmaSantaItem : MonoBehaviour
{
    public static ArmaSantaItem Instance;

    [Header("Configuração")]
    public Image mazzikinImage; // UI Image do Mazzi que será clicada
    public AudioClip activateSound;
    public AudioClip exorcismSound;

    private bool isActive = false;
    private MazzikinClickHandler mazziClickHandler;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        // Botão direito desativa
        if (isActive && Input.GetMouseButtonDown(1))
        {
            Deactivate();
        }
    }

    // ============================================
    // CONTROLE DE ATIVAÇÃO
    // ============================================
    public void Activate()
    {
        Debug.Log("[ArmaSantaItem] ArmaSanta ATIVADA");
        isActive = true;
        
        if (activateSound != null)
            AudioSource.PlayClipAtPoint(activateSound, Camera.main.transform.position, 0.6f);
        
        // Configura o handler de clique no Mazzi
        if (mazzikinImage != null)
        {
            mazziClickHandler = mazzikinImage.GetComponent<MazzikinClickHandler>();
            if (mazziClickHandler == null)
            {
                mazziClickHandler = mazzikinImage.gameObject.AddComponent<MazzikinClickHandler>();
            }
            mazziClickHandler.SetArmaSanta(this);
        }
    }

    public void Deactivate()
    {
        Debug.Log("[ArmaSantaItem] ArmaSanta DESATIVADA");
        isActive = false;
    }

    public void Toggle()
    {
        if (isActive)
            Deactivate();
        else
            Activate();
    }

    public bool IsActive() => isActive;

    // ============================================
    // EXORCISMO
    // ============================================
    public void ExorcizeMazzi()
    {
        if (!isActive)
        {
            Debug.Log("[ArmaSantaItem] ArmaSanta não está ativa!");
            return;
        }

        Debug.Log("[ArmaSantaItem] Exorcizando Mazzi...");

        if (exorcismSound != null)
            AudioSource.PlayClipAtPoint(exorcismSound, Camera.main.transform.position, 0.8f);

        // Completa a missão de exorcismo
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission("exorcismoMazzi");
            Debug.Log("[ArmaSantaItem] ✓ Missão 'exorcismoMazzi' completada!");
        }

        Deactivate();
    }
}

/// <summary>
/// Handler de clique para MazzikinImage
/// Ativado apenas quando ArmaSanta está equipada
/// </summary>
public class MazzikinClickHandler : MonoBehaviour, IPointerClickHandler
{
    private ArmaSantaItem armaSanta;

    public void SetArmaSanta(ArmaSantaItem arma)
    {
        armaSanta = arma;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[MazzikinClickHandler] Mazzi foi clicado!");

        if (armaSanta == null)
        {
            Debug.LogWarning("[MazzikinClickHandler] ArmaSanta não configurada!");
            return;
        }

        if (!armaSanta.IsActive())
        {
            Debug.Log("[MazzikinClickHandler] ArmaSanta não está ativa!");
            return;
        }

        // Exorciza o Mazzi
        armaSanta.ExorcizeMazzi();
    }
}