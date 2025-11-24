using UnityEngine;
using UnityEngine.UI;

public class MazzikinItem : MonoBehaviour
{
    [Header("Referências")]
    public Image mazzikinImage;                    // UI Image do Mazzi
    public RectTransform mazzikinRectTransform;   // RectTransform da imagem

    [Header("Posição final")]
    public Vector3 targetPosition = new Vector3(-27f, 89f, 0f);

    private bool revealed = false;

    public void RevealMazzikin()
    {
        Debug.LogError("🔥🔥🔥 RevealMazzikin() FOI CHAMADO! 🔥🔥🔥");

        if (revealed)
            return;

        revealed = true;

        if (mazzikinImage == null || mazzikinRectTransform == null)
        {
            Debug.LogError("[MazzikinItem] Referências não atribuídas!");
            return;
        }

        // Ativa o objeto
        mazzikinImage.gameObject.SetActive(true);
        mazzikinImage.enabled = true;
        mazzikinImage.color = Color.white;

        // Ajusta posição
        mazzikinRectTransform.anchoredPosition = new Vector2(targetPosition.x, targetPosition.y);
        mazzikinRectTransform.localScale = Vector3.one;
        mazzikinRectTransform.SetAsLastSibling();

        Debug.Log("[MazzikinItem] Mazzi ativado e posicionado!");
        
        // Depois de ativar o GameObject, garanta o link
        if (ArmaSantaItem.Instance != null)
        {
            var handler = mazzikinImage.GetComponent<MazzikinClickHandler>();
            if (handler == null)
                handler = mazzikinImage.gameObject.AddComponent<MazzikinClickHandler>();

            handler.SetArmaSanta(ArmaSantaItem.Instance);

            Debug.Log("[MazzikinItem] ArmaSanta vinculada ao Mazzikin após revelar.");
        }
        
    }

    public bool IsRevealed() => revealed;
}