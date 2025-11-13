using UnityEngine;

public class SaltItem : MonoBehaviour
{
    [Header("Configurações do sal")]
    public int maxUses = 3;
    public LayerMask cursedLayer;
    public Camera targetCamera;

    [Header("Tag da boneca")]
    public string bonecaTag = "Boneca"; // ✅ NOVO

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

        if (hit.collider != null)
        {
            // ✅ VERIFICA SE É A BONECA
            if (hit.collider.CompareTag(bonecaTag))
            {
                CheckDollExorcism(hit.collider.gameObject);
                return;
            }

            // Comportamento normal para outros objetos amaldiçoados
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

    // ✅ NOVO MÉTODO: Verifica se deve exorcizar a boneca diretamente
    private void CheckDollExorcism(GameObject doll)
    {
        Debug.Log("========================================");
        Debug.Log("[SaltItem] 🧂 SAL USADO NA BONECA!");

        if (MissionManager.Instance == null)
        {
            Debug.LogError("[SaltItem] ❌ MissionManager não encontrado!");
            return;
        }

        // Verifica se a missão findDoll foi completada (boneca consertada)
        bool dollWasFixed = MissionManager.Instance.IsCompleted("findDoll");

        Debug.Log($"[SaltItem] Boneca foi consertada? {dollWasFixed}");

        if (!dollWasFixed)
        {
            // ✅ BONECA NÃO FOI CONSERTADA - EXORCISMO DIRETO
            Debug.Log("[SaltItem] ⚡ Boneca NÃO consertada! Iniciando exorcismo imediato!");

            // Desativa a boneca visualmente
            doll.SetActive(false);

            // Usa o sal
            remainingUses--;
            Debug.Log($"[SaltItem] Sal usado. Restam {remainingUses} usos.");

            // ✅ DISPARA O EXORCISMO VIA FASE1MISSIONHANDLER
            var missionHandler = FindObjectOfType<Fase1MissionHandler>();
            if (missionHandler != null)
            {
                Debug.Log("[SaltItem] 🔥 Chamando HandleMission('exorcismoDaBoneca')");
                missionHandler.HandleMission("exorcismoDaBoneca");
            }
            else
            {
                Debug.LogError("[SaltItem] ❌ Fase1MissionHandler não encontrado!");
            }

            // Desequipa o sal
            Unequip();
        }
        else
        {
            // Boneca já foi consertada - comportamento normal
            Debug.Log("[SaltItem] ℹ️ Boneca já foi consertada. Usando sal normalmente.");
            
            var cursed = doll.GetComponent<CursedItem>();
            if (cursed != null && cursed.isCursed)
            {
                cursed.Purify();
                remainingUses--;
            }
        }

        Debug.Log("========================================");
    }
}