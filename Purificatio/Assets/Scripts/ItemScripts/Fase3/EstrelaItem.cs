using UnityEngine;

/// <summary>
/// Item coletável: Estrela (sem função por enquanto)
/// </summary>
public class EstrelaItem : MonoBehaviour
{
    public static EstrelaItem Instance;

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

    public void OnEstrelaCollected()
    {
        Debug.Log("[EstrelaItem] Estrela coletada!");
        // Função a ser implementada
    }
}