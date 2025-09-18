using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false; // Come�a invis�vel
    }

    public void Reveal()
    {
        sr.enabled = true; // Torna vis�vel quando dentro da c�mera
    }
}
