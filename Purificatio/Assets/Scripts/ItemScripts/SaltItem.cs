using UnityEngine;

public class SaltItem : MonoBehaviour
{
    [Header("Configurações do sal")]
    public int maxUses = 3;
    public LayerMask cursedLayer;
    public Camera targetCamera;

    [Header("Áudio")]
    public AudioClip saltUseSound;
    public AudioSource audioSource2D; // arraste um AudioSource 2D aqui

    private bool isActive = false;
    private int remainingUses;

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

        if (audioSource2D == null)
        {
            audioSource2D = gameObject.AddComponent<AudioSource>();
            audioSource2D.playOnAwake = false;
            audioSource2D.spatialBlend = 0f; // 2D
            audioSource2D.volume = 1f;
        }
    }
    private void Update()
    {
        if (!isActive) return;

        // Clique esquerdo usa o sal
        if (Input.GetMouseButtonDown(0))
        {
            TryUseSalt();
        }

        // Clique direito desativa
        if (Input.GetMouseButtonDown(1))
        {
            Unequip();
        }
    }

    // Chamado pelo botão de inventário
    public void OnSaltButtonClicked()
    {
        if (isActive) Unequip();
        else Equip();
    }

    // Chamado pelo botão de uso (ex: clicar na área amaldiçoada)
    public void OnSaltUseClicked()
    {
        if (!isActive)
        {
            Debug.Log("[SaltItem] ✖ Não está ativo, não pode usar.");
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
        if (targetCamera == null)
        {
            Debug.LogError("[SaltItem] Câmera inválida!");
            return;
        }

        Vector2 worldPos = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, cursedLayer);

        Debug.Log($"[SaltItem] Tentando usar sal em: {worldPos}");

        // 🔊 Som de uso
        if (saltUseSound != null && audioSource2D != null)
            audioSource2D.PlayOneShot(saltUseSound, 0.9f);

        if (hit.collider != null)
        {
            var cursed = hit.collider.GetComponent<CursedItem>();
            if (cursed != null && cursed.isCursed)
            {
                bool isMazziMission = MissionManager.Instance != null &&
                                      MissionManager.Instance.IsActive("SaltMazzi");

                remainingUses--;
                Debug.Log($"[SaltItem] Purificou {hit.collider.name}. Restam {remainingUses} usos.");

                if (MissionManager.Instance != null)
                {
                    if (isMazziMission && cursed.gameObject.name.Contains("Mazzi"))
                    {
                        MissionManager.Instance.CompleteMission("SaltMazzi");
                    }
                    else
                    {
                        MissionManager.Instance.CompleteMission("useSalt");
                    }
                }

                cursed.Purify();

                if (remainingUses <= 0)
                {
                    Debug.Log("[SaltItem] O sal acabou e foi desequipado.");
                    Unequip();
                }
            }
            else
            {
                Debug.Log("[SaltItem] O alvo clicado não é amaldiçoado.");
            }
        }
        else
        {
            Debug.Log("[SaltItem] Nenhum alvo atingido.");
        }
    }
}
