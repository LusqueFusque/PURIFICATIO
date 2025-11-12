using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableAreaUI : MonoBehaviour, IPointerClickHandler
{
    // ajuste os nomes de tag conforme seu projeto
    public string woodTag = "WoodLoose";
    public string dollTag = "Boneca"; // ou "BonecaImage" - use a mesma tag do StaplerItem

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[ClickableAreaUI] Clique detectado em {gameObject.name} (tag={gameObject.tag})");

        // madeira solta -> Crowbar
        if (gameObject.CompareTag(woodTag))
        {
            if (CrowbarItem.Instance != null)
            {
                Debug.Log("[ClickableAreaUI] Roteando para CrowbarItem.Instance.TryUseOn");
                CrowbarItem.Instance.TryUseOn(gameObject);
            }
            else
            {
                Debug.LogWarning("[ClickableAreaUI] CrowbarItem.Instance == null");
            }
            return;
        }

        // boneca -> Stapler
        if (gameObject.CompareTag(dollTag))
        {
            if (StaplerItem.Instance != null)
            {
                Debug.Log("[ClickableAreaUI] Roteando para StaplerItem.Instance.TryUseOn");
                StaplerItem.Instance.TryUseOn(gameObject);
            }
            else
            {
                Debug.LogWarning("[ClickableAreaUI] StaplerItem.Instance == null");
            }
            return;
        }

        Debug.Log("[ClickableAreaUI] Clique em objeto sem tratamento específico.");
    }
}