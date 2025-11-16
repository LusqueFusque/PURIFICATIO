using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Handler de missões da Fase 2
/// Missões: fadeIn, FindLamp, rubLamp, throwLamp, fadeOut
/// </summary>
public class Fase2MissionHandler : MissionHandlerBase
{
    [Header("Referências da Fase 2")]
    public GameObject djinnGhostSprite;
    public Image djinnUIImage;
    
    [Header("Áudio")]
    public AudioClip djinnScreamSound;
    public AudioClip lampThrowSound;
    
    [Header("Efeitos")]
    public float fadeDuration = 2f;

    void OnEnable()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;
        }
    }

    void OnDisable()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;
        }
    }

    private void OnMissionCompletedHandler(string completedMissionId)
    {
        Debug.Log($"[Fase2MissionHandler] Missão completada: {completedMissionId}");

        // Quando lâmpada é encontrada, dispara diálogo
        if (completedMissionId == "FindLamp")
        {
            Debug.Log("[Fase2] ✅ Lâmpada encontrada! Disparando diálogo 'nambulampada1'");
            
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.GoToNode("nambulampada1");
            }
        }
    }

    public override void HandleMission(string missionId)
    {
        Debug.Log($"[Fase2MissionHandler] Processando missão: {missionId}");

        switch (missionId)
        {
            case "fadeIn":
                StartCoroutine(FadeInSequence());
                break;

            case "FindLamp":
                // Missão inicia - aguarda jogador coletar lâmpada
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.StartMission("FindLamp");
                    Debug.Log("[Fase2] Missão 'FindLamp' iniciada. Aguardando coleta...");
                }
                break;

            case "rubLamp":
                StartCoroutine(RubLampSequence());
                break;

            case "throwLamp":
                StartCoroutine(ThrowLampSequence());
                break;

            case "fadeOut":
                StartCoroutine(FadeOutSequence());
                break;

            default:
                Debug.LogWarning($"[Fase2MissionHandler] Missão desconhecida: {missionId}");
                break;
        }
    }

    // ==================== FADE IN ====================
    private IEnumerator FadeInSequence()
    {
        Debug.Log("🟢 [Fase2] FadeInSequence INICIOU!");

        VisualEffectsManager vfx = GetEffectsManager();
        Debug.Log($"🟢 [Fase2] VFX Manager encontrado? {vfx != null}");
    
        if (vfx != null)
        {
            Debug.Log("🟢 [Fase2] Iniciando FadeFromBlack...");
            yield return StartCoroutine(vfx.FadeFromBlack(fadeDuration));
            Debug.Log("🟢 [Fase2] FadeFromBlack CONCLUÍDO!");
        }
        else
        {
            Debug.Log("⚠️ [Fase2] VFX não encontrado, aguardando tempo...");
            yield return new WaitForSeconds(fadeDuration);
        }

        Debug.Log("🟢 [Fase2] Completando missão fadeIn...");
        CompleteMission("fadeIn");
    
        Debug.Log("🟢 [Fase2] Aguardando 1 frame...");
        yield return null;
    
        Debug.Log("🟢 [Fase2] Chamando ShowNextLine...");
    
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
            Debug.Log("🟢 [Fase2] ShowNextLine chamado!");
        }
        else
        {
            Debug.LogError("❌ [Fase2] DialogueManager não encontrado!");
        }
    }

    // ==================== ESFREGAR LÂMPADA (INVOCAR DJINN) ====================
    private IEnumerator RubLampSequence()
    {
        Debug.Log("[Fase2] Iniciando sequência de esfregar lâmpada...");
        VisualEffectsManager vfx = GetEffectsManager();

        // Efeito vermelho
        if (vfx != null)
        {
            vfx.RedScreenEffect(1f);
        }

        yield return new WaitForSeconds(0.5f);

        // Som de invocação
        if (djinnScreamSound != null)
        {
            AudioSource.PlayClipAtPoint(djinnScreamSound, Camera.main.transform.position, 0.7f);
        }

        yield return new WaitForSeconds(0.5f);

        // Mostra Djinn
        if (djinnGhostSprite != null)
        {
            djinnGhostSprite.SetActive(true);
        }

        if (djinnUIImage != null)
        {
            djinnUIImage.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        if (vfx != null)
        {
            vfx.ClearRedScreen();
        }

        CompleteMission("rubLamp");
        Debug.Log("[Fase2] ✓ Djinn invocado!");

        yield return null;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }

    // ==================== JOGAR LÂMPADA PELA JANELA ====================
    private IEnumerator ThrowLampSequence()
    {
        Debug.Log("[Fase2] Iniciando sequência de jogar lâmpada...");
        VisualEffectsManager vfx = GetEffectsManager();

        // Para música
        AudioSource music = FindObjectOfType<AudioSource>();
        if (music != null && music.isPlaying)
        {
            music.Stop();
        }

        // Grito do Djinn
        if (djinnScreamSound != null)
        {
            AudioSource.PlayClipAtPoint(djinnScreamSound, Camera.main.transform.position, 0.7f);
        }

        yield return new WaitForSeconds(0.5f);

        // Efeito vermelho rápido
        if (vfx != null)
        {
            vfx.RedScreenEffect(1f);
        }

        yield return new WaitForSeconds(0.3f);

        // Som da lâmpada quebrando
        if (lampThrowSound != null)
        {
            AudioSource.PlayClipAtPoint(lampThrowSound, Camera.main.transform.position, 0.5f);
        }

        // Remove Djinn
        if (djinnGhostSprite != null)
        {
            djinnGhostSprite.SetActive(false);
        }

        if (djinnUIImage != null)
        {
            djinnUIImage.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);

        // Limpa efeito
        if (vfx != null)
        {
            vfx.ClearRedScreen();
        }

        // Volta música
        if (music != null)
        {
            music.Play();
        }

        CompleteMission("throwLamp");
        Debug.Log("[Fase2] ✓ Lâmpada jogada!");

        yield return null;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }

    // ==================== FADE OUT ====================
    private IEnumerator FadeOutSequence()
    {
        Debug.Log("[Fase2] Iniciando Fade Out...");
        VisualEffectsManager vfx = GetEffectsManager();

        if (vfx != null)
        {
            yield return StartCoroutine(vfx.FadeToBlack(fadeDuration));
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        CompleteMission("fadeOut");
        yield return null;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }
}