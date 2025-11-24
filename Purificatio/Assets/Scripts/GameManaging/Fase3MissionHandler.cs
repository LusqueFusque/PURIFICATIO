using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fase3MissionHandler : MissionHandlerBase
{
    [Header("UI da Fase 3")]
    public Image pentagramImage;
    public Image cursedItemGlow;
    
    [Header("Sprites e Entidades")]
    public GameObject tapeteAuraSprite;
    public GameObject demonMazzi;

    [Header("Itens Especiais")]
    public MazzikinItem mazzikinItem;   // referência ao script novo

    void OnEnable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted += OnMissionCompletedHandler;
        
        CursedItem.OnItemPurified += OnItemPurifiedHandler;
    }

    void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted -= OnMissionCompletedHandler;
        
        CursedItem.OnItemPurified -= OnItemPurifiedHandler;
    }

   private void OnItemPurifiedHandler(CursedItem cursedItem)
{
    try
    {
        // pega a instância localmente para evitar chamadas repetidas e null-conditional dentro de interpolação
        var mm = MissionManager.Instance;

        bool saltMazziActive = false;
        bool useSaltActive = false;

        if (mm != null)
        {
            // chama de forma segura
            saltMazziActive = mm.IsActive("SaltMazzi");
            useSaltActive = mm.IsActive("useSalt");
        }

        Debug.Log("[Fase3] Purificado: " + cursedItem.name + " | isMazziItem=" + cursedItem.isMazziItem + " | SaltMazzi ativa=" + saltMazziActive);
        Debug.Log("[Fase3] useSalt ativa=" + useSaltActive);
        Debug.Log("[Fase3] mazzikinItem é NULL? " + (mazzikinItem == null));

        // Considera ambos os IDs possíveis
        bool isMazziMission = mm != null && (saltMazziActive || useSaltActive);

        // fallback: se é Mazzi e a missão não estava ativa, tenta ativar SaltMazzi automaticamente
        if (cursedItem.isMazziItem && mm != null && !isMazziMission)
        {
            Debug.Log("[Fase3] Item é Mazzi mas missão não estava ativa — tentando StartMission(\"SaltMazzi\") como fallback.");
            mm.StartMission("SaltMazzi");

            // reavaliar flags
            saltMazziActive = mm.IsActive("SaltMazzi");
            useSaltActive = mm.IsActive("useSalt");
            isMazziMission = saltMazziActive || useSaltActive;

            Debug.Log("[Fase3] Após fallback, SaltMazzi ativa=" + saltMazziActive + ", useSalt ativa=" + useSaltActive);
        }

        if (isMazziMission && cursedItem.isMazziItem)
        {
            Debug.Log("[Fase3] Chamando RevealMazzikin()");
            mazzikinItem?.RevealMazzikin();
        }
        else
        {
            StartCoroutine(SaltSequence());
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError("[Fase3] Erro em OnItemPurifiedHandler: " + ex.Message + "\n" + ex.StackTrace);
    }
}

    private void OnMissionCompletedHandler(string missionId)
    {
        Debug.Log($"[Fase3] Missão completa: {missionId}");

        switch (missionId)
        {
            case "RevealPentagram": StartCoroutine(RevealPentagramSequence()); break;
            case "SummonMazzi": StartCoroutine(SummonMazziSequence()); break;
        }
    }

    public override void HandleMission(string missionId)
    {
        switch (missionId)
        {
            case "RevealPentagram":
                MissionManager.Instance.StartMission("RevealPentagram");
                break;

            case "SaltMazzi":
                MissionManager.Instance.StartMission("SaltMazzi");
                Debug.Log("[Fase3] Missão SaltMazzi iniciada - aguardando uso do sal");
                break;

            case "SummonMazzi":
                MissionManager.Instance.StartMission("SummonMazzi");
                break;

            case "fadeIn":  
                StartCoroutine(FadeInSequence());
                break;

            case "returnToMenu":
                if (GameManager.Instance != null)
                    GameManager.Instance.LoadScene("02. Menu");
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene("02. Menu");
                break;

            default:
                Debug.LogWarning($"[Fase3] Missão desconhecida: {missionId}");
                break;
        }
    }

    private IEnumerator FadeInSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();
        if (vfx != null) yield return vfx.FadeFromBlack(2f);
        else yield return new WaitForSeconds(2f);

        CompleteMission("fadeIn");
        yield return null;

        DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator RevealPentagramSequence()
    {
        if (pentagramImage != null) pentagramImage.gameObject.SetActive(true);
        if (cursedItemGlow != null) cursedItemGlow.gameObject.SetActive(true);
        if (tapeteAuraSprite != null) tapeteAuraSprite.SetActive(true);

        yield return null;
        DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator SaltSequence()
    {
        yield return new WaitForSeconds(0.2f);

        if (cursedItemGlow != null) cursedItemGlow.gameObject.SetActive(false);
        if (tapeteAuraSprite != null) tapeteAuraSprite.SetActive(false);

        if (MissionManager.Instance != null && MissionManager.Instance.IsActive("useSalt"))
            MissionManager.Instance.CompleteMission("useSalt");

        yield return new WaitForSeconds(0.3f);

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator SummonMazziSequence()
    {
        yield return new WaitForSeconds(0.4f);

        if (demonMazzi != null)
            demonMazzi.SetActive(true);

        yield return null;
        DialogueManager.Instance.ShowNextLine();
    }
}
