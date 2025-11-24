using UnityEngine;
using UnityEngine.EventSystems;

public class MazzikinClickHandler : MonoBehaviour, IPointerClickHandler
{
    private ArmaSantaItem armaSanta;

    public void SetArmaSanta(ArmaSantaItem a)
    {
        armaSanta = a;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[MazziClick] Clique chegou!");

        Debug.Log("[MazziClick] armaSanta == null ? " + (armaSanta == null));
    
        if (armaSanta != null)
            Debug.Log("[MazziClick] armaSanta.IsActive() = " + armaSanta.IsActive());
        else
            Debug.Log("[MazziClick] armaSanta está NULL!");

        if (armaSanta == null || !armaSanta.IsActive())
        {
            Debug.Log("[MazziClick] BLOQUEADO — ArmaSanta não ativa.");
            return;
        }

        Debug.Log("[MazziClick] Chamando exorcismo...");
        armaSanta.ExorcizeMazzi();
        
    }
}