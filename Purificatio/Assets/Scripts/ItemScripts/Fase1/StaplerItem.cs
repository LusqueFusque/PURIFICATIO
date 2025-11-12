using UnityEngine;
using UnityEngine.UI;

public class StaplerItem : MonoBehaviour
{
    public static StaplerItem Instance;
    public bool isActive = false;

    public Image bonecaImage; // imagem da boneca na cena
    public Sprite bonecaGrampeadaSprite; // sprite da boneca grampeada

    void Awake()
    {
        // Garante que Instance está sempre atualizado
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[StaplerItem] Já existe uma instância! Destruindo duplicata.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("[StaplerItem] Instance configurado no Awake.");
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Debug.Log("[StaplerItem] Instance limpo no OnDestroy.");
        }
    }

    void Start()
    {
        Debug.Log("[StaplerItem] Script iniciado. Instance=" + (Instance != null ? "OK" : "NULL"));
    }

    void Update()
    {
        // Desativar com botão direito do mouse
        if (isActive && Input.GetMouseButtonDown(1))
        {
            isActive = false;
            Debug.Log("[StaplerItem] Grampeador desativado (botão direito).");
        }
    }

    // Chamado quando o botão do inventário é clicado
    public void OnItemClicked()
    {
        Debug.Log($"[StaplerItem] OnItemClicked CHAMADO! Estado ANTES: isActive={isActive}");

        // Desativa o Crowbar se estiver ativo
        if (CrowbarItem.Instance != null && CrowbarItem.Instance.isActive)
        {
            CrowbarItem.Instance.isActive = false;
            Debug.Log("[StaplerItem] Crowbar desativado.");
        }

        // Toggle do estado
        isActive = !isActive;
        
        Debug.Log($"[StaplerItem] Estado DEPOIS: isActive={isActive}");
        Debug.Log(isActive ? "[StaplerItem] ✓ GRAMPEADOR ATIVADO!" : "[StaplerItem] ✗ Grampeador desativado.");
    }

    // Chamado ao clicar em algo interativo no cenário
    public void TryUseOn(GameObject target)
    {
        Debug.Log($"[StaplerItem] ===== TryUseOn CHAMADO =====");
        Debug.Log($"[StaplerItem] isActive={isActive}");
        Debug.Log($"[StaplerItem] target={target.name}, tag={target.tag}");

        if (!isActive)
        {
            Debug.LogWarning("[StaplerItem] ❌ IGNORADO: Grampeador NÃO está ativo!");
            Debug.LogWarning("[StaplerItem] Você clicou no botão do inventário para ativar o grampeador?");
            return;
        }

        if (target.CompareTag("Boneca")) // ou "BonecaImage" dependendo da sua tag
        {
            Debug.Log("[StaplerItem] ✓ Tag correta detectada!");
            
            if (bonecaImage != null && bonecaGrampeadaSprite != null)
            {
                bonecaImage.sprite = bonecaGrampeadaSprite;
                Debug.Log("[StaplerItem] ✓✓ Boneca grampeada com sucesso! Sprite trocado.");
            }
            else
            {
                Debug.LogError("[StaplerItem] ❌ ERRO: bonecaImage ou bonecaGrampeadaSprite não atribuídos no Inspector!");
                Debug.LogError($"[StaplerItem] bonecaImage={bonecaImage}, bonecaGrampeadaSprite={bonecaGrampeadaSprite}");
            }

            // Desativa após o uso
            isActive = false;
            Debug.Log("[StaplerItem] Grampeador desativado após uso.");
        }
        else
        {
            Debug.Log($"[StaplerItem] ❌ Tag '{target.tag}' não corresponde a 'Boneca'. Nada a grampear aqui.");
        }
    }
}