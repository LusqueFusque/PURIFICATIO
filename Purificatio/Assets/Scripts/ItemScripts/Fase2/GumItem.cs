using UnityEngine;

/// <summary>
/// Lógica do CHICLETE - Conserta chave quebrada no baú
/// </summary>
public class GumItem : MonoBehaviour
{
    public static GumItem Instance;

    [Header("Áudio")]
    public AudioClip gumUseSound;

    private bool isActive = false;

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
        // ✅ Botão direito desativa
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
        Debug.Log("[GumItem] Chiclete ATIVADO");
        isActive = true;
    }

    public void Deactivate()
    {
        Debug.Log("[GumItem] Chiclete DESATIVADO");
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
    // USO DO CHICLETE
    // ============================================
    public void UseGum()
    {
        Debug.Log("[GumItem] UseGum chamado!");

        if (!isActive)
        {
            Debug.Log("[GumItem] Chiclete não está ativo!");
            return;
        }

        // Som
        if (gumUseSound != null)
            AudioSource.PlayClipAtPoint(gumUseSound, Camera.main.transform.position, 0.5f);

        // Chama KeyItem para consertar
        if (KeyItem.Instance != null)
        {
            KeyItem.Instance.RepairWithGum();
            Debug.Log("[GumItem] ✓ Chiclete usado para consertar chave!");
        }
        else
        {
            Debug.LogWarning("[GumItem] KeyItem não encontrado!");
        }

        Deactivate();
    }
}