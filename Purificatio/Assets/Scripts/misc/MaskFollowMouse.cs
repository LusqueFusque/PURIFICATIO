using UnityEngine;
using UnityEngine.UI;

public class MaskFollowMouse : MonoBehaviour
{
    [Tooltip("The camera that renders the hidden items to a RenderTexture.")]
    public Camera hiddenItemsCamera;

    [Tooltip("The RawImage UI element that displays the RenderTexture.")]
    public RawImage renderTextureImage;

    private Transform maskTransform;
    private RectTransform rawImageRect;

    void Start()
    {
        if (hiddenItemsCamera == null)
        {
            Debug.LogError("hiddenItemsCamera is not assigned.");
            enabled = false;
            return;
        }

        if (renderTextureImage == null)
        {
            Debug.LogError("RenderTextureImage is not assigned.");
            enabled = false;
            return;
        }

        maskTransform = transform;
        rawImageRect = renderTextureImage.rectTransform;
    }

    void Update()
    {
        // Convert the mouse's screen position to a local position within the RawImage's RectTransform.
        // The 'camera' parameter is null for Screen Space - Overlay canvas.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImageRect, Input.mousePosition, null, out Vector2 localPoint))
        {
            // Normalize the local point to get viewport coordinates (0 to 1).
            // This accounts for the pivot of the RectTransform.
            Rect rect = rawImageRect.rect;
            Vector2 viewportPoint = new Vector2(
                (localPoint.x - rect.x) / rect.width,
                (localPoint.y - rect.y) / rect.height
            );

            // Convert the viewport point to a world point using the orthographic camera.
            Vector3 mouseWorldPos = hiddenItemsCamera.ViewportToWorldPoint(viewportPoint);

            // Update the mask's position, keeping its original Z coordinate.
            maskTransform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, maskTransform.position.z);
        }
    }
}