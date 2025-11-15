using UnityEngine;
using System.Collections;

public class Fase2MissionHandler : MissionHandlerBase
{
    [Header("Referências do Cenário")]
    public GameObject djinnSceneObject;      // Djinn na cena (imagem 2D)
    public GameObject lampSceneObject;       // Lâmpada mágica aberta/visível
    public GameObject chestObject;           // Baú do depósito
    public GameObject keyObject;             // Chave quebrada
    public GameObject gumObject;             // Chiclete
    public GameObject hammerObject;          // Martelo

    [Header("Áudio")]
    public AudioClip djinnScream;
    public AudioClip lampThrowSfx;

    [Header("Fade")]
    public float fadeDuration = 1.5f;

    public override void HandleMission(string missionId)
    {
        switch (missionId)
        {
            case "fadeIn":
                StartCoroutine(FadeIn());
                break;

            case "FindLamp":
                // Nada a fazer. O item "LampItem" vai chamar CompleteMission.
                break;

            case "throwLamp":
                StartCoroutine(ThrowLamp());
                break;

            case "leaveLamp":
                // Nada especial, o diálogo continua normalmente.
                CompleteMission("leaveLamp");
                break;

            case "fadeOut":
                StartCoroutine(FadeOut());
                break;

            default:
                Debug.LogWarning($"Missão desconhecida: {missionId}");
                break;
        }
    }

    private IEnumerator FadeIn()
    {
        var vfx = FindObjectOfType<VisualEffectsManager>();
        if (vfx != null) yield return vfx.FadeFromBlack(fadeDuration);
        CompleteMission("fadeIn");
        DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator FadeOut()
    {
        var vfx = FindObjectOfType<VisualEffectsManager>();
        if (vfx != null) yield return vfx.FadeToBlack(fadeDuration);
        CompleteMission("fadeOut");
        DialogueManager.Instance.ShowNextLine();
    }

    private IEnumerator ThrowLamp()
    {
        // Djinn grita
        if (djinnScream != null)
            AudioSource.PlayClipAtPoint(djinnScream, Camera.main.transform.position, 0.7f);

        yield return new WaitForSeconds(0.6f);

        var vfx = FindObjectOfType<VisualEffectsManager>();
        if (vfx != null) vfx.RedScreenEffect(0.8f);

        yield return new WaitForSeconds(0.25f);

        if (lampThrowSfx != null)
            AudioSource.PlayClipAtPoint(lampThrowSfx, Camera.main.transform.position, 0.7f);

        // Some com djinn e lâmpada
        if (lampSceneObject != null) lampSceneObject.SetActive(false);
        if (djinnSceneObject != null) djinnSceneObject.SetActive(false);

        yield return new WaitForSeconds(0.4f);

        if (vfx != null) vfx.ClearRedScreen();

        CompleteMission("throwLamp");
        DialogueManager.Instance.ShowNextLine();
    }
}
