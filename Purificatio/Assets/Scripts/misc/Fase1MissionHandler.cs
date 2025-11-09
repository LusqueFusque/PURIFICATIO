using UnityEngine;
using System.Collections;

/// <summary>
/// Handler específico para as missões da Fase 1.
/// Gerencia: FadeIn, findGhost, GhostSpriteAppear, findDoll, 
/// exorcismoDaBoneca, poltergeistTransformation, FadeOut
/// </summary>
public class Fase1MissionHandler : MissionHandlerBase
{
    [Header("Referências da Fase 1")]
    public GameObject evelineGhostSprite; // Sprite do fantasma Eveline na cena
    public AudioClip screamSound; // Som de choro/grito
    public AudioClip poltergeistSound; // Som do poltergeist

    [Header("Efeitos")]
    public float fadeDuration = 2f;

    public override void HandleMission(string missionId)
    {
        Debug.Log($"[Fase1MissionHandler] Processando missão: {missionId}");

        switch (missionId)
        {
            case "FadeIn":
            case "fadeIn":
                StartCoroutine(FadeInSequence());
                break;

            case "findGhost":
                // Missão: Usar câmera para ver Eveline
                // Completada por PhotoCameraItem quando detectar Eveline
                Debug.Log("[Fase1] Aguardando jogador usar câmera...");
                break;

            case "GhostSpriteAppear":
                // Faz Eveline aparecer visível (sem câmera)
                ShowGhostSprite();
                CompleteMissionAndContinue(missionId);
                break;

            case "findDoll":
                // Missão: Encontrar e consertar boneca
                // Completada por outros scripts quando jogador entregar boneca
                Debug.Log("[Fase1] Aguardando jogador encontrar boneca...");
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
        Debug.Log("[Fase1] ========== INICIANDO FADE IN ==========");
        
        VisualEffectsManager vfx = GetEffectsManager();
        
        if (vfx == null)
        {
            Debug.LogError("[Fase1] VisualEffectsManager não encontrado na cena!");
            yield return new WaitForSeconds(fadeDuration);
        }
        else
        {
            Debug.Log("[Fase1] Executando FadeFromBlack...");
            yield return StartCoroutine(vfx.FadeFromBlack(fadeDuration));
            Debug.Log("[Fase1] FadeFromBlack completo!");
        }
    
        Debug.Log("[Fase1] ========== FADE IN COMPLETO ==========");
        
        // Completa a missão e continua o diálogo
        CompleteMissionAndContinue("FadeIn");
    }

    // ==================== MOSTRAR FANTASMA ====================
    private void ShowGhostSprite()
    {
        if (evelineGhostSprite != null)
        {
            evelineGhostSprite.SetActive(true);
            
            // Fade in do sprite
            SpriteRenderer sr = evelineGhostSprite.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }

            Debug.Log("[Fase1] Eveline agora está visível!");
        }
        else
        {
            Debug.LogWarning("[Fase1] evelineGhostSprite não atribuído!");
        }
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

        Debug.Log("[Fase1] Exorcismo completo!");
        CompleteMissionAndContinue("exorcismoDaBoneca");
    }

    // ==================== POLTERGEIST ====================
    private IEnumerator PoltergeistSequence()
    {
        Debug.Log("[Fase1] Iniciando transformação em poltergeist...");

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
            vfx.RedScreenEffect(2f);
        }

        yield return new WaitForSeconds(0.5f);

        // 3. Som de lâmpada explodindo
        if (screamSound != null)
        {
            AudioSource.PlayClipAtPoint(screamSound, Camera.main.transform.position, 0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        // 4. Fade to black
        if (vfx != null)
        {
            yield return StartCoroutine(vfx.FadeToBlack(1f));
        }

        yield return new WaitForSeconds(1f);

        // 5. Sons de objetos batendo
        if (poltergeistSound != null)
        {
            AudioSource.PlayClipAtPoint(poltergeistSound, Camera.main.transform.position, 0.6f);
        }

        yield return new WaitForSeconds(2f);

        // 6. Limpa efeitos
        if (vfx != null)
        {
            vfx.ClearRedScreen();
        }

        Debug.Log("[Fase1] Poltergeist criado!");
        CompleteMissionAndContinue("poltergeistTransformation");
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

        Debug.Log("[Fase1] Fade Out completo!");
        CompleteMissionAndContinue("FadeOut");
    }
    
    // ==================== HELPER: COMPLETA E CONTINUA ====================
    /// <summary>
    /// Completa a missão E retoma o diálogo automaticamente
    /// </summary>
    private void CompleteMissionAndContinue(string missionId)
    {
        // 1. Marca missão como completa
        CompleteMission(missionId);
        
        // 2. Retoma o diálogo
        if (DialogueManager.Instance != null)
        {
            Debug.Log($"[Fase1] Retomando diálogo após missão '{missionId}'");
            DialogueManager.Instance.OnMissionComplete();
        }
        else
        {
            Debug.LogError("[Fase1] DialogueManager.Instance é nulo!");
        }
    }
}