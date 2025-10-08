using UnityEngine;

public class MaskFollowMouse : MonoBehaviour
{
    public Camera hiddenItemsCamera;

    private Transform maskTransform;

    void Awake()
    {
        maskTransform = transform;
        if (hiddenItemsCamera == null)
        {
            Debug.LogError("O campo 'hiddenItemsCamera' não está definido! Arraste sua câmera para o Inspector.");
        }
    }

    void Update()
    {
        if (hiddenItemsCamera == null) return;

        Vector3 mousePosScreen = Input.mousePosition;
        
        float zDistance = Mathf.Abs(hiddenItemsCamera.transform.position.z - maskTransform.position.z);
       
        mousePosScreen.z = zDistance;
       
        Vector3 worldPosition = hiddenItemsCamera.ScreenToWorldPoint(mousePosScreen);
      
        worldPosition.z = maskTransform.position.z;
       
        maskTransform.position = worldPosition;
    }
}