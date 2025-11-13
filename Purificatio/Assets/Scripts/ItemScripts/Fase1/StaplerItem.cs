using UnityEngine;
using UnityEngine.UI;

public class StaplerItem : MonoBehaviour
{
    public static StaplerItem Instance;
    private bool isActive = false;

    public Image bonecaImage;
    public Sprite bonecaGrampeadaSprite;

    private float activationTime = 0f;
    private const float ACTIVATION_DELAY = 0.2f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[StaplerItem] Destruindo duplicata.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log($"[StaplerItem] Instance configurado. ID: {GetInstanceID()}");
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
        Debug.Log($"[StaplerItem] Iniciado. bonecaImage={bonecaImage != null}, sprite={bonecaGrampeadaSprite != null}");
    }

    void Update()
    {
        // Botão direito desativa (com proteção de delay)
        if (isActive && Input.GetMouseButtonDown(1) && Time.time - activationTime > ACTIVATION_DELAY)
        {
            Deactivate();
        }
    }

    // ============================================
    // MÉTODOS PÚBLICOS
    // ============================================

    /// <summary>
    /// Ativa o grampeador.
    /// </summary>
    public void Activate()
    {
        Debug.Log("[StaplerItem] ===== ACTIVATE CHAMADO =====");
        
        // Desativa crowbar se estiver ativo
        if (CrowbarItem.Instance != null && CrowbarItem.Instance.IsActive())
        {
            CrowbarItem.Instance.Deactivate();
            Debug.Log("[StaplerItem] Crowbar desativado.");
        }

        isActive = true;
        activationTime = Time.time;
        Debug.Log($"[StaplerItem] ✓ GRAMPEADOR ATIVADO! (ID: {GetInstanceID()})");
    }

    /// <summary>
    /// Desativa o grampeador.
    /// </summary>
    public void Deactivate()
    {
        Debug.Log("[StaplerItem] ===== DEACTIVATE CHAMADO =====");
        isActive = false;
        Debug.Log("[StaplerItem] ✗ Grampeador desativado.");
    }

    /// <summary>
    /// Alterna entre ativo/inativo (toggle).
    /// </summary>
    public void Toggle()
    {
        Debug.Log("[StaplerItem] ===== TOGGLE CHAMADO =====");
        Debug.Log($"[StaplerItem] Estado ANTES: isActive={isActive}");
        
        if (isActive)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    /// <summary>
    /// Verifica se o grampeador está ativo.
    /// </summary>
    public bool IsActive()
    {
        return isActive;
    }

    /// <summary>
    /// Método de compatibilidade para código antigo.
    /// </summary>
    public void OnItemClicked()
    {
        Debug.Log("[StaplerItem] OnItemClicked → Chamando Toggle()");
        Toggle();
    }

    // ============================================
    // USO DO ITEM
    // ============================================

    public void TryUseOn(GameObject target)
    {
        Debug.Log($"[StaplerItem] ========================================");
        Debug.Log($"[StaplerItem] TryUseOn CHAMADO!");
        Debug.Log($"[StaplerItem] GameObject: {gameObject.name} (ID: {GetInstanceID()})");
        Debug.Log($"[StaplerItem] isActive: {isActive}");
        Debug.Log($"[StaplerItem] Target: {target.name}, Tag: {target.tag}");

        if (!isActive)
        {
            Debug.LogWarning("[StaplerItem] ✖ IGNORADO: Grampeador NÃO está ativo!");
            Debug.LogWarning("[StaplerItem] Clique no botão do inventário primeiro!");
            Debug.Log($"[StaplerItem] ========================================");
            return;
        }

        if (target.CompareTag("Boneca"))
        {
            Debug.Log("[StaplerItem] ✓ Tag 'Boneca' detectada!");
            
            if (bonecaImage != null && bonecaGrampeadaSprite != null)
            {
                bonecaImage.sprite = bonecaGrampeadaSprite;
                Debug.Log("[StaplerItem] ✓✓ Boneca grampeada com sucesso!");
                
                // ✅ COMPLETA A MISSÃO findDoll
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.CompleteMission("findDoll");
                    Debug.Log("[StaplerItem] ✅ Missão 'findDoll' completada!");
                }
                else
                {
                    Debug.LogError("[StaplerItem] ❌ MissionManager.Instance é NULL!");
                }
                
                Debug.Log("[StaplerItem] Grampeador permanece ativo. Use botão direito para desativar.");
            }
            else
            {
                Debug.LogError("[StaplerItem] ✖ ERRO: bonecaImage ou bonecaGrampeadaSprite não atribuídos!");
                Debug.LogError($"[StaplerItem] bonecaImage={bonecaImage}, bonecaGrampeadaSprite={bonecaGrampeadaSprite}");
            }
        }
        else
        {
            Debug.Log($"[StaplerItem] Tag '{target.tag}' não é 'Boneca'. Nada a fazer.");
        }
        
        Debug.Log($"[StaplerItem] ========================================");
    }
}