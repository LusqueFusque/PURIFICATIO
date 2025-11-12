using UnityEngine;
using UnityEngine.UI;

public class MinimapButton : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Nome exato da sala (ex: 'Sala', 'Quarto')")]
    public string roomName;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError($"[MinimapButton] Botão não encontrado em {gameObject.name}!");
        }
    }

    void OnButtonClick()
    {
        Debug.Log($"[MinimapButton] Botão clicado! Mudando para: {roomName}");
        
        if (AdvancedMapManager.Instance != null)
        {
            AdvancedMapManager.Instance.ChangeRoom(roomName);
        }
        else
        {
            Debug.LogError("[MinimapButton] AdvancedMapManager.Instance é NULL!");
        }
    }
}