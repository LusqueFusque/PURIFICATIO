using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite cursedSprite;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = normalSprite;   // começa normal
    }

    public void ShowCursed()
    {
        sr.sprite = cursedSprite;
    }

    public void ShowNormal()
    {
        sr.sprite = normalSprite;
    }
}