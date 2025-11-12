using UnityEngine;
using UnityEngine.UI;

public class CrowbarItem : MonoBehaviour
{
    public static CrowbarItem Instance;

    public bool isActive = false;
    public Image salaPanel;
    public Sprite madeiraRemovidaSprite;
    public GameObject bonecaImage;
    private bool madeiraRemovida = false;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (isActive && Input.GetMouseButtonDown(1))
        {
            isActive = false;
            Debug.Log("[ItemCrowbar] Crowbar desativado (right click).");
        }
    }

    public void OnItemClicked()
    {
        isActive = !isActive;
        Debug.Log("[ItemCrowbar] Crowbar " + (isActive ? "ATIVADO" : "DESATIVADO") + " via OnItemClicked.");
    }

    public void TryUseOn(GameObject target)
    {
        Debug.Log("[ItemCrowbar] TryUseOn chamado. isActive=" + isActive + " target=" + (target? target.name : "null"));

        if (!isActive)
        {
            Debug.Log("[ItemCrowbar] Ignorando: crowbar não ativo.");
            return;
        }

        if (target.CompareTag("WoodLoose") && !madeiraRemovida)
        {
            if (salaPanel != null && madeiraRemovidaSprite != null)
                salaPanel.sprite = madeiraRemovidaSprite;

            madeiraRemovida = true;
            isActive = false;
            Debug.Log("[ItemCrowbar] Madeira removida.");

            // desativa clickareas
            var clickables = GameObject.FindGameObjectsWithTag("WoodLoose");
            foreach (var go in clickables) go.SetActive(false);

            if (bonecaImage != null) bonecaImage.SetActive(true);

            if (AdvancedMapManager.Instance != null)
                AdvancedMapManager.Instance.SetGlobalFlag("WoodRemoved", true);
        }
        else
        {
            Debug.Log("[ItemCrowbar] Nada a fazer aqui.");
        }
    }
}