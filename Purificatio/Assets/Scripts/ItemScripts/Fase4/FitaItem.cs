using UnityEngine;

public class FitaItem : MonoBehaviour
{
    public static FitaItem Instance;

    private bool isActive = false;

    [Header("ItemData (ScriptableObject)")]
    public ItemData FitaData;   // referência ao ScriptableObject da fita

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
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Chamado quando o jogador coleta a fita no cenário.
    /// </summary>
    public void OnFitaCollected()
    {
        Debug.Log("[FitaItem] Fita coletada!");
        var inv = FindObjectOfType<DynamicInventory>();
        if (inv != null && FitaData != null)
        {
            inv.AddItem(FitaData);
        }
        else
        {
            Debug.LogError("[FitaItem] Inventory ou ItemData não configurados!");
        }

        // desativa o objeto físico da cena
        gameObject.SetActive(false);

        // opcional: marcar missão concluída
        if (MissionManager.Instance != null)
            MissionManager.Instance.CompleteMission("findTape");
    }

    /// <summary>
    /// Ativa o item no inventário para uso (ex: clicar no botão da fita).
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
        Debug.Log("[FitaItem] Estado ativo = " + isActive);
    }

    /// <summary>
    /// Verifica se a fita está ativa para uso.
    /// </summary>
    public bool IsActive()
    {
        return isActive;
    }
}