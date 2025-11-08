using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Gerencia efeitos visuais compartilhados por todas as fases.
/// Tela vermelha, fade to black, shake, etc.
/// </summary>
public class VisualEffectsManager : MonoBehaviour
{
    public static VisualEffectsManager Instance;

    [Header("Overlays")]
    public Image blackScreen; // Overlay preto para fades
    public Image redScreen;   // Overlay vermelho para sustos

    [Header("Camera")]
    public Camera mainCamera;

    private Vector3 originalCameraPosition;
    private Coroutine shakeCoroutine;

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

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
            originalCameraPosition = mainCamera.transform.position;

        // Inicializa overlays como invisíveis
        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 0f;
            blackScreen.color = c;
            blackScreen.gameObject.SetActive(true);
        }

        if (redScreen != null)
        {
            Color c = redScreen.color;
            c.a = 0f;
            redScreen.color = c;
            redScreen.gameObject.SetActive(true);
        }
    }

    // ==================== FADE TO BLACK ====================
    public IEnumerator FadeToBlack(float duration)
    {
        if (blackScreen == null) yield break;

        float elapsed = 0f;
        Color c = blackScreen.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            blackScreen.color = c;
            yield return null;
        }

        c.a = 1f;
        blackScreen.color = c;
    }

    // ==================== FADE FROM BLACK ====================
    public IEnumerator FadeFromBlack(float duration)
    {
        if (blackScreen == null) yield break;

        float elapsed = 0f;
        Color c = blackScreen.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            blackScreen.color = c;
            yield return null;
        }

        c.a = 0f;
        blackScreen.color = c;
    }

    // ==================== RED SCREEN EFFECT ====================
    public void RedScreenEffect(float duration)
    {
        StartCoroutine(RedScreenCoroutine(duration));
    }

    private IEnumerator RedScreenCoroutine(float duration)
    {
        if (redScreen == null) yield break;

        // Fade in rápido
        float elapsed = 0f;
        Color c = redScreen.color;

        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 0.7f, elapsed / 0.3f);
            redScreen.color = c;
            yield return null;
        }

        c.a = 0.7f;
        redScreen.color = c;

        // Mantém
        yield return new WaitForSeconds(duration - 0.6f);

        // Fade out
        elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0.7f, 0f, elapsed / 0.3f);
            redScreen.color = c;
            yield return null;
        }

        c.a = 0f;
        redScreen.color = c;
    }

    public void ClearRedScreen()
    {
        if (redScreen != null)
        {
            Color c = redScreen.color;
            c.a = 0f;
            redScreen.color = c;
        }
    }

    // ==================== SCREEN SHAKE ====================
    public void ScreenShake(float intensity = 0.3f, float duration = 0.5f)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        if (mainCamera == null) yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition;
    }

    // ==================== FLASH ====================
    public void FlashWhite(float duration = 0.2f)
    {
        StartCoroutine(FlashCoroutine(Color.white, duration));
    }

    private IEnumerator FlashCoroutine(Color flashColor, float duration)
    {
        if (blackScreen == null) yield break;

        blackScreen.color = flashColor;

        yield return new WaitForSeconds(duration);

        Color c = flashColor;
        c.a = 0f;
        blackScreen.color = c;
    }
}