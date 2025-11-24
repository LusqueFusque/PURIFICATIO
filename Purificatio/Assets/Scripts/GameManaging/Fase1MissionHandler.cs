using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fase1MissionHandler : MissionHandlerBase
{
    [Header("Referências da Fase 1")]
    public GameObject evelineGhostSprite;
    public Image evelineUIImage;

    [Header("Áudio da Fase 1")]
    public AudioClip screamSound;
    public AudioClip poltergeistSound;

    [Header("Efeitos")]
    public float fadeDuration = 2f;
    
    [Header("UI da Fase 1")]
    public GameObject dialoguePanel;
    public GameObject hudPanel;
    
    [Header("Trilha Sonora")]
    public AudioClip fase1Music;
    private AudioSource musicSource;


    void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;
        
        // 🎵 Inicia trilha sonora em loop
        if (fase1Music != null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = fase1Music;
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = 0.6f;
            musicSource.Play();
            Debug.Log("[Fase1] 🎶 Trilha sonora iniciada.");
        }
        else
        {
            Debug.LogWarning("[Fase1] ⚠️ Nenhuma trilha atribuída ao fase1Music.");
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
            Debug.Log("[Fase1] 🛑 Trilha sonora parada.");
        }
    }


    private void OnMissionCompletedHandler(string completedMissionId)
    {
        Debug.Log($"[Fase1MissionHandler] Missão completada detectada: {completedMissionId}");

        if (completedMissionId == "findDoll")
        {
            Debug.Log("[Fase1MissionHandler] ✅ findDoll completada! Disparando diálogo 'encontrou_boneca'");
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.GoToNode("encontrou_boneca");
        }
    }

    public override void HandleMission(string missionId)
    {
        Debug.Log($"[Fase1MissionHandler] Processando missão: {missionId}");

        switch (missionId)
        {
            case "FadeIn": StartCoroutine(FadeInSequence()); break;
            case "findGhost": Debug.Log("[Fase1] Aguardando jogador usar câmera..."); break;
            case "GhostSpriteAppear": StartCoroutine(GhostSpriteAppearSequence()); break;
            case "findDoll":
                if (MissionManager.Instance != null)
                {
                    MissionManager.Instance.StartMission("findDoll");
                    Debug.Log("[Fase1] Missão 'findDoll' registrada. Aguardando jogador encontrar boneca...");
                }
                break;
            case "exorcismoDaBoneca": StartCoroutine(ExorcismSequence()); break;
            case "poltergeistTransformation": StartCoroutine(PoltergeistSequence()); break;
            case "FadeOut":
            case "fadeOut": StartCoroutine(FadeOutSequence()); break;
            case "wait": StartCoroutine(WaitSequence()); break;

            // ✅ Só aqui voltamos ao menu, quando o JSON manda "returnToMenu"
            case "returnToMenu":
                if (GameManager.Instance != null)
                    GameManager.Instance.LoadScene("02. Menu");
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene("02. Menu");
                break;

            default:
                Debug.LogWarning($"[Fase1MissionHandler] Missão desconhecida: {missionId}");
                break;
        }
    }

    // --- Fade In ---
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

    // --- Ghost Sprite Appear ---
    private IEnumerator GhostSpriteAppearSequence()
    {
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

        if (evelineUIImage == null)
        {
            CompleteMission("GhostSpriteAppear");
            yield return null;
            if (DialogueManager.Instance != null) DialogueManager.Instance.ShowNextLine();
            yield break;
        }

        Color color = evelineUIImage.color;
        color.a = 0f;
        evelineUIImage.color = color;
        evelineUIImage.gameObject.SetActive(true);

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

        color.a = 1f;
        evelineUIImage.color = color;

        yield return new WaitForSeconds(0.5f);

        CompleteMission("GhostSpriteAppear");
        yield return null;

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }

    // --- Exorcism ---
    private IEnumerator ExorcismSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();

        // Para a trilha da fase
        if (musicSource != null && musicSource.isPlaying) musicSource.Stop();
        if (vfx != null) vfx.RedScreenEffect(10f);

        yield return new WaitForSeconds(0.5f);
        if (screamSound != null) AudioSource.PlayClipAtPoint(screamSound, Camera.main.transform.position, 0.7f);
        yield return new WaitForSeconds(2f);

        if (evelineGhostSprite != null) evelineGhostSprite.SetActive(false);
        if (evelineUIImage != null) evelineUIImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        if (vfx != null) vfx.ClearRedScreen();

        // Retoma a trilha da fase
        if (musicSource != null) musicSource.Play();

        CompleteMission("exorcismoDaBoneca");
        yield return new WaitForSeconds(0.5f);

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.GoToNode("exorcismo_completo");

        SaveSystem.Instance.fase1_exorcizou = true;
        SaveSystem.Instance.Salvar();
    }


    // --- Poltergeist ---
    private IEnumerator PoltergeistSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(false);

        // Para a trilha da fase
        if (musicSource != null && musicSource.isPlaying) musicSource.Stop();
        if (vfx != null) vfx.RedScreenEffect(2f);

        yield return new WaitForSeconds(0.5f);
        if (screamSound != null) AudioSource.PlayClipAtPoint(screamSound, Camera.main.transform.position, 0.5f);
        yield return new WaitForSeconds(0.5f);

        if (vfx != null) yield return StartCoroutine(vfx.FadeToBlack(1f));
        yield return new WaitForSeconds(1f);

        if (poltergeistSound != null)
        {
            AudioSource.PlayClipAtPoint(poltergeistSound, Camera.main.transform.position, 0.6f);
            yield return new WaitForSeconds(poltergeistSound.length);
        }
        else yield return new WaitForSeconds(2f);

        if (vfx != null) vfx.ClearRedScreen();

        CompleteMission("poltergeistTransformation");

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (hudPanel != null) hudPanel.SetActive(true);

        yield return null;
        if (DialogueManager.Instance != null) DialogueManager.Instance.ShowNextLine();

        // Retoma a trilha da fase
        if (musicSource != null) musicSource.Play();

        SaveSystem.Instance.fase1_exorcizou = false;
        SaveSystem.Instance.Salvar();
    }

    // --- Fade Out ---
    private IEnumerator FadeOutSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();
        if (vfx != null) yield return StartCoroutine(vfx.FadeToBlack(fadeDuration));
        else yield return new WaitForSeconds(fadeDuration);

        CompleteMission("FadeOut");
        yield return null;

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();

        // ❌ Não troca de cena aqui
    }

    private IEnumerator WaitSequence()
    {
        yield return new WaitForSeconds(3f);
        CompleteMission("wait");
        if (DialogueManager.Instance != null) DialogueManager.Instance.ShowNextLine();
    }
}
