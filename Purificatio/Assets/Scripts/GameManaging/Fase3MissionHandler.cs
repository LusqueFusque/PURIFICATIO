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
        SaveSystem.Instance.fase3_exorcizou = false;
        SaveSystem.Instance.Salvar();

        
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
            
            case "MazziOut":
                MissionManager.Instance.StartMission("MazziOut");
                HandleMazziOut();
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
            
            case "fadeOut":
                StartCoroutine(FadeOutSequence());
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
    
    public void HolyWaterMazzi()
    {
        StartCoroutine(HolyWaterSequence());
    }

    private IEnumerator HolyWaterSequence()
    {
        Debug.Log("[Fase3] Água benta usada no Mazzi.");

        // se quiser som separado:
        // AudioSource.PlayClipAtPoint(holyWaterSfx, Camera.main.transform.position, 1f);

        yield return new WaitForSeconds(0.2f);

        if (demonMazzi != null)
            demonMazzi.SetActive(false);

        if (MissionManager.Instance != null)
            MissionManager.Instance.CompleteMission("holyWaterMazzi");

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
    
    // =============================================
// EXORCISMO DO MAZZIKIN (chamado pela ArmaSanta)
// =============================================
    [Header("Exorcismo")]
    public AudioClip exorcismSfx;  // som configurável no Inspector
    public float exorcismHideDelay = 0.2f;

    public void ExorciseMazzi()
    {
        StartCoroutine(ExorcismSequence());
    }

    private IEnumerator ExorcismSequence()
    {
        Debug.Log("[Fase3] Exorcismo iniciado.");

        // Som
        if (exorcismSfx != null)
            AudioSource.PlayClipAtPoint(exorcismSfx, Camera.main.transform.position, 1f);

        // pequeno delay dramático
        yield return new WaitForSeconds(exorcismHideDelay);

        // desaparece o demon Mazzi
        if (demonMazzi != null)
            demonMazzi.SetActive(false);

        Debug.Log("[Fase3] Mazzi removido da cena.");

        // completa missão
        if (MissionManager.Instance != null)
            MissionManager.Instance.CompleteMission("exorcismoMazzi");

        // segue diálogo
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
        
        SaveSystem.Instance.fase3_exorcizou = true;
        SaveSystem.Instance.Salvar();

        
    }
    
    
    // ==================== FADE OUT ====================
    private IEnumerator FadeOutSequence()
    {
        VisualEffectsManager vfx = GetEffectsManager();
        float fadeDuration = 1.2f;
        if (vfx != null)
            yield return StartCoroutine(vfx.FadeToBlack(fadeDuration));
        else
            yield return new WaitForSeconds(fadeDuration);

        // Marca missão concluída
        CompleteMission("fadeOut");

        // Aguarda 1 frame
        yield return null;

        // ❌ Não chamar ShowNextLine aqui
        // O diálogo continua normalmente pelo DialogueManager
    }
    
    [Header("MazziOut")]
    public float mazziOutFadeDuration = 1.2f;
    public AudioClip mazziOutSfx;

    public void HandleMazziOut()
    {
        StartCoroutine(MazziOutSequence());
    }

    private IEnumerator MazziOutSequence()
    {
        Debug.Log("[Fase3] Iniciando MazziOutSequence...");

        if (mazziOutSfx != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(mazziOutSfx, Camera.main.transform.position, 1f);

        if (demonMazzi != null)
        {
            Image img = demonMazzi.GetComponent<Image>();
            SpriteRenderer sr = demonMazzi.GetComponent<SpriteRenderer>();
            CanvasGroup cg = demonMazzi.GetComponent<CanvasGroup>();

            if (cg == null) cg = demonMazzi.AddComponent<CanvasGroup>();
            cg.interactable = false;
            cg.blocksRaycasts = false;

            float elapsed = 0f;
            while (elapsed < mazziOutFadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / mazziOutFadeDuration);
                cg.alpha = alpha;

                if (img != null) { var c = img.color; c.a = alpha; img.color = c; }
                if (sr != null) { var c = sr.color; c.a = alpha; sr.color = c; }

                yield return null;
            }

            demonMazzi.SetActive(false);
        }

        CompleteMission("MazziOut");

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
        
        SaveSystem.Instance.fase3_exorcizou = false;
        SaveSystem.Instance.Salvar();

    }

}
