using UnityEngine;

public class CameraItem : MonoBehaviour
{
    [Header("Configura��es da C�mera")]
    public RectTransform cameraRectUI;   // Ret�ngulo do HUD da c�mera (UI)
    public Canvas canvas;                 // Canvas onde o ret�ngulo est�
    public LayerMask hiddenItemLayer;     // Layer onde os itens ocultos est�o
    public float detectionRadius = 50f;   // Raio de detec��o dentro do ret�ngulo
    public LayerMask hiddenLayer;


    private bool isActive = false;

    void Update()
    {
        if (!isActive) return;

        FollowMouse();

        if (Input.GetMouseButtonDown(1)) // Bot�o direito desativa a c�mera
        {
            DeactivateCamera();
        }

        DetectHiddenItems();
    }

    // Ativa a c�mera
    public void ActivateCamera()
    {
        isActive = true;
        cameraRectUI.gameObject.SetActive(true);
    }

    // Desativa a c�mera
    public void DeactivateCamera()
    {
        isActive = false;
        cameraRectUI.gameObject.SetActive(false);
    }

    // Faz o ret�ngulo seguir o mouse
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

    // Detecta itens ocultos dentro do ret�ngulo
    private void DetectHiddenItems()
    {
        // Pega posi��o do mouse em mundo
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f; // z=0 para 2D

        // Usa tamanho do ret�ngulo como box
        Vector2 size = cameraRectUI.sizeDelta / 100f; // Ajuste da escala se necess�rio

        Collider2D[] hits = Physics2D.OverlapBoxAll(worldPos, size, 0f, hiddenItemLayer);

        foreach (var hit in hits)
        {
            HiddenObject item = hit.GetComponent<HiddenObject>();
            if (item != null)
            {
                item.Reveal();
            }
        }
}

    // Visualiza��o no editor (opcional)
    private void OnDrawGizmos()
    {
        if (cameraRectUI != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(cameraRectUI.position, cameraRectUI.sizeDelta);
        }
    }


}