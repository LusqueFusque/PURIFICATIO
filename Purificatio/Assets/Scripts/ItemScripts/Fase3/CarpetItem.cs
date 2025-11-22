using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Lógica do TAPETE - Muda de PNG ao clicar
/// </summary>
public class CarpetItem : MonoBehaviour, IPointerClickHandler
{
    [Header("Sprites do Tapete")]
    [Tooltip("Sprite inicial (com pentagrama oculto)")]
    public Sprite spriteInitial;
    
    [Tooltip("Sprite após revelar (pentagrama visível)")]
    public Sprite spriteRevealed;

    [Header("Referência")]
    [Tooltip("Image do tapete que será alterada")]
    public Image carpetImage;

    private bool pentagramRevealed = false;
    private CanvasGroup canvasGroup; // ← Para controlar visibilidade

    void Start()
    {
        // Estado inicial: pentagrama oculto
        if (carpetImage != null && spriteInitial != null)
        {
            carpetImage.sprite = spriteInitial;
        }

        // ✅ Garante que tem CanvasGroup para controlar alpha
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        // ✅ Se DialogueManager está mostrando diálogo, mantém o tapete visível
        if (DialogueManager.Instance != null && DialogueManager.Instance.CurrentLine != null)
        {
            canvasGroup.alpha = 1f; // Sempre visível durante diálogo
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[CarpetItem] Tapete clicado!");

        if (pentagramRevealed)
        {
            Debug.Log("[CarpetItem] Pentágrama já foi revelado!");
            return;
        }

        RevealPentagram();
    }

    private void RevealPentagram()
    {
        pentagramRevealed = true;

        // Troca sprite
        if (carpetImage != null && spriteRevealed != null)
        {
            carpetImage.sprite = spriteRevealed;
            Debug.Log("[CarpetItem] ✓ Pentágrama revelado!");
        }

        // Completa missão
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission("RevealPentagram");
        }

        // Atualiza condicionais
        if (AdvancedMapManager.Instance != null)
        {
            AdvancedMapManager.Instance.RefreshAllConditionals();
        }
    }

    public bool IsPentagramRevealed() => pentagramRevealed;
}