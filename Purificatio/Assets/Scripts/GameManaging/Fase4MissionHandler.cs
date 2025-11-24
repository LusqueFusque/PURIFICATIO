using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fase4MissionHandler : MissionHandlerBase
{
    [Header("UI da Fase 4")]
    public Image tapeImage;
    public Image telaClickArea;

    [Header("Sprites e Entidades")]
    public GameObject philippaSprite;
    public GameObject jarvisGhost;

    [Header("Itens Especiais")]
    public GameObject fitaItem; // referência ao coletável

    void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;
    }

    void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;
    }

    private void OnMissionCompletedHandler(string missionId)
    {
        Debug.Log($"[Fase4] Missão completa: {missionId}");

        switch (missionId)
        {
            case "findTape": StartCoroutine(FindTapeSequence()); break;
            case "watchTape": StartCoroutine(WatchTapeSequence()); break;
            case "ghostAttack": StartCoroutine(GhostAttackSequence()); break;
            case "ghostDisappear": StartCoroutine(GhostDisappearSequence()); break;
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

            case "returnToMenu":
                if (GameManager.Instance != null)
                    GameManager.Instance.LoadScene("02. Menu");
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene("02. Menu");
                break;

            case "findTape":
                MissionManager.Instance.StartMission("findTape");
                Debug.Log("[Fase4] Missão findTape iniciada - jogador deve coletar a fita.");
                break;

            case "watchTape":
                MissionManager.Instance.StartMission("watchTape");
                Debug.Log("[Fase4] Missão watchTape iniciada - exibindo conteúdo da fita.");
                break;

            case "ghostAttack":
                MissionManager.Instance.StartMission("ghostAttack");
                Debug.Log("[Fase4] Missão ghostAttack iniciada - fantasma ataca Philippa.");
                break;

            case "ghostDisappear":
                MissionManager.Instance.StartMission("ghostDisappear");
                Debug.Log("[Fase4] Missão ghostDisappear iniciada - fantasma desaparece.");
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
        if (vfx != null) yield return vfx.FadeFromBlack(2f);
        else yield return new WaitForSeconds(2f);

        CompleteMission("fadeIn");
        yield return null;

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
        // não chamar ShowNextLine aqui
    }

    private IEnumerator FindTapeSequence()
    {
        Debug.Log("[Fase4] Fita coletada.");
        if (tapeImage != null) tapeImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        MissionManager.Instance.CompleteMission("findTape");
        DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator WatchTapeSequence()
    {
        Debug.Log("[Fase4] Assistindo a fita…");
        if (telaClickArea != null) telaClickArea.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        MissionManager.Instance.CompleteMission("watchTape");
        DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator GhostAttackSequence()
    {
        Debug.Log("[Fase4] Fantasma Jarvis ataca Philippa!");
        if (jarvisGhost != null) jarvisGhost.SetActive(true);

        yield return new WaitForSeconds(1f);

        MissionManager.Instance.CompleteMission("ghostAttack");
        DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator GhostDisappearSequence()
    {
        Debug.Log("[Fase4] Fantasma Jarvis desaparece.");
        if (jarvisGhost != null) jarvisGhost.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        MissionManager.Instance.CompleteMission("ghostDisappear");
        DialogueManager.Instance.ShowNextLine();
    }
}
