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
        // Escuta eventos do MissionManager
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;
        
        // Escuta eventos de purificação de itens amaldiçoados
        CursedItem.OnItemPurified += OnItemPurifiedHandler;
    }

    void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;
        
        CursedItem.OnItemPurified -= OnItemPurifiedHandler;
    }

    // ============================================================
    // HANDLER DE PURIFICAÇÃO - RECEBE NOTIFICAÇÃO DO CURSEDITEM
    // ============================================================
    private void OnItemPurifiedHandler(CursedItem cursedItem)
    {
        Debug.Log($"[Fase3] Item purificado recebido: {cursedItem.name}, IsMazziItem: {cursedItem.isMazziItem}");

        // Verifica se a missão SaltMazzi está ativa
        bool isMazziMission = MissionManager.Instance != null && 
                             MissionManager.Instance.IsActive("SaltMazzi");

        if (isMazziMission && cursedItem.isMazziItem)
        {
            Debug.Log("[Fase3] ✓ Iniciando sequência do Mazzi!");
            StartCoroutine(SaltMazziSequence());
        }
        else
        {
            Debug.Log("[Fase3] Iniciando sequência genérica de sal");
            StartCoroutine(SaltSequence());
        }
    }

    private void OnMissionCompletedHandler(string missionId)
    {
        Debug.Log($"[Fase3] Missão completa: {missionId}");

        switch (missionId)
        {
            case "RevealPentagram":
                StartCoroutine(RevealPentagramSequence());
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
                Debug.Log("[Fase3] Missão SaltMazzi iniciada - aguardando uso do sal");
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

        // Completa missão genérica (se houver)
        if (MissionManager.Instance != null && MissionManager.Instance.IsActive("useSalt"))
        {
            MissionManager.Instance.CompleteMission("useSalt");
        }

        yield return new WaitForSeconds(0.3f);

        // Avança diálogo
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }

    // ============================================================
    // SAL NO MAZZI - SEQUÊNCIA ESPECIAL
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

        // Completa a missão SaltMazzi
        if (MissionManager.Instance != null && MissionManager.Instance.IsActive("SaltMazzi"))
        {
            Debug.Log("[Fase3] ✓ Completando missão SaltMazzi");
            MissionManager.Instance.CompleteMission("SaltMazzi");
        }

        yield return new WaitForSeconds(0.3f);

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

        // Completa a missão SaltMazzi para retomar o diálogo
        if (MissionManager.Instance != null && MissionManager.Instance.IsActive("SaltMazzi"))
        {
            Debug.Log("[Fase3] ✓ Completando missão SaltMazzi");
            MissionManager.Instance.CompleteMission("SaltMazzi");
        }
        
        // Avança para próxima linha de diálogo
        if (DialogueManager.Instance != null)
        {
            Debug.Log("[Fase3] Avançando para próxima linha de diálogo");
            DialogueManager.Instance.ShowNextLine();
        }
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
        Debug.Log("[Fase3] Fade in do Mazzi completo");
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