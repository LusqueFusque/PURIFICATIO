using UnityEngine;
using UnityEngine.UI;

public class CrowbarItem : MonoBehaviour
{
    public static CrowbarItem Instance;

    private bool isActive = false;
    public Image salaPanel;
    public Sprite madeiraRemovidaSprite;
    public GameObject bonecaImage;
    private bool madeiraRemovida = false;

    private float activationTime = 0f;
    private const float ACTIVATION_DELAY = 0.2f;
    
    [Header("Áudio")]
    public AudioClip crowbarUseSound;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[CrowbarItem] Destruindo duplicata.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log($"[CrowbarItem] Instance configurado. ID: {GetInstanceID()}");
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
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
    /// Ativa o crowbar.
    /// </summary>
    public void Activate()
    {
        Debug.Log("[CrowbarItem] ===== ACTIVATE CHAMADO =====");
        
        // Desativa stapler se estiver ativo
        if (StaplerItem.Instance != null && StaplerItem.Instance.IsActive())
        {
            StaplerItem.Instance.Deactivate();
            Debug.Log("[CrowbarItem] Stapler desativado.");
        }

        isActive = true;
        activationTime = Time.time;
        Debug.Log($"[CrowbarItem] ✓ CROWBAR ATIVADO! (ID: {GetInstanceID()})");
    }

    /// <summary>
    /// Desativa o crowbar.
    /// </summary>
    public void Deactivate()
    {
        Debug.Log("[CrowbarItem] ===== DEACTIVATE CHAMADO =====");
        isActive = false;
        Debug.Log("[CrowbarItem] ✗ Crowbar desativado.");
    }

    /// <summary>
    /// Alterna entre ativo/inativo (toggle).
    /// </summary>
    public void Toggle()
    {
        Debug.Log("[CrowbarItem] ===== TOGGLE CHAMADO =====");
        Debug.Log($"[CrowbarItem] Estado ANTES: isActive={isActive}");
        
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
    /// Verifica se o crowbar está ativo.
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
        Debug.Log("[CrowbarItem] OnItemClicked → Chamando Toggle()");
        Toggle();
    }

    // ============================================
    // USO DO ITEM
    // ============================================

    public void TryUseOn(GameObject target)
    {
        Debug.Log("[CrowbarItem] ========================================");
        Debug.Log("[CrowbarItem] TryUseOn chamado.");
        Debug.Log($"[CrowbarItem] isActive={isActive}, target={(target ? target.name : "null")}");

        if (!isActive)
        {
            Debug.Log("[CrowbarItem] Ignorando: crowbar não ativo.");
            Debug.Log("[CrowbarItem] ========================================");
            return;
        }

        if (target.CompareTag("WoodLoose") && !madeiraRemovida)
        {
            Debug.Log("[CrowbarItem] ✓ Removendo madeira...");

            if (salaPanel != null && madeiraRemovidaSprite != null)
                salaPanel.sprite = madeiraRemovidaSprite;

            madeiraRemovida = true;
            Debug.Log("[CrowbarItem] ✓✓ Madeira removida! Crowbar permanece ativo.");

            // 🔊 Som de uso
            if (crowbarUseSound != null)
                AudioSource.PlayClipAtPoint(crowbarUseSound, Camera.main.transform.position, 0.7f);

            // Desativa clickareas da madeira
            var clickables = GameObject.FindGameObjectsWithTag("WoodLoose");
            foreach (var go in clickables) go.SetActive(false);

            // Ativa boneca
            if (bonecaImage != null) bonecaImage.SetActive(true);

            // Seta flag global
            if (AdvancedMapManager.Instance != null)
                AdvancedMapManager.Instance.SetGlobalFlag("WoodRemoved", true);
        }
        else
        {
            Debug.Log("[CrowbarItem] Nada a fazer aqui.");
        }
        
        Debug.Log("[CrowbarItem] ========================================");
    }
}