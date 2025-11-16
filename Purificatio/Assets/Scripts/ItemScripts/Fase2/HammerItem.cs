using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lógica do MARTELO - Quebra vidro de proteção para revelar chave
/// </summary>
public class HammerItem : MonoBehaviour
{
    public static HammerItem Instance;

    [Header("Painel da Cena - Vidro de Proteção")]
    [Tooltip("Image do painel do depósito (troca sprite)")]
    public Image depositoPanelImage;

    [Header("Sprites do Painel")]
    public Sprite spriteGlassIntact;
    public Sprite spriteGlassBroken;

    [Header("Objetos Condicionais")]
    [Tooltip("Image da chave (ativa após quebrar vidro)")]
    public GameObject keyCollectibleImage;

    [Header("Áudio")]
    public AudioClip glassBreakSound;

    private bool isActive = false;
    private bool glassIsBroken = false;

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

    void Start()
    {
        // Estado inicial: vidro intacto, chave invisível
        if (depositoPanelImage != null && spriteGlassIntact != null)
            depositoPanelImage.sprite = spriteGlassIntact;

        if (keyCollectibleImage != null)
            keyCollectibleImage.SetActive(false);
    }

    void Update()
    {
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
        Debug.Log("[HammerItem] Martelo ATIVADO");
        isActive = true;
    }

    public void Deactivate()
    {
        Debug.Log("[HammerItem] Martelo DESATIVADO");
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
    // USO DO MARTELO
    // ============================================
    public void UseHammer()
    {
        Debug.Log("[HammerItem] UseHammer chamado!");

        if (!isActive)
        {
            Debug.Log("[HammerItem] Martelo não está ativo!");
            return;
        }

        if (glassIsBroken)
        {
            Debug.Log("[HammerItem] Vidro já foi quebrado!");
            return;
        }

        BreakGlass();
    }

    private void BreakGlass()
    {
        glassIsBroken = true;

        // Som
        if (glassBreakSound != null)
            AudioSource.PlayClipAtPoint(glassBreakSound, Camera.main.transform.position, 0.6f);

        // Troca sprite do painel
        if (depositoPanelImage != null && spriteGlassBroken != null)
        {
            depositoPanelImage.sprite = spriteGlassBroken;
            Debug.Log("[HammerItem] ✓ Sprite trocado: vidro quebrado");
        }

        // Mostra a chave coletável
        if (keyCollectibleImage != null)
        {
            keyCollectibleImage.SetActive(true);
            Debug.Log("[HammerItem] ✓ Chave agora visível para coleta");
        }

        // Flag global (opcional)
        if (AdvancedMapManager.Instance != null)
            AdvancedMapManager.Instance.SetGlobalFlag("GlassBroken", true);

        Debug.Log("[HammerItem] ✓✓ Vidro quebrado com sucesso!");
    }
}