using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fase4MissionHandler : MissionHandlerBase
{
    [Header("Sprites e Entidades")]
    public GameObject philippaSprite;
    public GameObject jarvisGhost;

    [Header("UI extra")]
    public GameObject pImage;
    
    [Header("Sons")]
    public AudioClip ghostAttackSound;
    public AudioClip ghostDisappearSound;

    [Header("Trilha Sonora da Fase 4")]
    public AudioClip fase4Music;
    private AudioSource musicSource;
    
    void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;

        // Música SEMPRE usa um AudioSource NOVO (como a Fase 3)
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = 0.3f;
        musicSource.spatialBlend = 0f;

        if (fase4Music != null)
        {
            musicSource.clip = fase4Music;
            musicSource.Play();
            Debug.Log("[Fase4] 🎶 Música iniciada em loop.");
        }
        else
        {
            Debug.LogWarning("[Fase4] ⚠️ Nenhuma música atribuída em fase4Music.");
        }
    }


    void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;

        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("[Fase4] 🛑 Música parada no OnDisable.");
        }
    }


    private void OnMissionCompletedHandler(string missionId)
    {
        Debug.Log($"[Fase4] Missão completa: {missionId}");

        switch (missionId)
        {
            case "ghostAttack":
                StartCoroutine(GhostAttackSequence());
                break;

            case "ghostDisappear":
                StartCoroutine(GhostDisappearSequence());
                break;
        }
    }

    public override void HandleMission(string missionId)
    {
        switch (missionId)
        {
            case "fadeIn":
                StartCoroutine(FadeInSequence());
                break;

            case "fadeOut":
                StartCoroutine(FadeOutSequence());
                break;

            case "GotoCutscene":
                GoToCutscene();
                break;

            case "findTape":
                // A missão é iniciada, mas a lógica está no CollectibleFita
                MissionManager.Instance.StartMission("findTape");
                Debug.Log("[Fase4] Missão findTape iniciada - jogador deve coletar a fita.");
                break;

            case "watchTape":
                StartCoroutine(WatchTapeSequence());
                break;

            case "ghostAttack":
                // ✅ CORRIGIDO: Inicia a missão E executa a sequência
                MissionManager.Instance.StartMission("ghostAttack");
                Debug.Log("[Fase4] Missão ghostAttack iniciada - fantasma ataca Philippa.");
                StartCoroutine(GhostAttackSequence());
                break;
            
            case "ghostDisappear":
                MissionManager.Instance.StartMission("ghostDisappear");
                Debug.Log("[Fase4] Missão ghostDisappear iniciada - fantasma desaparece.");
                StartCoroutine(GhostDisappearSequence());
                break;

            default:
                Debug.LogWarning($"[Fase4] Missão desconhecida: {missionId}");
                break;
        }
    }

    // ==================== SEQUÊNCIAS ====================

    private IEnumerator FadeInSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();

        if (vfx != null)
            yield return vfx.FadeFromBlack(2f);
        else
            yield return new WaitForSeconds(2f);

        CompleteMission("fadeIn");
        yield return null;
        
        if (pImage != null)
        {
            pImage.SetActive(false);
            Debug.Log("[Fase4] pImage desativado após FadeIn.");
        }

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator FadeOutSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();
        float fadeDuration = 1.2f;

        if (vfx != null)
            yield return StartCoroutine(vfx.FadeToBlack(fadeDuration));
        else
            yield return new WaitForSeconds(fadeDuration);

        CompleteMission("fadeOut");
        yield return null;

        // Avança diálogo após fade
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator GhostAttackSequence()
    {
        Debug.Log("[Fase4] === FANTASMA JARVIS ATACA PHILIPPA ===");
        
        if (pImage != null)
        {
            pImage.SetActive(false);
            Debug.Log("[Fase4] pImage desativado no início do GhostAttackSequence.");
        }
        
        // 1. Põe tela preta (fadeIn)
        VisualEffectsManager vfx = GetEffectsManager();
        if (vfx != null)
        {
            Debug.Log("[Fase4] ✓ Iniciando fade para preto...");
            yield return StartCoroutine(vfx.FadeToBlack(0.5f));
        }
        else
        {
            Debug.LogWarning("[Fase4] VisualEffectsManager não encontrado!");
            yield return new WaitForSeconds(1f);
        }

        // 2. Toca áudio (grito)
        if (ghostAttackSound != null)
        {
            AudioSource.PlayClipAtPoint(ghostAttackSound, Camera.main.transform.position, 0.7f);
            Debug.Log("[Fase4] ✓ Áudio de ataque tocando");
        
            // Espera um pouco para o áudio tocar
            yield return new WaitForSeconds(5f);
        }
        else
        {
            Debug.LogWarning("[Fase4] ghostAttackSound não configurado!");
            yield return new WaitForSeconds(1f);
        }

        // 3. Tira tela preta (fadeOut)
        if (vfx != null)
        {
            Debug.Log("[Fase4] ✓ Iniciando fade de volta...");
            yield return StartCoroutine(vfx.FadeFromBlack(1f));
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("[Fase4] === ATAQUE CONCLUÍDO ===");

        // 4. Continua diálogo
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowNextLine();
        }
        SaveSystem.Instance.fase3_exorcizou = false;
        SaveSystem.Instance.Salvar();
    }

    private IEnumerator GhostDisappearSequence()
    {
        Debug.Log("[Fase4] === FANTASMA JARVIS DESAPARECE ===");

        // Som de desaparecimento
        if (ghostDisappearSound != null)
        {
            AudioSource.PlayClipAtPoint(ghostDisappearSound, Camera.main.transform.position, 0.5f);
        }

        // Desativa o fantasma
        if (jarvisGhost != null)
        {
            jarvisGhost.SetActive(false);
            Debug.Log("[Fase4] ✓ Fantasma Jarvis desativado");
        }

        yield return new WaitForSeconds(0.5f);

        // Avança diálogo
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }
    private IEnumerator WatchTapeSequence()
    {
        Debug.Log("[Fase4] === ASSISTINDO A FITA ===");
    
        // A sequência real acontece no TelaClickableArea
        // Aqui só confirmamos que a missão foi completada
    
        yield return new WaitForSeconds(0.5f);
    
        // Avança diálogo
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }
    
    private void GoToCutscene()
    {
        Debug.Log("[Fase4] Indo para cutscene...");
    
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene("10. Cutscene");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("10. Cutscene");
        }
    }
    
}