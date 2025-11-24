using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fase3MissionHandler : MissionHandlerBase
{
    [Header("UI da Fase 3")]
    public Image pentagramImage;
    public Image cursedItemGlow;
    public Image mazzikinImage;
    
    [Header("Sprite (única exceção)")]
    public GameObject tapeteAuraSprite;

    [Header("Entidade")]
    public GameObject demonMazzi;

    [Header("Sons")]
    public AudioClip revealSound;
    public AudioClip saltSound;
    public AudioClip demonAppearSound;

    [Header("Configurações de Fade")]
    public float mazziFadeDuration = 1.5f;

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
        Debug.Log($"[Fase3] Item purificado: {cursedItem.name}, IsMazziItem: {cursedItem.isMazziItem}");

        bool isMazziMission = MissionManager.Instance != null && 
                              MissionManager.Instance.IsActive("SaltMazzi");

        if (isMazziMission && cursedItem.isMazziItem)
            StartCoroutine(SaltMazziSequence());
        else
            StartCoroutine(SaltSequence());
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

            // ✅ Só aqui voltamos ao menu, quando o JSON manda "returnToMenu"
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
        if (revealSound != null)
            AudioSource.PlayClipAtPoint(revealSound, Camera.main.transform.position, 0.5f);

        if (pentagramImage != null) pentagramImage.gameObject.SetActive(true);
        if (cursedItemGlow != null) cursedItemGlow.gameObject.SetActive(true);
        if (tapeteAuraSprite != null) tapeteAuraSprite.SetActive(true);

        yield return null;
        DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator SaltSequence()
    {
        if (saltSound != null)
            AudioSource.PlayClipAtPoint(saltSound, Camera.main.transform.position, 0.5f);

        yield return new WaitForSeconds(0.2f);

        if (cursedItemGlow != null) cursedItemGlow.gameObject.SetActive(false);
        if (tapeteAuraSprite != null) tapeteAuraSprite.SetActive(false);

        if (MissionManager.Instance != null && MissionManager.Instance.IsActive("useSalt"))
            MissionManager.Instance.CompleteMission("useSalt");

        yield return new WaitForSeconds(0.3f);

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator SaltMazziSequence()
    {
        if (saltSound != null)
            AudioSource.PlayClipAtPoint(saltSound, Camera.main.transform.position, 0.5f);

        yield return new WaitForSeconds(0.2f);

        if (cursedItemGlow != null)
            cursedItemGlow.gameObject.SetActive(false);

        if (tapeteAuraSprite != null)
            tapeteAuraSprite.SetActive(false);

        if (MissionManager.Instance != null && MissionManager.Instance.IsActive("SaltMazzi"))
            MissionManager.Instance.CompleteMission("SaltMazzi");

        yield return new WaitForSeconds(0.3f);

        // ✅ Ativa e posiciona o Mazzi direto
        if (mazzikinImage != null)
        {
            mazzikinImage.gameObject.SetActive(true);
            RectTransform rt = mazzikinImage.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.localPosition = new Vector3(-27f, 89f, 0.4646343f);
                Debug.Log("[Fase3] Mazzi ativado e posicionado em (-27, 89, 0.4646343)");
            }
        }

        if (demonAppearSound != null)
            AudioSource.PlayClipAtPoint(demonAppearSound, Camera.main.transform.position, 0.6f);

        yield return new WaitForSeconds(0.3f);

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }
    
    private IEnumerator FadeInMazzi()
    {
        if (mazzikinImage == null) yield break;

        CanvasGroup canvasGroup = mazzikinImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = mazzikinImage.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        
        while (elapsed < mazziFadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / mazziFadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator SummonMazziSequence()
    {
        if (demonAppearSound != null)
            AudioSource.PlayClipAtPoint(demonAppearSound, Camera.main.transform.position, 0.8f);

        yield return new WaitForSeconds(0.4f);

        if (demonMazzi != null)
            demonMazzi.SetActive(true);

        yield return null;
        DialogueManager.Instance.ShowNextLine();
    }
}
