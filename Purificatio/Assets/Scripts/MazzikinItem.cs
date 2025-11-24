using UnityEngine;
using UnityEngine.UI;

public class MazzikinItem : MonoBehaviour
{
    [Header("Referências")]
    public Image mazzikinImage;
    public RectTransform mazzikinRectTransform;

    [Header("Posição final")]
    public Vector3 targetPosition = new Vector3(-27f, 89f, 0f);

    private bool revealed = false;

    public void RevealMazzikin()
    {
        Debug.LogError("🔥🔥🔥 RevealMazzikin() FOI CHAMADO! 🔥🔥🔥");
        if (revealed) return;
        revealed = true;
        

        if (mazzikinImage == null || mazzikinRectTransform == null)
        {
            Debug.LogError("[MazzikinItem] Referências não atribuídas!");
            return;
        }

        mazzikinImage.gameObject.SetActive(true);
        mazzikinImage.enabled = true;
        mazzikinImage.color = Color.white;

        mazzikinRectTransform.anchoredPosition = new Vector2(targetPosition.x, targetPosition.y);
        mazzikinRectTransform.localScale = Vector3.one;
        mazzikinRectTransform.SetAsLastSibling();

        var handler = mazzikinImage.GetComponent<MazzikinClickHandler>();
        if (handler == null)
            handler = mazzikinImage.gameObject.AddComponent<MazzikinClickHandler>();

        // envia referências
        handler.SetArmaSanta(ArmaSantaItem.Instance);
        handler.SetHolyWater(HolyWaterItem.Instance);

        mazzikinImage.raycastTarget = true;

        Debug.Log("[MazzikinItem] Mazzi ativado, posicionado e handler configurado.");
    }

    public bool IsRevealed() => revealed;
}