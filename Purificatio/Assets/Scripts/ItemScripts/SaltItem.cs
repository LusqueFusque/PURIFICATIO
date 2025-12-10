using UnityEngine;
using UnityEngine.UI;

public class SaltItem : MonoBehaviour
{
    [Header("Configurações do sal")]
    public int maxUses = 3;
    public LayerMask cursedLayer;
    public Camera targetCamera;
    [Tooltip("RawImage que mostra a RenderTexture da câmera")]
    public RawImage renderTextureImage;

    [Header("Efeito Visual")]
    public ParticleSystem saltEffectPrefab; // ⬅ Particle System

    [Header("Áudio")]
    public AudioClip saltUseSound;
    public AudioSource audioSource2D;

    private bool isActive = false;
    private int remainingUses;
    private RectTransform rawImageRect;

    private void Start()
    {
        remainingUses = maxUses;

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                Debug.LogError("[SaltItem] Nenhuma câmera encontrada! Item desativado.");
                enabled = false;
                return;
            }
            Debug.LogWarning("[SaltItem] targetCamera não atribuída. Usando Camera.main.");
        }
        
        if (renderTextureImage != null)
        {
            rawImageRect = renderTextureImage.rectTransform;
        }
        else
        {
            Debug.LogWarning("[SaltItem] renderTextureImage não atribuída. Converter coordenadas pode falhar!");
        }

        if (audioSource2D == null)
        {
            audioSource2D = gameObject.AddComponent<AudioSource>();
            audioSource2D.playOnAwake = false;
            audioSource2D.spatialBlend = 0f;
        }
    }

    private void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(0))
            TryUseSalt();

        if (Input.GetMouseButtonDown(1))
            Unequip();
    }

    public void OnSaltButtonClicked()
    {
        if (isActive) Unequip();
        else Equip();
    }

    public void OnSaltUseClicked()
    {
        if (!isActive)
        {
            Debug.Log("[SaltItem] ✖ Não está ativo.");
            return;
        }

        TryUseSalt();
    }

    private void Equip()
    {
        if (remainingUses <= 0)
        {
            Debug.Log("[SaltItem] O sal acabou!");
            return;
        }

        isActive = true;
        Debug.Log($"[SaltItem] ✓ Equipado. Usos restantes: {remainingUses}");
    }

    private void Unequip()
    {
        isActive = false;
        Debug.Log("[SaltItem] ✗ Desequipado.");
    }

    private void TryUseSalt()
    {
        if (rawImageRect == null)
        {
            Debug.LogError("[SaltItem] renderTextureImage não setado!");
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImageRect, Input.mousePosition, null, out Vector2 localPoint))
        {
            return;
        }

        Rect rect = rawImageRect.rect;
        Vector2 viewportPoint = new Vector2(
            (localPoint.x - rect.x) / rect.width,
            (localPoint.y - rect.y) / rect.height
        );

        Vector3 worldPos = targetCamera.ViewportToWorldPoint(viewportPoint);
        worldPos.z = 0f;

        // 🎇 Dispara o efeito no ponto clicado
        //SpawnSaltVFX(worldPos);
        saltEffectPrefab.transform.position = worldPos;
        saltEffectPrefab.Play();

        // 🔊 Som de uso
        if (saltUseSound != null)
            audioSource2D.PlayOneShot(saltUseSound, 0.9f);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, cursedLayer);

        if (hit.collider == null)
        {
            Debug.Log("[SaltItem] Nenhum alvo atingido.");
            return;
        }

        var cursed = hit.collider.GetComponent<CursedItem>();
        if (cursed == null || !cursed.isCursed)
        {
            Debug.Log("[SaltItem] O alvo clicado não é amaldiçoado.");
            return;
        }

        bool isMazziMission = MissionManager.Instance != null &&
                              MissionManager.Instance.IsActive("SaltMazzi");

        remainingUses--;
        Debug.Log($"[SaltItem] Purificou {hit.collider.name}. Restam {remainingUses} usos.");

        if (MissionManager.Instance != null)
        {
            if (isMazziMission && cursed.gameObject.name.Contains("Mazzi"))
                MissionManager.Instance.CompleteMission("SaltMazzi");
            else
                MissionManager.Instance.CompleteMission("useSalt");
        }

        cursed.Purify();

        if (remainingUses <= 0)
        {
            Debug.Log("[SaltItem] O sal acabou!");
            Unequip();
        }
    }
}
