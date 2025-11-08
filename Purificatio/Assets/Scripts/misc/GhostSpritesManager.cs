using UnityEngine;

/// <summary>
/// Gerencia o sprite do fantasma que fica visível na cena durante diálogos.
/// Diferente dos humanos, fantasmas não aparecem na UI lateral.
/// </summary>
public class GhostSpriteManager : MonoBehaviour
{
    public static GhostSpriteManager Instance;

    [Header("Configuração")]
    public SpriteRenderer ghostSpriteRenderer; // O sprite do fantasma na cena
    public float fadeSpeed = 2f;

    private Color originalColor;
    private bool isVisible = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (ghostSpriteRenderer == null)
        {
            ghostSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (ghostSpriteRenderer != null)
        {
            originalColor = ghostSpriteRenderer.color;
            HideImmediate(); // Começa invisível
        }
    }

    /// <summary>
    /// Mostra o fantasma gradualmente quando fala
    /// </summary>
    public void Show()
    {
        if (ghostSpriteRenderer == null) return;

        gameObject.SetActive(true);
        isVisible = true;
        
        // Pode adicionar fade in aqui se quiser
        Color targetColor = originalColor;
        targetColor.a = 1f;
        ghostSpriteRenderer.color = targetColor;
        
        Debug.Log($"[GhostSpriteManager] Fantasma '{gameObject.name}' mostrado.");
    }

    /// <summary>
    /// Esconde o fantasma gradualmente quando para de falar
    /// </summary>
    public void Hide()
    {
        if (ghostSpriteRenderer == null) return;

        isVisible = false;
        
        // Pode adicionar fade out aqui se quiser
        Color targetColor = originalColor;
        targetColor.a = 0f;
        ghostSpriteRenderer.color = targetColor;
        
        Debug.Log($"[GhostSpriteManager] Fantasma '{gameObject.name}' escondido.");
    }

    /// <summary>
    /// Esconde instantaneamente (para inicialização)
    /// </summary>
    public void HideImmediate()
    {
        if (ghostSpriteRenderer == null) return;

        Color transparent = originalColor;
        transparent.a = 0f;
        ghostSpriteRenderer.color = transparent;
        isVisible = false;
    }

    /// <summary>
    /// Muda a opacidade do fantasma (útil para efeitos)
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (ghostSpriteRenderer == null) return;

        Color newColor = ghostSpriteRenderer.color;
        newColor.a = Mathf.Clamp01(alpha);
        ghostSpriteRenderer.color = newColor;
    }

    /// <summary>
    /// Faz o fantasma piscar (efeito assustador)
    /// </summary>
    public void Flicker(int times = 3)
    {
        StartCoroutine(FlickerRoutine(times));
    }

    private System.Collections.IEnumerator FlickerRoutine(int times)
    {
        for (int i = 0; i < times; i++)
        {
            SetAlpha(0f);
            yield return new WaitForSeconds(0.1f);
            SetAlpha(1f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public bool IsVisible => isVisible;
}