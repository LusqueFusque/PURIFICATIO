using UnityEngine;

/// <summary>
/// Item coletável: Crescente (sem função por enquanto)
/// </summary>
public class CrescenteItem : MonoBehaviour
{
    public static CrescenteItem Instance;

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

    public void OnCrescenteCollected()
    {
        Debug.Log("[CrescenteItem] Crescente coletado!");
        // Função a ser implementada
    }
}