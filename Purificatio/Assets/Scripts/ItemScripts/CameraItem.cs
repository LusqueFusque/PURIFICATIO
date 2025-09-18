using UnityEngine;

public class CameraItem : MonoBehaviour
{
    [Header("Configurações da Câmera")]
    public RectTransform cameraRectUI;   // Retângulo do HUD da câmera (UI)
    public Canvas canvas;                 // Canvas onde o retângulo está
    public LayerMask hiddenItemLayer;     // Layer onde os itens ocultos estão
    public float detectionRadius = 50f;   // Raio de detecção dentro do retângulo
    public LayerMask hiddenLayer;


    private bool isActive = false;

    void Update()
    {
        if (!isActive) return;

        FollowMouse();

        if (Input.GetMouseButtonDown(1)) // Botão direito desativa a câmera
        {
            DeactivateCamera();
        }

        DetectHiddenItems();
    }

    // Ativa a câmera
    public void ActivateCamera()
    {
        isActive = true;
        cameraRectUI.gameObject.SetActive(true);
    }

    // Desativa a câmera
    public void DeactivateCamera()
    {
        isActive = false;
        cameraRectUI.gameObject.SetActive(false);
    }

    // Faz o retângulo seguir o mouse
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

    // Detecta itens ocultos dentro do retângulo
    private void DetectHiddenItems()
    {
        // Pega posição do mouse em mundo
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f; // z=0 para 2D

        // Usa tamanho do retângulo como box
        Vector2 size = cameraRectUI.sizeDelta / 100f; // Ajuste da escala se necessário

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

    // Visualização no editor (opcional)
    private void OnDrawGizmos()
    {
        if (cameraRectUI != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(cameraRectUI.position, cameraRectUI.sizeDelta);
        }
    }


}