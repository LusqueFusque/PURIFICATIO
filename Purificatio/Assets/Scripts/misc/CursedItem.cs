using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class CursedItem : MonoBehaviour
{
    public bool isCursed = true;
    
    [Header("Identificação Especial")]
    [Tooltip("Marque como true se este é o item do Mazzi")]
    public bool isMazziItem = false;

    // Evento que notifica quando o item é purificado
    public static event Action<CursedItem> OnItemPurified;

    public void Purify()
    {
        if (!isCursed) return;

        isCursed = false;
        Debug.Log($"[CursedItem] {name} purificado! IsMazziItem: {isMazziItem}");
        
        // Notifica todos os ouvintes que este item foi purificado
        OnItemPurified?.Invoke(this);
        
        // Desativa o GameObject só depois de notificar
        gameObject.SetActive(false);
    }
}