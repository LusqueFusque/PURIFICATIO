using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Handler de missões da Fase 3
/// Missões: fadeIn, FindCarpet, RevealPentagram, SaltMazzi, exorcismoMazzi, holyWaterMazzi, fadeOut
/// </summary>
public class Fase3MissionHandler : MissionHandlerBase
{
    [Header("Referências da Fase 3")]
    public GameObject mazziGhostSprite;
    public Image mazziUIImage;
    
    [Header("Áudio")]
    public AudioClip mazziScreamSound;
    public AudioClip saltSound;
    public AudioClip holyWaterSound;
    public AudioClip exorcismSound;
    
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
        Debug.Log($"[Fase3MissionHandler] Missão completada: {completedMissionId}");

        // Quando tapete é encontrado, dispara diálogo
        if (completedMissionId == "FindCarpet")
        {
            Debug.Log("[Fase3] ✅ Tapete encontrado! Disparando diálogo 'encontrou_tapete'");
            
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.GoToNode("encontrou_tapete");
            }
        }
    }

    public override void HandleMission(string missionId)
    {
        Debug.Log($"[Fase3MissionHandler] Processando missão: {missionId}");

        switch (missionId)
        {
            case "fadeIn":
                StartCoroutine(FadeInSequence());
                break;

            case "FindCarpet":
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.StartMission("FindCarpet");
                    Debug.Log("[Fase3] Missão 'FindCarpet' iniciada. Aguardando coleta...");
                }
                break;

            case "RevealPentagram":
                StartCoroutine(RevealPentagramSequence());
                break;

            case "SaltMazzi":
                StartCoroutine(SaltMazziSequence());
                break;

            case "exorcismoMazzi":
                StartCoroutine(ExorcismoMazziSequence());
                break;

            case "holyWaterMazzi":
                StartCoroutine(HolyWaterMazziSequence());
                break;

            case "fadeOut":
                StartCoroutine(FadeOutSequence());
                break;

            default:
                Debug.LogWarning($"[Fase3MissionHandler] Missão desconhecida: {missionId}");
                break;
        }
    }

    // ==================== FADE IN ====================
    private IEnumerator FadeInSequence()
    {
        Debug.Log("🟢 [Fase3] FadeInSequence INICIOU!");

        VisualEffectsManager vfx = GetEffectsManager();
        Debug.Log($"🟢 [Fase3] VFX Manager encontrado? {vfx != null}");
    
        if (vfx != null)
        {
            Debug.Log("🟢 [Fase3] Iniciando FadeFromBlack...");
            yield return StartCoroutine(vfx.FadeFromBlack(fadeDuration));
            Debug.Log("🟢 [Fase3] FadeFromBlack CONCLUÍDO!");
        }
        else
        {
            Debug.Log("⚠️ [Fase3] VFX não encontrado, aguardando tempo...");
            yield return new WaitForSeconds(fadeDuration);
        }

        Debug.Log("🟢 [Fase3] Completando missão fadeIn...");
        CompleteMission("fadeIn");
    
        Debug.Log("🟢 [Fase3] Aguardando 1 frame...");
        yield return null;
    
        Debug.Log("🟢 [Fase3] Chamando ShowNextLine...");
    
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
            Debug.Log("🟢 [Fase3] ShowNextLine chamado!");
        }
        else
        {
            Debug.LogError("❌ [Fase3] DialogueManager não encontrado!");
        }
    }

    // ==================== REVELAR PENTÁGRAMA ====================
    private IEnumerator RevealPentagramSequence()
    {
        Debug.Log("[Fase3] Iniciando sequência de revelar pentágrama...");
        VisualEffectsManager vfx = GetEffectsManager();

        yield return new WaitForSeconds(0.5f);

        Debug.Log("[Fase3] ✓ Pentágrama revelado!");

        CompleteMission("RevealPentagram");
        yield return null;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }

    // ==================== SAL GROSSO - INVOCAR MAZZI ====================
    private IEnumerator SaltMazziSequence()
    {
        Debug.Log("[Fase3] Iniciando sequência de sal grosso (invocar Mazzi)...");
        VisualEffectsManager vfx = GetEffectsManager();

        // Som de sal
        if (saltSound != null)
        {
            AudioSource.PlayClipAtPoint(saltSound, Camera.main.transform.position, 0.6f);
        }

        yield return new WaitForSeconds(0.5f);

        // Efeito vermelho
        if (vfx != null)
        {
            vfx.RedScreenEffect(1f);
        }

        yield return new WaitForSeconds(0.5f);

        // Som de invocação
        if (mazziScreamSound != null)
        {
            AudioSource.PlayClipAtPoint(mazziScreamSound, Camera.main.transform.position, 0.7f);
        }

        yield return new WaitForSeconds(0.5f);

        // Mostra Mazzi
        if (mazziGhostSprite != null)
        {
            mazziGhostSprite.SetActive(true);
        }

        if (mazziUIImage != null)
        {
            mazziUIImage.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        if (vfx != null)
        {
            vfx.ClearRedScreen();
        }

        CompleteMission("SaltMazzi");
        Debug.Log("[Fase3] ✓ Mazzi invocado!");

        yield return null;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }

    // ==================== EXORCISMO MAZZI ====================
    private IEnumerator ExorcismoMazziSequence()
    {
        Debug.Log("[Fase3] Iniciando sequência de exorcismo Mazzi...");
        VisualEffectsManager vfx = GetEffectsManager();

        // Para música
        AudioSource music = FindObjectOfType<AudioSource>();
        if (music != null && music.isPlaying)
        {
            music.Stop();
        }

        // Grito de Mazzi
        if (mazziScreamSound != null)
        {
            AudioSource.PlayClipAtPoint(mazziScreamSound, Camera.main.transform.position, 0.7f);
        }

        yield return new WaitForSeconds(0.5f);

        // Efeito vermelho
        if (vfx != null)
        {
            vfx.RedScreenEffect(1.5f);
        }

        yield return new WaitForSeconds(0.5f);

        // Som de exorcismo
        if (exorcismSound != null)
        {
            AudioSource.PlayClipAtPoint(exorcismSound, Camera.main.transform.position, 0.6f);
        }

        yield return new WaitForSeconds(0.5f);

        // Remove Mazzi
        if (mazziGhostSprite != null)
        {
            mazziGhostSprite.SetActive(false);
        }

        if (mazziUIImage != null)
        {
            mazziUIImage.gameObject.SetActive(false);
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

        CompleteMission("exorcismoMazzi");
        Debug.Log("[Fase3] ✓ Mazzi exorcizado!");

        yield return null;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }

    // ==================== ÁGUA BENTA - MAZZI OCULTO ====================
    private IEnumerator HolyWaterMazziSequence()
    {
        Debug.Log("[Fase3] Iniciando sequência de água benta...");

        // Som de água benta
        if (holyWaterSound != null)
        {
            AudioSource.PlayClipAtPoint(holyWaterSound, Camera.main.transform.position, 0.6f);
        }

        yield return new WaitForSeconds(0.5f);

        // Remove Mazzi (fica oculto)
        if (mazziGhostSprite != null)
        {
            mazziGhostSprite.SetActive(false);
        }

        if (mazziUIImage != null)
        {
            mazziUIImage.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);

        CompleteMission("holyWaterMazzi");
        Debug.Log("[Fase3] ✓ Água benta lançada! Mazzi ocultado!");

        yield return null;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }

    // ==================== FADE OUT ====================
    private IEnumerator FadeOutSequence()
    {
        Debug.Log("[Fase3] Iniciando Fade Out...");
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