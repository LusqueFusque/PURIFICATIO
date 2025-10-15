using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CursedItem : MonoBehaviour
{
    public bool isCursed = true;

    public void Purify()
    {
        if (!isCursed) return;

        isCursed = false;
        gameObject.SetActive(false); // ou troque o sprite
        Debug.Log($"[CursedItem] {name} purificado!");
    }
}