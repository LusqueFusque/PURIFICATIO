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
    public GameObject evelineGhostSprite; // Sprite do fantasma Eveline na cena (NÃO USADO)
    public UnityEngine.UI.Image evelineImage; // Image UI da Eveline (USAR ESTE)
    public AudioClip screamSound; // Som de choro/grito
    public AudioClip poltergeistSound; // Som do poltergeist

    [Header("Efeitos")]
    public float fadeDuration = 2f;
    public float ghostFadeInDuration = 1.5f; // Tempo do fade in do fantasma

    public override void HandleMission(string missionId)
    {
        Debug.Log($"[Fase1MissionHandler] Processando missão: {missionId}");

        switch (missionId)
        {
            case "faseIn":
            case "fadeIn":
                StartCoroutine(FadeInSequence());
                break;

            case "findGhost":
                // Missão: Jogador deve usar a câmera manualmente
                // O item da câmera completa essa missão quando for usado
                Debug.Log("[Fase1] Aguardando jogador usar a câmera fotográfica...");
                // NÃO completa aqui - o PhotoCameraItem vai chamar CompleteMission("findGhost")
                break;

            case "GhostSpriteAppear":
                // Faz Eveline aparecer visível (fade in gradual na UI)
                StartCoroutine(GhostAppearSequence());
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

    // ==================== GHOST APPEAR (FADE IN GRADUAL) ====================
    private IEnumerator GhostAppearSequence()
    {
        Debug.Log("[Fase1] ========== INICIANDO GHOST APPEAR ==========");
        
        // Desativa o sprite (SpriteRenderer) se estiver ativo
        if (evelineGhostSprite != null)
        {
            evelineGhostSprite.SetActive(false);
            Debug.Log("[Fase1] Sprite do fantasma desativado.");
        }
        
        // Ativa a Image UI e faz fade in
        if (evelineImage != null)
        {
            evelineImage.gameObject.SetActive(true);
            
            // Começa transparente
            Color c = evelineImage.color;
            c.a = 0f;
            evelineImage.color = c;
            
            Debug.Log("[Fase1] Iniciando fade in da EvelineImage...");
            
            // Fade in gradual
            float elapsed = 0f;
            while (elapsed < ghostFadeInDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / ghostFadeInDuration);
                evelineImage.color = c;
                yield return null;
            }
            
            // Garante que ficou totalmente visível
            c.a = 1f;
            evelineImage.color = c;
            
            Debug.Log("[Fase1] Eveline agora está visível na UI!");
        }
        else
        {
            Debug.LogWarning("[Fase1] evelineImage não atribuído no Inspector!");
        }
        
        Debug.Log("[Fase1] ========== GHOST APPEAR COMPLETO ==========");
        
        // Completa missão e continua diálogo
        CompleteMissionAndContinue("GhostSpriteAppear");
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