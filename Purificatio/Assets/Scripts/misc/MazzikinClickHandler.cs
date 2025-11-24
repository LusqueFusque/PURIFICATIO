using UnityEngine;
using UnityEngine.EventSystems;

public class MazzikinClickHandler : MonoBehaviour, IPointerClickHandler
{
    private ArmaSantaItem armaSanta;
    private HolyWaterItem holyWater;

    public void SetArmaSanta(ArmaSantaItem a)
    {
        armaSanta = a;
    }

    public void SetHolyWater(HolyWaterItem h)
    {
        holyWater = h;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool santaAtiva = armaSanta != null && armaSanta.IsActive();
        bool aguaBentaAtiva = holyWater != null && holyWater.IsActive();

        Debug.Log("[MazziClick] santaAtiva=" + santaAtiva + " | aguaBentaAtiva=" + aguaBentaAtiva);

        if (!santaAtiva && !aguaBentaAtiva)
        {
            Debug.Log("[MazziClick] Nenhum item ativo — bloqueado.");
            return;
        }

        // Arma Santa tem prioridade
        if (santaAtiva)
        {
            Debug.Log("[MazziClick] Exorcismo via ArmaSanta!");
            armaSanta.ExorcizeMazzi();
            return;
        }

        // Holy Water
        if (aguaBentaAtiva)
        {
            Debug.Log("[MazziClick] Exorcismo via HolyWater!");
            holyWater.UseHolyWater();
            return;
        }
    }
}