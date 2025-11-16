using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lógica da CHAVE - Quebra no cenário, chiclete conserta no cenário
/// </summary>
public class KeyItem : MonoBehaviour
{
    public static KeyItem Instance;

    [Header("Painel da Cena - Baú e Chave")]
    [Tooltip("Image do painel do baú (troca sprite 3 vezes)")]
    public Image bauPanelImage;

    [Header("Sprites do Painel (3 estados)")]
    [Tooltip("Baú fechado (estado inicial)")]
    public Sprite spriteChestClosed;
    
    [Tooltip("Baú com chave quebrada (após tentar usar chave)")]
    public Sprite spriteChestWithBrokenKey;
    
    [Tooltip("Baú aberto com lâmpada (após usar chiclete)")]
    public Sprite spriteChestOpen;

    [Header("Objetos Condicionais")]
    [Tooltip("Image da lâmpada (ativa após abrir baú)")]
    public GameObject lampCollectibleImage;

    [Header("Áudio")]
    public AudioClip keyBreakSound;
    public AudioClip chestOpenSound;

    private enum KeyState { NotCollected, Collected, BrokenInChest }
    private KeyState currentState = KeyState.NotCollected;

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

    void Start()
    {
        // Estado inicial: baú fechado, lâmpada invisível
        if (bauPanelImage != null && spriteChestClosed != null)
            bauPanelImage.sprite = spriteChestClosed;

        if (lampCollectibleImage != null)
            lampCollectibleImage.SetActive(false);
    }

    void Update()
    {
        if (isActive && Input.GetMouseButtonDown(1))
        {
            Deactivate();
        }
    }

    public void Activate()
    {
        Debug.Log($"[KeyItem] Chave ATIVADA (Estado: {currentState})");
        isActive = true;
    }

    public void Deactivate()
    {
        Debug.Log("[KeyItem] Chave DESATIVADA");
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

    // Chamado quando chave é coletada
    public void OnKeyCollected()
    {
        Debug.Log("[KeyItem] OnKeyCollected - Chave adicionada ao inventário");
        currentState = KeyState.Collected;
    }

    // Chamado ao clicar no baú com chave ativa
    public void UseKey()
    {
        Debug.Log($"[KeyItem] UseKey chamado! Estado: {currentState}, Ativo: {isActive}");

        if (!isActive)
        {
            Debug.Log("[KeyItem] Chave não está ativa!");
            return;
        }

        if (currentState == KeyState.Collected)
        {
            BreakKeyInChest();
        }
        else if (currentState == KeyState.BrokenInChest)
        {
            Debug.Log("[KeyItem] ⚠️ Chave já está quebrada no baú! Use chiclete no cenário.");
        }
        else
        {
            Debug.Log("[KeyItem] ⚠️ Você não tem a chave!");
        }
    }

    private void BreakKeyInChest()
    {
        currentState = KeyState.BrokenInChest;

        // Som
        if (keyBreakSound != null)
            AudioSource.PlayClipAtPoint(keyBreakSound, Camera.main.transform.position, 0.5f);

        // Muda cenário para mostrar chave quebrada NO BAÚ
        if (bauPanelImage != null && spriteChestWithBrokenKey != null)
        {
            bauPanelImage.sprite = spriteChestWithBrokenKey;
            Debug.Log("[KeyItem] ✓ Sprite trocado: baú com chave quebrada");
        }

        // Remove chave do inventário (foi quebrada)
        RemoveKeyFromInventory();

        Debug.Log("[KeyItem] ✗ Chave quebrou no baú! Use chiclete no cenário para consertar.");
        
        Deactivate();
    }

    private void RemoveKeyFromInventory()
    {
        DynamicInventory inventory = FindObjectOfType<DynamicInventory>();
        if (inventory == null) return;

        foreach (var slot in inventory.slots)
        {
            if (!slot.gameObject.activeSelf) continue;

            var tmpText = slot.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpText != null && (tmpText.text.Contains("Chave") || tmpText.text.Contains("Key")))
            {
                slot.gameObject.SetActive(false);
                slot.onClick.RemoveAllListeners();
                
                inventory.items.RemoveAll(item => 
                    item.itemName == "Chave" || item.itemName == "Key");
                
                Debug.Log("[KeyItem] ✓ Chave removida do inventário");
                break;
            }
        }
    }

    // Chamado por GumItem quando chiclete é usado
    public void RepairWithGum()
    {
        Debug.Log("[KeyItem] RepairWithGum chamado!");

        if (currentState != KeyState.BrokenInChest)
        {
            Debug.Log("[KeyItem] ⚠️ Chave não está quebrada no baú!");
            return;
        }

        OpenChest();
    }

    private void OpenChest()
    {
        // Som
        if (chestOpenSound != null)
            AudioSource.PlayClipAtPoint(chestOpenSound, Camera.main.transform.position, 0.6f);

        // Troca sprite: baú aberto com lâmpada
        if (bauPanelImage != null && spriteChestOpen != null)
        {
            bauPanelImage.sprite = spriteChestOpen;
            Debug.Log("[KeyItem] ✓ Sprite trocado: baú aberto");
        }

        // Ativa lâmpada coletável
        if (lampCollectibleImage != null)
        {
            lampCollectibleImage.SetActive(true);
            Debug.Log("[KeyItem] ✓ Lâmpada agora visível para coleta");
        }

        if (AdvancedMapManager.Instance != null)
            AdvancedMapManager.Instance.SetGlobalFlag("ChestOpened", true);

        Debug.Log("[KeyItem] ✓✓ Baú aberto com sucesso!");
    }

    public bool IsBrokenInChest() => currentState == KeyState.BrokenInChest;
}