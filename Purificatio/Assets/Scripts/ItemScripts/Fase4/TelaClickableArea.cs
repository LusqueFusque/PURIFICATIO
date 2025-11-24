using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class TelaClickableArea : MonoBehaviour, IPointerClickHandler
{
    [Header("Referências")]
    [Tooltip("Image que será exibida ao assistir a fita")]
    public Image telaImage;
    
    [Tooltip("Áudio que toca ao assistir a fita")]
    public AudioClip fitaAudio;
    
    [Tooltip("Tempo em segundos antes de avançar o diálogo")]
    public float waitTime = 4f;

    private AudioSource audioSource;
    private Image thisImage;

    void Start()
    {
        // Cria AudioSource se não existir
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        
        // Valida Image deste GameObject
        thisImage = GetComponent<Image>();
        if (thisImage == null)
        {
            Debug.LogError("[TelaClickableArea] ❌ Este GameObject precisa ter um componente IMAGE!");
        }
        else if (!thisImage.raycastTarget)
        {
            Debug.LogWarning("[TelaClickableArea] ⚠️ Raycast Target está DESMARCADO! Marque para detectar cliques.");
        }
        
        // Esconde a imagem de conteúdo
        if (telaImage != null)
        {
            telaImage.gameObject.SetActive(false);
            Debug.Log("[TelaClickableArea] ✓ TelaImage configurada e desativada");
        }
        else
        {
            Debug.LogError("[TelaClickableArea] ❌ TelaImage não configurada no Inspector!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[TelaClickableArea] 🖱️ CLIQUE DETECTADO em {gameObject.name}!");
        
        // Valida FitaItem
        if (FitaItem.Instance == null)
        {
            Debug.LogError("[TelaClickableArea] ❌ FitaItem.Instance é NULL! Certifique-se que FitaItemManager existe na cena.");
            return;
        }
        
        bool fitaAtiva = FitaItem.Instance.IsActive();
        Debug.Log($"[TelaClickableArea] Fita está ativa? {fitaAtiva}");
        
        if (fitaAtiva)
        {
            Debug.Log("[TelaClickableArea] ✅ Iniciando sequência da fita!");
            FitaItem.Instance.Deactivate();
            StartCoroutine(PlayTapeSequence());
        }
        else
        {
            Debug.LogWarning("[TelaClickableArea] ❌ Use a FITA no inventário primeiro!");
        }
    }

    private IEnumerator PlayTapeSequence()
    {
        Debug.Log("[TelaClickableArea] ═══════════════════════════");
        Debug.Log("[TelaClickableArea] 🎬 SEQUÊNCIA DA FITA INICIADA");
        Debug.Log("[TelaClickableArea] ═══════════════════════════");
    
        // 1. Toca áudio
        if (fitaAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(fitaAudio);
            Debug.Log("[TelaClickableArea] ✓ Áudio tocando");
        }

        // 2. Ativa imagem
        if (telaImage != null)
        {
            telaImage.gameObject.SetActive(true);
            Debug.Log("[TelaClickableArea] ✓ Imagem do conteúdo ATIVADA");
        }

        // 3. Espera
        Debug.Log($"[TelaClickableArea] ⏳ Esperando {waitTime} segundos...");
        yield return new WaitForSeconds(waitTime);

        // 4. Completa missão
        Debug.Log("[TelaClickableArea] ✓ Completando missão 'watchTape'");
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission("watchTape");
        }

        // 5. Vai direto para a escolha após assistir
        Debug.Log("[TelaClickableArea] ✓ Indo para 'escolha_apos_assistir'");
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.GoToNode("escolha_apos_assistir");
        }
    
        Debug.Log("[TelaClickableArea] ═══════════════════════════");
        Debug.Log("[TelaClickableArea] ✅ SEQUÊNCIA CONCLUÍDA");
        Debug.Log("[TelaClickableArea] ═══════════════════════════");
    }
}