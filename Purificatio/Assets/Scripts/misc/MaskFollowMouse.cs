using UnityEngine;

public class MaskFollowMouse : MonoBehaviour
{
    public Camera hiddenItemsCamera; // A câmera onde os itens ocultos estão
    private Transform maskTransform;

    void Awake()
    {
        maskTransform = transform;

        if (hiddenItemsCamera == null)
            Debug.LogError("HiddenItemsCamera não atribuída!");
    }

    void Update()
    {
        if (hiddenItemsCamera == null) return;

        // Pega a posição do mouse em pixels na tela
        Vector3 mouseScreenPos = Input.mousePosition;

        // Define a distância da máscara para a câmera
        mouseScreenPos.z = Mathf.Abs(hiddenItemsCamera.transform.position.z - maskTransform.position.z);

        // Converte a posição da tela para posição no mundo da câmera da máscara
        Vector3 mouseWorldPos = hiddenItemsCamera.ScreenToWorldPoint(mouseScreenPos);

        // Move a máscara
        maskTransform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, maskTransform.position.z);
    }
}
