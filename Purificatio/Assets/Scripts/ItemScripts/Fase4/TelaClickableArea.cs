using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class TelaClickableArea : MonoBehaviour, IPointerClickHandler
{
    public Image telaImage;
    public AudioClip fitaAudio;
    public float waitTime = 4f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        if (telaImage != null) telaImage.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (FitaItem.Instance != null && FitaItem.Instance.IsActive())
        {
            FitaItem.Instance.Deactivate();
            StartCoroutine(PlayTapeSequence());
        }
        else
        {
            Debug.LogWarning("[TelaClickableArea] Fita não está ativa!");
        }
    }

    private IEnumerator PlayTapeSequence()
    {
        if (fitaAudio != null)
            audioSource.PlayOneShot(fitaAudio);

        if (telaImage != null)
            telaImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(waitTime);

        // Completa missão
        MissionManager.Instance?.CompleteMission("watchTape");

        // Avança diálogo
        DialogueManager.Instance?.GoToNode("rota_assistir2");
    }
}