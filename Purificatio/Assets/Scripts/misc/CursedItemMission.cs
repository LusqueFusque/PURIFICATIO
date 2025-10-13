using UnityEngine;

[RequireComponent(typeof(CursedItem))]
public class CursedItemMissionChecker : MonoBehaviour
{
    [Header("Câmera usada para ver o item")]
    public Camera hiddenItemsCamera;

    private CursedItem cursedItem;
    private bool missionCompleted = false;

    void Awake()
    {
        cursedItem = GetComponent<CursedItem>();

        if (cursedItem == null)
            Debug.LogError("[CursedItemMissionChecker] CursedItem não encontrado!");

        if (hiddenItemsCamera == null)
            Debug.LogError("[CursedItemMissionChecker] HiddenItemsCamera não atribuída!");
    }

    void Update()
    {
        if (missionCompleted || cursedItem.isCursed) return;

        // Detecta clique esquerdo sobre o item
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = hiddenItemsCamera.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("CursedItem"));
            if (hit != null && hit.gameObject == gameObject)
            {
                Debug.Log("[CursedItemMissionChecker] Item purificado observado. Missão completada!");
                MissionManager.Instance?.CompleteMission("saltCursedObject");
                missionCompleted = true;

                // Desativa o script para não disparar de novo
                enabled = false;
            }
        }
    }
}
