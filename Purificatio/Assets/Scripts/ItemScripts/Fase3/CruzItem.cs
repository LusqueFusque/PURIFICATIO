using UnityEngine;

/// <summary>
/// Item coletável: Cruz (sem função por enquanto)
/// </summary>
public class CruzItem : MonoBehaviour
{
    public static CruzItem Instance;

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

    public void OnCruzCollected()
    {
        Debug.Log("[CruzItem] Cruz coletada!");
        // Função a ser implementada
    }
}