using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Carta clicável que mostra a imagem e depois abre o diálogo
/// </summary>
public class CartaClickable : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuração da Carta")]
    [Tooltip("Image da carta que será exibida (ex: CartaPlaceholder)")]
    public GameObject cartaImagePanel;

    [Tooltip("ID do diálogo a abrir após fechar a carta")]
    public string dialogueNodeId = "carta1";

    private bool cartaAberta = false;

    void Start()
    {
        // Carta começa fechada
        if (cartaImagePanel != null)
        {
            cartaImagePanel.SetActive(false);
        }
    }

    void Update()
    {
        // Se a carta está aberta e clicou em qualquer lugar, fecha
        if (cartaAberta && Input.GetMouseButtonDown(0))
        {
            FecharCarta();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[CartaClickable] Carta clicada! Abrindo imagem...");
        AbrirCarta();
    }

    private void AbrirCarta()
    {
        if (cartaImagePanel != null)
        {
            cartaImagePanel.SetActive(true);
            cartaAberta = true;
            Debug.Log("[CartaClickable] Imagem da carta exibida. Clique em qualquer lugar para fechar.");
        }
    }

    private void FecharCarta()
    {
        if (cartaImagePanel != null)
        {
            cartaImagePanel.SetActive(false);
        }

        cartaAberta = false;
        Debug.Log("[CartaClickable] Carta fechada. Abrindo diálogo...");

        // Abre o diálogo sobre a carta
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.GoToNode(dialogueNodeId);
        }
    }
}