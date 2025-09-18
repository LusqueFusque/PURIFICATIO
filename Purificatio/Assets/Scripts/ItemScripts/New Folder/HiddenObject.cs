using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false; // Começa invisível
    }

    public void Reveal()
    {
        sr.enabled = true; // Torna visível quando dentro da câmera
    }
}
