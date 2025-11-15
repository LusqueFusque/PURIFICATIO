using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Martelo usado para quebrar o vidro de proteção.
/// Segue o padrão de CrowbarItem da Fase 1.
/// </summary>
public class HammerItem : MonoBehaviour
{
    public static HammerItem Instance;

    [Header("Painel da Cena")]
    [Tooltip("Image do painel do depósito (troca apenas o sprite)")]
    public Image depositoPanelImage;

    [Header("Sprites do Painel")]
    [Tooltip("Sprite com vidro INTACTO")]
    public Sprite spriteGlassIntact;
    
    [Tooltip("Sprite com vidro QUEBRADO e chave visível")]
    public Sprite spriteGlassBroken;

    [Header("ClickAreas Condicionais")]
    [Tooltip("ClickArea do vidro (desativa após quebrar)")]
    public GameObject glassClickArea;
    
    [Tooltip("ClickArea da chave (ativa após quebrar)")]
    public GameObject keyClickArea;

    [Header("Áudio")]
    public AudioClip glassBreakSound;

    private bool isActive = false;
    private bool glassIsBroken = false;
    private float activationTime = 0f;
    private const float ACTIVATION_DELAY = 0.2f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[HammerItem] Destruindo duplicata.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log($"[HammerItem] Instance configurado. ID: {GetInstanceID()}");
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
        // Garante estado inicial correto
        if (depositoPanelImage != null && spriteGlassIntact != null)
        {
            depositoPanelImage.sprite = spriteGlassIntact;
        }

        if (glassClickArea != null)
            glassClickArea.SetActive(true);
            
        if (keyClickArea != null)
            keyClickArea.SetActive(false);
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

    public void Activate()
    {
        Debug.Log("[HammerItem] ===== ACTIVATE CHAMADO =====");
        isActive = true;
        activationTime = Time.time;
        Debug.Log($"[HammerItem] ✓ MARTELO ATIVADO! (ID: {GetInstanceID()})");
    }

    public void Deactivate()
    {
        Debug.Log("[HammerItem] ===== DEACTIVATE CHAMADO =====");
        isActive = false;
        Debug.Log("[HammerItem] ✗ Martelo desativado.");
    }

    public void Toggle()
    {
        Debug.Log("[HammerItem] ===== TOGGLE CHAMADO =====");
        Debug.Log($"[HammerItem] Estado ANTES: isActive={isActive}");

        if (isActive)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void OnItemClicked()
    {
        Debug.Log("[HammerItem] OnItemClicked → Chamando Toggle()");
        Toggle();
    }

    // ============================================
    // USO DO ITEM
    // ============================================

    public void TryUseOn(GameObject target)
    {
        Debug.Log("[HammerItem] ========================================");
        Debug.Log("[HammerItem] TryUseOn chamado.");
        Debug.Log($"[HammerItem] isActive={isActive}, target={(target ? target.name : "null")}");

        if (!isActive)
        {
            Debug.Log("[HammerItem] Ignorando: martelo não ativo.");
            Debug.Log("[HammerItem] ========================================");
            return;
        }

        // Quebra o vidro de proteção
        if (target.CompareTag("ProtectionGlass") && !glassIsBroken)
        {
            Debug.Log("[HammerItem] ✓ Quebrando vidro de proteção...");
            BreakGlass();
        }
        else if (glassIsBroken)
        {
            Debug.Log("[HammerItem] Vidro já foi quebrado.");
        }
        else
        {
            Debug.Log("[HammerItem] Nada a fazer aqui.");
        }

        Debug.Log("[HammerItem] ========================================");
    }

    private void BreakGlass()
    {
        glassIsBroken = true;

        // Som de vidro quebrando
        if (glassBreakSound != null)
        {
            AudioSource.PlayClipAtPoint(glassBreakSound, Camera.main.transform.position, 0.6f);
        }

        // ============================================
        // TROCA APENAS O SPRITE DA IMAGE
        // ============================================
        if (depositoPanelImage != null && spriteGlassBroken != null)
        {
            Debug.Log("[HammerItem] 🔄 Trocando sprite: Vidro Intacto → Vidro Quebrado");
            depositoPanelImage.sprite = spriteGlassBroken;
            Debug.Log("[HammerItem] ✓ Sprite do painel atualizado!");
        }
        else
        {
            Debug.LogWarning("[HammerItem] ⚠️ Referências não atribuídas no Inspector!");
        }

        // Desativa ClickArea do vidro
        if (glassClickArea != null)
        {
            glassClickArea.SetActive(false);
            Debug.Log("[HammerItem] ✓ ClickArea do vidro desativada.");
        }

        // Ativa ClickArea da chave
        if (keyClickArea != null)
        {
            keyClickArea.SetActive(true);
            Debug.Log("[HammerItem] ✓ ClickArea da chave ativada.");
        }

        // Seta flag global
        if (AdvancedMapManager.Instance != null)
        {
            AdvancedMapManager.Instance.SetGlobalFlag("GlassBroken", true);
        }

        Debug.Log("[HammerItem] ✓✓✓ Vidro quebrado com sucesso! Chave agora está visível!");
        
        // Martelo permanece ativo
    }
}