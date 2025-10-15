using UnityEngine;

[RequireComponent(typeof(CursedItem))]
public class CursedItemMission : MonoBehaviour
{
    [Header("C�mera usada para ver o item")]
    public Camera hiddenItemsCamera;

    private CursedItem cursedItem;
    private bool missionCompleted = false;

    void Awake()
    {
        cursedItem = GetComponent<CursedItem>();

        if (cursedItem == null)
            Debug.LogError("[CursedItemMissionChecker] CursedItem n�o encontrado!");

        if (hiddenItemsCamera == null)
            Debug.LogError("[CursedItemMissionChecker] HiddenItemsCamera n�o atribu�da!");
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
                Debug.Log("[CursedItemMissionChecker] Item purificado observado. Miss�o completada!");
                MissionManager.Instance?.CompleteMission("saltCursedObject");
                missionCompleted = true;

                // Desativa o script para n�o disparar de novo
                enabled = false;
            }
        }
    }
}
