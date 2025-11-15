using UnityEngine;
using UnityEngine.UI;

public class GumItem : MonoBehaviour
{
    public Image gumImage;

    private bool collected = false;

    public void OnClick()
    {
        if (collected) return;

        collected = true;
        gumImage.gameObject.SetActive(false);

        MissionManager.Instance.CompleteMission("FixKey");
    }
}