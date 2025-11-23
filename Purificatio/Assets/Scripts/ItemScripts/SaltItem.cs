using UnityEngine;

public class SaltItem : MonoBehaviour
{
    [Header("Configurações do sal")]
    public int maxUses = 3;
    public LayerMask cursedLayer;
    public Camera targetCamera;

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
    }

    public void OnSaltButtonClicked()
    {
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
        Debug.Log($"[SaltItem] Equipado. Usos restantes: {remainingUses}");
    }

    private void Unequip()
    {
        isActive = false;
        Debug.Log("[SaltItem] Desequipado.");
    }

    private void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryUseSalt();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Unequip();
        }
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

        if (hit.collider != null)
        {
            Debug.Log($"[SaltItem] Hit detectado: {hit.collider.name}");
            
            var cursed = hit.collider.GetComponent<CursedItem>();
            if (cursed != null && cursed.isCursed)
            {
                Debug.Log($"[SaltItem] CursedItem encontrado: {cursed.gameObject.name}");
                
                // Verifica se é o item Mazzi durante a missão SaltMazzi
                bool isMazziMission = MissionManager.Instance != null && 
                                     MissionManager.Instance.IsActive("SaltMazzi");
                
                Debug.Log($"[SaltItem] Missão SaltMazzi ativa? {isMazziMission}");
                Debug.Log($"[SaltItem] Nome do objeto contém 'Mazzi'? {cursed.gameObject.name.Contains("Mazzi")}");
                
                // NÃO purifica ainda - só depois de verificar
                remainingUses--;
                Debug.Log($"[SaltItem] Purificou {hit.collider.name}. Restam {remainingUses} usos.");

                // Completa a missão apropriada
                if (MissionManager.Instance != null)
                {
                    if (isMazziMission && cursed.gameObject.name.Contains("Mazzi"))
                    {
                        // Completa a missão específica do Mazzi
                        Debug.Log("[SaltItem] ✓ Completando missão SaltMazzi!");
                        MissionManager.Instance.CompleteMission("SaltMazzi");
                    }
                    else
                    {
                        // Completa missão genérica de uso de sal
                        Debug.Log("[SaltItem] Completando missão useSalt");
                        MissionManager.Instance.CompleteMission("useSalt");
                    }
                }

                // Purifica DEPOIS de completar a missão
                cursed.Purify();

                if (remainingUses <= 0)
                {
                    Debug.Log("[SaltItem] O sal acabou e foi desequipado.");
                    Unequip();
                }
            }
            else
            {
                Debug.Log("[SaltItem] O alvo clicado não é amaldiçoado ou CursedItem não encontrado.");
            }
        }
        else
        {
            Debug.Log("[SaltItem] Nenhum alvo atingido no raycast.");
        }
    }
}