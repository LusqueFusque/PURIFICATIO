using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// ArmaSanta - Ativa, clica em MazzikinImage para exorcizar
/// </summary>
public class ArmaSantaItem : MonoBehaviour

{
    [Header("Item a dar")]
    public ItemData armaSantaItem; // ← Arraste o ItemData aqui no Inspector
    
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
    private void GiveArmaSanta()
    {
        if (armaSantaItem != null)
        {
            // pega a instância do inventário na cena
            DynamicInventory inventory = FindObjectOfType<DynamicInventory>();
            if (inventory != null)
            {
                bool added = inventory.AddItem(armaSantaItem);
                if (added)
                    Debug.Log("[ArmaSantaItem] 🎁 ArmaSanta adicionada ao inventário!");
                else
                    Debug.LogWarning("[ArmaSantaItem] Inventário cheio!");
            }
            else
            {
                Debug.LogError("[ArmaSantaItem] Nenhum DynamicInventory encontrado na cena!");
            }
        }
        else
        {
            Debug.LogError("[ClickAreaPanela] ItemData 'ArmaSanta' não atribuído no Inspector!");
        }
    }
}