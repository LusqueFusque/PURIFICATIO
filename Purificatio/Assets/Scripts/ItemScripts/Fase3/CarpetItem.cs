using UnityEngine;
using UnityEngine.UI;
public class CarpetItem : MonoBehaviour
{
    [Header("Sprites do Tapete")]
    public Sprite spriteRevealed;

    [Header("Referências")]
    public Button carpetButton;                 // TapeteImage (botão clicável)
    public Image carpetCopy;                    // Cópia fora do HUD
    public RectTransform carpetCopyRectTransform;

    [Header("Movimento")]
    public Vector3 targetPosition = new Vector3(263, -335, 0);

    private bool pentagramRevealed = false;

    void Start()
    {
        if (carpetButton != null)
            carpetButton.onClick.AddListener(OnCarpetClicked);
        else
            Debug.LogError("[CarpetItem] Botão não atribuído!");
    }

    private void OnCarpetClicked()
    {
        if (pentagramRevealed)
            return;

        RevealPentagram();
    }

    private void RevealPentagram()
    {
        pentagramRevealed = true;

        // Troca o sprite do TapeteImage
        Image btnImg = carpetButton.GetComponent<Image>();
        if (btnImg != null && spriteRevealed != null)
            btnImg.sprite = spriteRevealed;

        // Move o próprio TapeteImage (o botão)
        RectTransform buttonRect = carpetButton.GetComponent<RectTransform>();
        if (buttonRect != null)
        {
            buttonRect.localPosition = targetPosition;
            Debug.Log("[CarpetItem] TapeteImage movido para posição final!");
        }

        // Destrói a cópia que estava fora do painel
        if (carpetCopy != null)
        {
            Destroy(carpetCopy.gameObject);
            Debug.Log("[CarpetItem] Cópia destruída!");
        }

        // Atualiza sistemas externos (caso existam)
        MissionManager.Instance?.CompleteMission("RevealPentagram");
        AdvancedMapManager.Instance?.RefreshAllConditionals();
    }

    public bool IsPentagramRevealed() => pentagramRevealed;
}
