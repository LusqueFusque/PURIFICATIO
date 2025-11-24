using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Handler de missões da Fase 2
/// Missões: fadeIn, FindLamp, rubLamp, throwLamp, fadeOut, returnToMenu
/// </summary>
public class Fase2MissionHandler : MissionHandlerBase
{
    [Header("Referências da Fase 2")]
    public GameObject djinnGhostSprite;
    public Image djinnUIImage;
    
    [Header("Áudio de Efeitos")]
    public AudioClip djinnScreamSound;
    public AudioClip lampThrowSound;      // Som 1: Whoosh/arremesso
    public AudioClip glassShatterSound;   // Som 2: Vidro quebrando
    public AudioClip metalImpactSound;    // Som 3: Impacto metálico

    [Header("Trilha Sonora da Fase 2")]
    public AudioClip fase2Music;
    private AudioSource musicSource;
    
    [Header("Efeitos")]
    public float fadeDuration = 2f;

    [Header("Referências Extras")]
    public GameObject KeyImage;
    
    void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;
        SaveSystem.Instance.fase2_exorcizou = false;
        SaveSystem.Instance.Salvar();

        // 🎵 Inicia trilha sonora em loop
        if (fase2Music != null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = fase2Music;
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = 0.6f;
            musicSource.Play();
            Debug.Log("[Fase2] 🎶 Trilha sonora iniciada.");
        }
        else
        {
            Debug.LogWarning("[Fase2] ⚠️ Nenhuma trilha atribuída ao fase2Music.");
        }
    }

    void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;

        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Destroy(musicSource);
            Debug.Log("[Fase2] 🛑 Trilha sonora parada.");
        }
    }

    private void OnMissionCompletedHandler(string completedMissionId)
    {
        Debug.Log($"[Fase2MissionHandler] Missão completada: {completedMissionId}");

        if (completedMissionId == "FindLamp")
        {
            Debug.Log("[Fase2] ✅ Lâmpada encontrada! Disparando diálogo 'nambulampada1'");
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.GoToNode("nambulampada1");
        }

        if (completedMissionId == "glassBreak")
        {
            Debug.Log("[Fase2] 🔑 Vidro quebrado! Ativando KeyImage...");
            if (KeyImage != null)
            {
                KeyImage.SetActive(true);
                Debug.Log("[Fase2] ✓ KeyImage ativada com sucesso!");
            }
            else
            {
                Debug.LogWarning("[Fase2] ⚠️ KeyImage não está atribuída no inspetor!");
            }
        }
    }

    public override void HandleMission(string missionId)
    {
        Debug.Log($"[Fase2MissionHandler] Processando missão: {missionId}");

        switch (missionId)
        {
            case "fadeIn": StartCoroutine(FadeInSequence()); break;
            case "FindLamp":
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.StartMission("FindLamp");
                    Debug.Log("[Fase2] Missão 'FindLamp' iniciada. Aguardando coleta...");
                }
                break;
            case "rubLamp": StartCoroutine(RubLampSequence()); break;
            case "throwLamp": StartCoroutine(ThrowLampSequence()); break;
            case "fadeOut": StartCoroutine(FadeOutSequence()); break;

            case "returnToMenu":
                if (GameManager.Instance != null)
                    GameManager.Instance.LoadScene("02. Menu");
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene("02. Menu");
                break;

            default:
                Debug.LogWarning($"[Fase2MissionHandler] Missão desconhecida: {missionId}");
                break;
        }
    }

    // ==================== FADE IN ====================
    private IEnumerator FadeInSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();
        if (vfx != null) yield return StartCoroutine(vfx.FadeFromBlack(fadeDuration));
        else yield return new WaitForSeconds(fadeDuration);

        CompleteMission("fadeIn");
        yield return null;

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }

    // ==================== ESFREGAR LÂMPADA ====================
    private IEnumerator RubLampSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();
        if (vfx != null) vfx.RedScreenEffect(1f);

        yield return new WaitForSeconds(0.5f);

        if (djinnScreamSound != null)
            AudioSource.PlayClipAtPoint(djinnScreamSound, Camera.main.transform.position, 0.7f);

        yield return new WaitForSeconds(0.5f);

        if (djinnGhostSprite != null) djinnGhostSprite.SetActive(true);
        if (djinnUIImage != null) djinnUIImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        if (vfx != null) vfx.ClearRedScreen();

        CompleteMission("rubLamp");
        yield return null;

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }

    // ==================== JOGAR LÂMPADA ====================
    private IEnumerator ThrowLampSequence()
    {
        // Para a trilha da fase
        if (musicSource != null && musicSource.isPlaying) musicSource.Stop();

        if (djinnScreamSound != null)
            AudioSource.PlayClipAtPoint(djinnScreamSound, Camera.main.transform.position, 0.7f);

        yield return new WaitForSeconds(0.5f);

        if (lampThrowSound != null)
            AudioSource.PlayClipAtPoint(lampThrowSound, Camera.main.transform.position, 0.5f);

        yield return new WaitForSeconds(0.7f);

        if (glassShatterSound != null)
            AudioSource.PlayClipAtPoint(glassShatterSound, Camera.main.transform.position, 0.6f);

        yield return new WaitForSeconds(0.5f);

        if (metalImpactSound != null)
            AudioSource.PlayClipAtPoint(metalImpactSound, Camera.main.transform.position, 0.5f);

        if (djinnGhostSprite != null) djinnGhostSprite.SetActive(false);
        if (djinnUIImage != null) djinnUIImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        // Retoma a trilha da fase
        if (musicSource != null) musicSource.Play();

        CompleteMission("throwLamp");
        yield return null;

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.GoToNode("rota_a4");

        SaveSystem.Instance.fase2_exorcizou = true;
        SaveSystem.Instance.Salvar();
    }

    // ==================== FADE OUT ====================
    private IEnumerator FadeOutSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();
        if (vfx != null)
            yield return StartCoroutine(vfx.FadeToBlack(fadeDuration));
        else
            yield return new WaitForSeconds(fadeDuration);

        CompleteMission("fadeOut");
        yield return null;
    }
}
