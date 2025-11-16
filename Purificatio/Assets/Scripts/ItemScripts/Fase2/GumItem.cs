using UnityEngine;

/// <summary>
/// Lógica do CHICLETE - Usa diretamente no cenário para consertar chave e abrir baú
/// </summary>
public class GumItem : MonoBehaviour
{
    public static GumItem Instance;

    [Header("Áudio")]
    public AudioClip useGumSound;

    private bool gumUsed = false;

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

    // Chamado ao clicar no botão do chiclete no inventário
    public void UseGum()
    {
        Debug.Log("[GumItem] UseGum chamado!");

        if (gumUsed)
        {
            Debug.Log("[GumItem] ⚠️ Chiclete já foi usado!");
            return;
        }

        // Verifica se a chave está quebrada NO BAÚ (no cenário)
        if (KeyItem.Instance == null || !KeyItem.Instance.IsBrokenInChest())
        {
            Debug.Log("[GumItem] ⚠️ A chave não está quebrada no baú!");
            return;
        }

        gumUsed = true;

        // Som
        if (useGumSound != null)
            AudioSource.PlayClipAtPoint(useGumSound, Camera.main.transform.position, 0.5f);

        // Conserta a chave E abre o baú
        KeyItem.Instance.RepairWithGum();

        // Remove chiclete do inventário
        RemoveFromInventory();

        Debug.Log("[GumItem] ✓✓ Chiclete usado! Baú aberto!");
    }

    private void RemoveFromInventory()
    {
        DynamicInventory inventory = FindObjectOfType<DynamicInventory>();
        if (inventory == null) return;

        foreach (var slot in inventory.slots)
        {
            if (!slot.gameObject.activeSelf) continue;

            var tmpText = slot.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpText != null && (tmpText.text.Contains("Chiclete") || tmpText.text.Contains("Gum")))
            {
                slot.gameObject.SetActive(false);
                slot.onClick.RemoveAllListeners();
                
                inventory.items.RemoveAll(item => 
                    item.itemName == "Chiclete" || item.itemName == "Gum");
                
                Debug.Log("[GumItem] ✓ Chiclete removido do inventário");
                break;
            }
        }
    }

    public bool IsUsed() => gumUsed;
}