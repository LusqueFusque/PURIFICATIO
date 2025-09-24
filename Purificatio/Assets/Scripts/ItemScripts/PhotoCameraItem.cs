using UnityEngine;

public class PhotoCameraItem : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform cameraRectUI;  // o retângulo de UI que segue o mouse
    public Canvas canvas;               // canvas onde está o retângulo
    public LayerMask hiddenLayer;       // layer dos HiddenItems

    private bool isActive = false;

    void Update()
    {
        if (!isActive) return;

        FollowMouse();
        if (Input.GetMouseButtonDown(1)) Deactivate();

        RevealInsideRect();
    }

    public void Activate()
    {
        isActive = true;
        cameraRectUI.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        isActive = false;
        cameraRectUI.gameObject.SetActive(false);
        Debug.Log("[CameraItem] DeactivateCamera() called.");

        // Preferir MissionManager (novo sistema de missões)
        if (MissionManager.Instance != null)
        {
            if (MissionManager.Instance.IsActive("useCamera"))
            {
                Debug.Log("[CameraItem] MissionManager: completing 'useCamera'.");
                MissionManager.Instance.CompleteMission("useCamera");
            }
            return;
        }

        // Fallback: MissionChecker antigo (callback)
        if (MissionChecker.Instance != null)
        {
            if (MissionChecker.Instance.IsOnMission("useCamera"))
            {
                Debug.Log("[CameraItem] MissionChecker: completing 'useCamera'.");
                MissionChecker.Instance.CompleteMission("useCamera");
            }
        }
    }


    private void FollowMouse()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out mousePos
        );
        cameraRectUI.localPosition = mousePos;
    }

    private void RevealInsideRect()
    {
        // Converter o retângulo UI para coordenadas de mundo
        Vector3 worldCenter = cameraRectUI.position;
        Vector2 size = cameraRectUI.sizeDelta;

        // Pegar todos os colliders 2D que encostam nesse box
        Collider2D[] hits = Physics2D.OverlapBoxAll(worldCenter, size, 0f, hiddenLayer);

        // Primeiro: tudo volta ao normal
        foreach (var item in FindObjectsOfType<HiddenObject>())
            item.ShowNormal();

        // Depois: só os que estão dentro ficam amaldiçoados
        foreach (var h in hits)
        {
            HiddenObject hi = h.GetComponent<HiddenObject>();
            if (hi != null) hi.ShowCursed();
        }
    }

    private void OnDrawGizmos()
    {
        if (cameraRectUI != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(cameraRectUI.position, cameraRectUI.sizeDelta);
        }
    }
}
