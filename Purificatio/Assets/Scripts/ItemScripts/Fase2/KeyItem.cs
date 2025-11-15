using UnityEngine;
using UnityEngine.UI;

public class KeyItem : MonoBehaviour
{
    public Image keyImage;
    public Sprite brokenKeySprite;
    public Sprite fixedKeySprite;

    private bool collected = false;
    private bool fixedKey = false;

    void Start()
    {
        // comece como chave quebrada
        keyImage.sprite = brokenKeySprite;
        keyImage.gameObject.SetActive(false);
    }

    public void ShowKey()
    {
        keyImage.gameObject.SetActive(true);
    }

    public void OnClick()
    {
        if (!collected)
        {
            collected = true;
            MissionManager.Instance.CompleteMission("FindKey");
            return;
        }

        // Se já tem chiclete, conserta chave
        if (!fixedKey && MissionManager.Instance.IsCompleted("FixKey"))
        {
            fixedKey = true;
            keyImage.sprite = fixedKeySprite;
            return;
        }

        // Usar chave consertada para abrir o baú
        if (fixedKey)
        {
            MissionManager.Instance.CompleteMission("OpenChest");
        }
    }
}