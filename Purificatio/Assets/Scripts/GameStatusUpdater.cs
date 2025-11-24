using UnityEngine;
using TMPro;

/// <summary>
/// Atualiza os textos de status das fases com base nas decisões salvas no SaveSystem.
/// Funciona com GameObjects que possuem componentes TextMeshProUGUI.
/// </summary>
public class GameStatusUpdater : MonoBehaviour
{
    [Header("Referências de Status (GameObjects com TMP)")]
    public GameObject fase1Status;
    public GameObject fase2Status;
    public GameObject fase3Status;
    public GameObject fase4Status;

    void Start()
    {
        AtualizarStatus();
    }

    public void AtualizarStatus()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogWarning("[StatusUpdaterTMP] SaveSystem.Instance é NULL!");
            return;
        }

        AtualizarTexto(fase1Status, SaveSystem.Instance.fase1_exorcizou);
        AtualizarTexto(fase2Status, SaveSystem.Instance.fase2_exorcizou);
        AtualizarTexto(fase3Status, SaveSystem.Instance.fase3_exorcizou);
        AtualizarTexto(fase4Status, SaveSystem.Instance.fase4_exorcizou);

        Debug.Log("[StatusUpdaterTMP] Status das fases atualizado com sucesso.");
    }

    private void AtualizarTexto(GameObject statusObj, bool exorcizou)
    {
        if (statusObj == null)
        {
            Debug.LogWarning("[StatusUpdaterTMP] GameObject de status está NULL!");
            return;
        }

        TextMeshProUGUI tmp = statusObj.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogWarning($"[StatusUpdaterTMP] GameObject '{statusObj.name}' não possui TextMeshProUGUI!");
            return;
        }

        tmp.text = exorcizou ? "Exorcismo concluído" : "Exorcismo não realizado";
    }
}