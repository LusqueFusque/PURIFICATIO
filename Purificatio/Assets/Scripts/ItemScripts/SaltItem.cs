using UnityEngine;

public class SaltItem : MonoBehaviour
{
    [Header("Configurações do sal")]
    public int maxUses = 3;
    public LayerMask cursedLayer; // layer dos sprites amaldiçoados
    public Camera targetCamera;   // câmera que vê os sprites amaldiçoados (pode ser a principal)

    private bool isActive = false;
    private int remainingUses;

    private void Start()
    {
        remainingUses = maxUses;
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    public void OnSaltButtonClicked()
    {
        // Equipa / Desequipa
        if (isActive)
        {
            Unequip();
        }
        else
        {
            Equip();
        }
    }

    private void Equip()
    {
        if (remainingUses <= 0)
        {
            Debug.Log("[SaltItem] O sal acabou!");
            return;
        }

        isActive = true;
        Debug.Log("[SaltItem] Equipado. Usos restantes: " + remainingUses);
    }

    private void Unequip()
    {
        isActive = false;
        Debug.Log("[SaltItem] Desequipado.");
    }

    private void Update()
    {
        if (!isActive) return;

        // Clique esquerdo = tentar purificar
        if (Input.GetMouseButtonDown(0))
        {
            TryUseSalt();
        }

        // Clique direito = desequipar manualmente
        if (Input.GetMouseButtonDown(1))
        {
            Unequip();
        }
    }

    private void TryUseSalt()
    {
        Vector2 worldPos = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, cursedLayer);

        if (hit.collider != null)
        {
            var cursed = hit.collider.GetComponent<CursedItem>();
            if (cursed != null && cursed.isCursed)
            {
                cursed.Purify();
                remainingUses--;

                Debug.Log($"[SaltItem] Purificou {hit.collider.name}. Restam {remainingUses} usos.");

                if (MissionManager.Instance != null)
                    MissionManager.Instance.CompleteMission("useSalt");

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
