using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Handler de clique para MazzikinImage
/// Ativado apenas quando ArmaSanta está equipada
/// </summary>
public class MazzikinClickHandler : MonoBehaviour, IPointerClickHandler
{
    private ArmaSantaItem armaSanta;

    public void SetArmaSanta(ArmaSantaItem arma)
    {
        armaSanta = arma;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[MazzikinClickHandler] Mazzi foi clicado!");

        if (armaSanta == null)
        {
            Debug.LogWarning("[MazzikinClickHandler] ArmaSanta não configurada!");
            return;
        }

        if (!armaSanta.IsActive())
        {
            Debug.Log("[MazzikinClickHandler] ArmaSanta não está ativa!");
            return;
        }

        // Exorciza o Mazzi
        armaSanta.ExorcizeMazzi();
    }
}