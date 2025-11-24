using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HolyWaterItem : MonoBehaviour
{
    public static HolyWaterItem Instance;

    [Header("ItemData")]
    public ItemData holyWaterData; // arraste o ItemData da Água Benta

    [Header("Configuração")]
    public Image mazzikinImage; // referência à imagem do Mazzi
    public AudioClip activateSound;
    public AudioClip useSound;

    private bool isActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    // ============================================
    // INVENTÁRIO
    // ============================================
    public void OnHolyWaterCollected()
    {
        Debug.Log("[HolyWater] Coletada!");
        var inv = FindObjectOfType<DynamicInventory>();
        if (inv != null)
            inv.AddItem(holyWaterData);
        else
            Debug.LogError("Inventário não encontrado!");
    }

    // ============================================
    // CONTROLE DE ATIVAÇÃO
    // ============================================
    public void Activate()
    {
        isActive = true;
        Debug.Log("[HolyWater] ATIVADA");
        if (activateSound != null)
            AudioSource.PlayClipAtPoint(activateSound, Camera.main.transform.position, 0.6f);
    }

    public void Deactivate()
    {
        isActive = false;
        Debug.Log("[HolyWater] DESATIVADA");
    }

    public void Toggle()
    {
        if (isActive) Deactivate();
        else Activate();
    }

    public bool IsActive() => isActive;

    // ============================================
    // USO NO MAZZIKIN
    // ============================================
    public void UseHolyWater()
    {
        if (!isActive) return;

        Debug.Log("[HolyWater] Usando Água Benta no Mazzi...");
        if (useSound != null)
            AudioSource.PlayClipAtPoint(useSound, Camera.main.transform.position, 1f);

        // dispara a sequência de fade-out
        var fase3 = FindObjectOfType<Fase3MissionHandler>();
        if (fase3 != null)
            fase3.StartCoroutine(FadeOutMazzikin(fase3));
        else
            Debug.LogError("[HolyWater] Nenhum Fase3MissionHandler encontrado!");

        Deactivate(); // consome o item
    }

    private IEnumerator FadeOutMazzikin(Fase3MissionHandler fase3)
    {
        if (mazzikinImage != null)
        {
            float duration = 1.2f;
            float elapsed = 0f;
            Color startColor = mazzikinImage.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                mazzikinImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            mazzikinImage.gameObject.SetActive(false);
        }

        // completa missão
        if (MissionManager.Instance != null)
            MissionManager.Instance.CompleteMission("holyWaterMazzi");

        // segue diálogo
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowNextLine();
    }
}
