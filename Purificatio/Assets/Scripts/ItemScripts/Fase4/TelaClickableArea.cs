using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class TelaClickableArea : MonoBehaviour, IPointerClickHandler
{
    [Header("Referências")]
    [Tooltip("Imagem que será ativada quando assistir a fita")]
    public Image telaImage;
    
    [Header("Áudio")]
    [Tooltip("Áudio que será tocado quando assistir a fita")]
    public AudioClip fitaAudio;
    
    [Header("Configurações")]
    [Tooltip("Tempo de espera antes de avançar o diálogo")]
    public float waitTime = 4f;

    private AudioSource audioSource;

    void Start()
    {
        // Cria AudioSource se não existir
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Estado inicial: tela desativada
        if (telaImage != null)
        {
            telaImage.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[TelaClickableArea] Clique detectado em {gameObject.name}");

        // Verifica se a Fita está ativa
        if (FitaItem.Instance != null && FitaItem.Instance.IsActive())
        {
            Debug.Log("[TelaClickableArea] ✓ FitaItem está ativo! Iniciando sequência...");
            
            // Desativa a fita após uso
            FitaItem.Instance.Deactivate();
            
            // Inicia a sequência
            StartCoroutine(PlayTapeSequence());
        }
        else
        {
            if (FitaItem.Instance == null)
                Debug.LogWarning("[TelaClickableArea] FitaItem.Instance é null!");
            else
                Debug.LogWarning("[TelaClickableArea] FitaItem não está ativo! Use a fita primeiro.");
        }
    }

    private IEnumerator PlayTapeSequence()
    {
        Debug.Log("[TelaClickableArea] === INICIANDO SEQUÊNCIA DA FITA ===");

        // 1. Toca o áudio
        if (fitaAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(fitaAudio);
            Debug.Log("[TelaClickableArea] ✓ Áudio tocando...");
        }
        else
        {
            Debug.LogWarning("[TelaClickableArea] Áudio ou AudioSource não configurado!");
        }

        // 2. Ativa a imagem
        if (telaImage != null)
        {
            telaImage.gameObject.SetActive(true);
            Debug.Log("[TelaClickableArea] ✓ Imagem da tela ativada");
        }
        else
        {
            Debug.LogError("[TelaClickableArea] telaImage não configurado no Inspector!");
        }

        // 3. Aguarda o tempo configurado
        Debug.Log($"[TelaClickableArea] Aguardando {waitTime} segundos...");
        yield return new WaitForSeconds(waitTime);

        // 4. Completa a missão watchTape
        if (MissionManager.Instance != null)
        {
            Debug.Log("[TelaClickableArea] ✓ Completando missão watchTape");
            MissionManager.Instance.CompleteMission("watchTape");
        }

        // 5. Vai para o diálogo específico
        if (DialogueManager.Instance != null)
        {
            Debug.Log("[TelaClickableArea] ✓ Indo para diálogo 'rota_assistir2'");
            DialogueManager.Instance.GoToNode("rota_assistir2");
        }
        else
        {
            Debug.LogError("[TelaClickableArea] DialogueManager.Instance é null!");
        }

        Debug.Log("[TelaClickableArea] === SEQUÊNCIA COMPLETA ===");
    }
}