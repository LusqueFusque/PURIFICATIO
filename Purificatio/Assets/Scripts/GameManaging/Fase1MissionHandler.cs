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
    
    [Header("UI da Fase 1")]
    public GameObject dialoguePanel;   // arraste o painel de diálogo no Inspector
    public GameObject hudPanel;        // arraste o painel de HUD no Inspector
    void OnEnable()
    {
        // ✅ Registra listener para detectar quando findDoll é completada
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;
        }
    }

    void OnDisable()
    {
        // Remove listener ao desabilitar
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;
        }
    }

    // ✅ Handler que escuta todas as missões completadas
    private void OnMissionCompletedHandler(string completedMissionId)
    {
        Debug.Log($"[Fase1MissionHandler] Missão completada detectada: {completedMissionId}");

        // Quando findDoll é completada, dispara o diálogo
        if (completedMissionId == "findDoll")
        {
            Debug.Log("[Fase1MissionHandler] ✅ findDoll completada! Disparando diálogo 'encontrou_boneca'");
            
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.GoToNode("encontrou_boneca");
            }
            else
            {
                Debug.LogError("[Fase1MissionHandler] DialogueManager não encontrado!");
            }
        }
    }

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
            
            case "wait":
                StartCoroutine(WaitSequence());
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

        // ✅ Completa a missão
        CompleteMission("fadeIn");
        
        // ✅ Aguarda 1 frame
        yield return null;
        
        // ✅ Continua o diálogo
        Debug.Log("[Fase1] Fade In completo. Continuando diálogo...");
        
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
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
            yield return null;
            
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowNextLine();
            }
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

        // 7. Marca missão concluída
        CompleteMission("GhostSpriteAppear");
        
        // 8. Aguarda 1 frame e continua
        yield return null;
        
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
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
            vfx.RedScreenEffect(10f);
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

        // Desativa também a UI da Eveline
        if (evelineUIImage != null)
        {
            evelineUIImage.gameObject.SetActive(false);
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

        // ✅ MARCA MISSÃO COMPLETA
        CompleteMission("exorcismoDaBoneca");
        Debug.Log("[Fase1] Exorcismo completo!");

        // ✅ Vai para o diálogo pós-exorcismo
        yield return new WaitForSeconds(0.5f);
        
        if (DialogueManager.Instance != null)
        {
            Debug.Log("[Fase1] Indo para diálogo 'exorcismo_completo'");
            DialogueManager.Instance.GoToNode("exorcismo_completo");
        }
    }

    // ==================== POLTERGEIST ====================
    private IEnumerator PoltergeistSequence()
    {
        Debug.Log("[Fase1] Iniciando transformação em poltergeist...");

        VisualEffectsManager vfx = GetEffectsManager();

        // Fecha HUD e diálogo
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (hudPanel != null)
            hudPanel.SetActive(false);

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
            // espera a duração do áudio antes de reabrir HUD/diálogo
            yield return new WaitForSeconds(poltergeistSound.length);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        if (vfx != null)
        {
            vfx.ClearRedScreen();
        }

        // ✅ Completa a missão
        CompleteMission("poltergeistTransformation");
        Debug.Log("[Fase1] Poltergeist criado!");

        // Reabre HUD e diálogo
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (hudPanel != null)
            hudPanel.SetActive(true);

        yield return null;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
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

        // ✅ Completa a missão
        CompleteMission("FadeOut");
        
        // ✅ Aguarda 1 frame
        yield return null;
        
        // ✅ Continua o diálogo
        Debug.Log("[Fase1] Fade Out completo. Continuando diálogo...");
        
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }
    
    private IEnumerator WaitSequence()
    {
        Debug.Log("[Fase1] Iniciando missão 'wait'...");

        // espera 3 segundos
        yield return new WaitForSeconds(3f);

        // marca missão como concluída
        CompleteMission("wait");
        Debug.Log("[Fase1] Missão 'wait' concluída!");

        // continua o diálogo
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }

}