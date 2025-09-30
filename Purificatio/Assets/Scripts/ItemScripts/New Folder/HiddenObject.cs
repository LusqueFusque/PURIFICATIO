using UnityEngine;
using UnityEngine.UI;

public class HiddenObject : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite cursedSprite;

    private Image sr;

    void Awake()
    {
        sr = GetComponent<Image>();
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