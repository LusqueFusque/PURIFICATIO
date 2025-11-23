using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fase3MissionHandler : MissionHandlerBase
{
    [Header("UI da Fase 3")]
    public Image pentagramImage;       // UI Image
    public Image cursedItemGlow;       // UI Image (aura UI, se houver)
    public Image mazzikinImage;        // UI Image do Mazzi que vai aparecer
    
    [Header("Sprite (única exceção)")]
    public GameObject tapeteAuraSprite; // SpriteRenderer já existente

    [Header("Entidade")]
    public GameObject demonMazzi;       // GameObject comum

    [Header("Sons")]
    public AudioClip revealSound;
    public AudioClip saltSound;
    public AudioClip demonAppearSound;

    [Header("Configurações de Fade")]
    public float mazziFadeDuration = 1.5f;

    void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;
    }

    void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;
    }

    private void OnMissionCompletedHandler(string missionId)
    {
        Debug.Log($"[Fase3] Missão completa: {missionId}");

        switch (missionId)
        {
            case "RevealPentagram":
                StartCoroutine(RevealPentagramSequence());
                break;

            case "useSalt":
                StartCoroutine(SaltSequence());
                break;

            case "SaltMazzi":
                StartCoroutine(SaltMazziSequence());
                break;

            case "SummonMazzi":
                StartCoroutine(SummonMazziSequence());
                break;
        }
    }

    public override void HandleMission(string missionId)
    {
        switch (missionId)
        {
            case "RevealPentagram":
                MissionManager.Instance.StartMission("RevealPentagram");
                break;

            case "SaltMazzi":
                MissionManager.Instance.StartMission("SaltMazzi");
                break;

            case "SummonMazzi":
                MissionManager.Instance.StartMission("SummonMazzi");
                break;

            case "fadeIn":  
                StartCoroutine(FadeInSequence());
                break;

            default:
                Debug.LogWarning($"[Fase3] Missão desconhecida: {missionId}");
                break;
        }
    }

    // ============================================================
    // FADE IN — NÃO ALTERADO — COMPATÍVEL COM TODA A FASE
    // ============================================================
    private IEnumerator FadeInSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();

        if (vfx != null)
            yield return vfx.FadeFromBlack(2f);
        else
            yield return new WaitForSeconds(2f);

        CompleteMission("fadeIn");

        yield return null;

        DialogueManager.Instance.ShowNextLine();
    }

    // ============================================================
    // REVELAR PENTAGRAMA
    // ============================================================
    private IEnumerator RevealPentagramSequence()
    {
        if (revealSound != null)
            AudioSource.PlayClipAtPoint(revealSound, Camera.main.transform.position, 0.5f);

        // UI Image
        if (pentagramImage != null)
            pentagramImage.gameObject.SetActive(true);

        // UI aura (se existir)
        if (cursedItemGlow != null)
            cursedItemGlow.gameObject.SetActive(true);

        // SpriteRenderer (única exceção)
        if (tapeteAuraSprite != null)
            tapeteAuraSprite.SetActive(true);

        yield return null;

        DialogueManager.Instance.ShowNextLine();
    }

    // ============================================================
    // SAL NO ITEM AMALDIÇOADO GENÉRICO
    // ============================================================
    private IEnumerator SaltSequence()
    {
        if (saltSound != null)
            AudioSource.PlayClipAtPoint(saltSound, Camera.main.transform.position, 0.5f);

        yield return new WaitForSeconds(0.2f);

        // UI Image
        if (cursedItemGlow != null)
            cursedItemGlow.gameObject.SetActive(false);

        // SpriteRenderer
        if (tapeteAuraSprite != null)
            tapeteAuraSprite.SetActive(false);

        yield return null;

        DialogueManager.Instance.ShowNextLine();
    }

    // ============================================================
    // SAL NO MAZZI - NOVA SEQUÊNCIA
    // ============================================================
    private IEnumerator SaltMazziSequence()
    {
        if (saltSound != null)
            AudioSource.PlayClipAtPoint(saltSound, Camera.main.transform.position, 0.5f);

        yield return new WaitForSeconds(0.2f);

        // Remove auras/glows do item amaldiçoado
        if (cursedItemGlow != null)
            cursedItemGlow.gameObject.SetActive(false);

        if (tapeteAuraSprite != null)
            tapeteAuraSprite.SetActive(false);

        // Fade in gradual do Mazzi
        if (mazzikinImage != null)
        {
            mazzikinImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeInMazzi());
        }

        // Toca som de aparição do demônio (opcional)
        if (demonAppearSound != null)
            AudioSource.PlayClipAtPoint(demonAppearSound, Camera.main.transform.position, 0.6f);

        yield return new WaitForSeconds(0.3f);

        // Avança para próxima linha de diálogo
        DialogueManager.Instance.ShowNextLine();
    }

    // ============================================================
    // FADE IN DO MAZZI
    // ============================================================
    private IEnumerator FadeInMazzi()
    {
        if (mazzikinImage == null) yield break;

        CanvasGroup canvasGroup = mazzikinImage.GetComponent<CanvasGroup>();
        
        // Se não houver CanvasGroup, adiciona um
        if (canvasGroup == null)
        {
            canvasGroup = mazzikinImage.gameObject.AddComponent<CanvasGroup>();
        }

        // Começa invisível
        canvasGroup.alpha = 0f;

        float elapsed = 0f;
        
        while (elapsed < mazziFadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / mazziFadeDuration);
            yield return null;
        }

        // Garante que termina com alpha 1
        canvasGroup.alpha = 1f;
    }

    // ============================================================
    // INVOCAR MAZZI (mantido para compatibilidade)
    // ============================================================
    private IEnumerator SummonMazziSequence()
    {
        if (demonAppearSound != null)
            AudioSource.PlayClipAtPoint(demonAppearSound, Camera.main.transform.position, 0.8f);

        yield return new WaitForSeconds(0.4f);

        if (demonMazzi != null)
            demonMazzi.SetActive(true);

        yield return null;

        DialogueManager.Instance.ShowNextLine();
    }
}