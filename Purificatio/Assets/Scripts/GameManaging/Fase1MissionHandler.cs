using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Handler específico para as missões da Fase 1.
/// Gerencia: FadeIn, findGhost, GhostSpriteAppear, findDoll, 
/// exorcismoDaBoneca, poltergeistTransformation, FadeOut
/// </summary>
public class Fase1MissionHandler : MissionHandlerBase
{
    [Header("Referências da Fase 1")]
    [Tooltip("Sprite do fantasma Eveline na cena (objeto 2D do cenário)")]
    public GameObject evelineGhostSprite;

    [Tooltip("Imagem UI da Eveline (por exemplo, 'EvelineImage' no Canvas)")]
    public Image evelineUIImage;

    [Header("Áudio da Fase 1")]
    public AudioClip screamSound;
    public AudioClip poltergeistSound;

    [Header("Efeitos")]
    [Tooltip("Duração dos fades (em segundos)")]
    public float fadeDuration = 2f;

    public override void HandleMission(string missionId)
    {
        Debug.Log($"[Fase1MissionHandler] Processando missão: {missionId}");

        switch (missionId)
        {
            case "FadeIn":
                StartCoroutine(FadeInSequence());
                break;

            case "findGhost":
                // Missão: Usar câmera para ver Eveline
                Debug.Log("[Fase1] Aguardando jogador usar câmera...");
                break;

            case "GhostSpriteAppear":
                StartCoroutine(GhostSpriteAppearSequence());
                break;

            case "findDoll":
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.StartMission("findDoll");
                    Debug.Log("[Fase1] Missão 'findDoll' registrada. Aguardando jogador encontrar boneca...");
                }
                else
                {
                    Debug.LogError("[Fase1] MissionManager não encontrado!");
                }
                break;

            case "exorcismoDaBoneca":
                StartCoroutine(ExorcismSequence());
                break;

            case "poltergeistTransformation":
                StartCoroutine(PoltergeistSequence());
                break;

            case "FadeOut":
            case "fadeOut":
                StartCoroutine(FadeOutSequence());
                break;

            default:
                Debug.LogWarning($"[Fase1MissionHandler] Missão desconhecida: {missionId}");
                break;
        }
    }

    // ==================== FADE IN ====================
    private IEnumerator FadeInSequence()
    {
        Debug.Log("[Fase1] Iniciando Fade In...");

        VisualEffectsManager vfx = GetEffectsManager();
        if (vfx != null)
        {
            yield return StartCoroutine(vfx.FadeFromBlack(fadeDuration));
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        CompleteMission("FadeIn");
    }

    // ==================== MOSTRAR FANTASMA (FADE-IN DE EVELINE) ====================
    private IEnumerator GhostSpriteAppearSequence()
    {
        Debug.Log("[Fase1] Iniciando GhostSpriteAppearSequence (fade-in de Eveline)...");

        // 1. Garante que o sprite 2D (no mundo) comece invisível
        if (evelineGhostSprite != null)
        {
            evelineGhostSprite.SetActive(true);
            SpriteRenderer sr = evelineGhostSprite.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color startColor = sr.color;
                startColor.a = 0f;
                sr.color = startColor;
            }
        }

        // 2. Garante que a imagem da UI está configurada
        if (evelineUIImage == null)
        {
            Debug.LogWarning("[Fase1] ⚠️ Nenhuma referência UI da Eveline atribuída no Inspector!");
            CompleteMission("GhostSpriteAppear");
            DialogueManager.Instance.ContinueDialogue();
            yield break;
        }

        // 3. Inicia invisível
        Color color = evelineUIImage.color;
        color.a = 0f;
        evelineUIImage.color = color;
        evelineUIImage.gameObject.SetActive(true);

        // 4. Fade in gradual (0 → 1)
        float elapsed = 0f;
        float duration = fadeDuration > 0 ? fadeDuration : 2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(0f, 1f, t);
            evelineUIImage.color = color;
            yield return null;
        }

        // 5. Garante visibilidade total ao final
        color.a = 1f;
        evelineUIImage.color = color;

        Debug.Log("[Fase1] Fade-in de EvelineImage concluído!");

        // 6. Pequena pausa pra impacto visual
        yield return new WaitForSeconds(0.5f);

        // 7. Marca missão concluída e prossegue diálogo
        CompleteMission("GhostSpriteAppear");
        DialogueManager.Instance.ContinueDialogue();
    }

    // ==================== EXORCISMO ====================
    private IEnumerator ExorcismSequence()
    {
        Debug.Log("[Fase1] Iniciando sequência de exorcismo...");

        VisualEffectsManager vfx = GetEffectsManager();

        // 1. Para música
        AudioSource music = FindObjectOfType<AudioSource>();
        if (music != null && music.isPlaying)
        {
            music.Stop();
        }

        // 2. Tela vermelha
        if (vfx != null)
        {
            vfx.RedScreenEffect(3f);
        }

        yield return new WaitForSeconds(0.5f);

        // 3. Som de choro horripilante
        if (screamSound != null)
        {
            AudioSource.PlayClipAtPoint(screamSound, Camera.main.transform.position, 0.7f);
        }

        yield return new WaitForSeconds(2f);

        // 4. Desativa Eveline (exorcizada)
        if (evelineGhostSprite != null)
        {
            evelineGhostSprite.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);

        // 5. Volta ao normal
        if (vfx != null)
        {
            vfx.ClearRedScreen();
        }

        // 6. Volta música (opcional)
        if (music != null)
        {
            music.Play();
        }

        CompleteMission("exorcismoDaBoneca");
        Debug.Log("[Fase1] Exorcismo completo!");
    }

    // ==================== POLTERGEIST ====================
    private IEnumerator PoltergeistSequence()
    {
        Debug.Log("[Fase1] Iniciando transformação em poltergeist...");

        VisualEffectsManager vfx = GetEffectsManager();

        AudioSource music = FindObjectOfType<AudioSource>();
        if (music != null && music.isPlaying)
        {
            music.Stop();
        }

        if (vfx != null)
        {
            vfx.RedScreenEffect(2f);
        }

        yield return new WaitForSeconds(0.5f);

        if (screamSound != null)
        {
            AudioSource.PlayClipAtPoint(screamSound, Camera.main.transform.position, 0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        if (vfx != null)
        {
            yield return StartCoroutine(vfx.FadeToBlack(1f));
        }

        yield return new WaitForSeconds(1f);

        if (poltergeistSound != null)
        {
            AudioSource.PlayClipAtPoint(poltergeistSound, Camera.main.transform.position, 0.6f);
        }

        yield return new WaitForSeconds(2f);

        if (vfx != null)
        {
            vfx.ClearRedScreen();
        }

        CompleteMission("poltergeistTransformation");
        Debug.Log("[Fase1] Poltergeist criado!");
    }

    // ==================== FADE OUT ====================
    private IEnumerator FadeOutSequence()
    {
        Debug.Log("[Fase1] Iniciando Fade Out...");

        VisualEffectsManager vfx = GetEffectsManager();
        if (vfx != null)
        {
            yield return StartCoroutine(vfx.FadeToBlack(fadeDuration));
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        CompleteMission("FadeOut");
    }
}
