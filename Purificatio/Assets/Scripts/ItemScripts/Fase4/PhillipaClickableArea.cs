using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Detecta clique na Philippa quando a fita está ativa para entregar a fita
/// </summary>
public class PhilippaClickableArea : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuração")]
    [Tooltip("Nome do diálogo que será acionado ao entregar a fita")]
    public string dialogueNodeId = "rota_entrega1";

    private Image thisImage;

    void Start()
    {
        // Valida Image deste GameObject
        thisImage = GetComponent<Image>();
        if (thisImage == null)
        {
            Debug.LogError("[PhilippaClickableArea] ❌ Este GameObject precisa ter um componente IMAGE!");
        }
        else if (!thisImage.raycastTarget)
        {
            Debug.LogWarning("[PhilippaClickableArea] ⚠️ Raycast Target está DESMARCADO! Marque para detectar cliques.");
        }
        
        Debug.Log("[PhilippaClickableArea] ✓ Área clicável de Philippa configurada");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[PhilippaClickableArea] 🖱️ CLIQUE DETECTADO em {gameObject.name}!");
        
        // Verifica se FitaItem está ativo
        if (FitaItem.Instance == null)
        {
            Debug.LogError("[PhilippaClickableArea] ❌ FitaItem.Instance é NULL!");
            return;
        }
        
        bool fitaAtiva = FitaItem.Instance.IsActive();
        Debug.Log($"[PhilippaClickableArea] Fita está ativa? {fitaAtiva}");
        
        if (fitaAtiva)
        {
            Debug.Log("[PhilippaClickableArea] ✅ Entregando fita para Philippa!");
            
            // Vai para o diálogo de entrega primeiro
            if (DialogueManager.Instance != null)
            {
                Debug.Log($"[PhilippaClickableArea] ✓ Indo para diálogo: {dialogueNodeId}");
                DialogueManager.Instance.GoToNode(dialogueNodeId);

                // Só depois desativa a fita e salva decisão
                FitaItem.Instance.Deactivate();
                SaveSystem.Instance.fase4_exorcizou = true;
                SaveSystem.Instance.Salvar();
                Debug.Log("[PhilippaClickableArea] ✓ Exorcismo da Fase 4 registrado.");
            }
            else
            {
                Debug.LogError("[PhilippaClickableArea] ❌ DialogueManager.Instance é NULL!");
            }
        }
        else
        {
            Debug.LogWarning("[PhilippaClickableArea] ❌ Fita não está ativa! Clique no botão da fita no inventário primeiro.");
        }
    }
}
