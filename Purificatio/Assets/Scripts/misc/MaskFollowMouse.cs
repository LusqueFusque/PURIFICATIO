using UnityEngine;

public class MaskFollowMouse : MonoBehaviour
{
    public Camera hiddenItemsCamera; // A c�mera onde os itens ocultos est�o
    private Transform maskTransform;

    void Awake()
    {
        maskTransform = transform;

        if (hiddenItemsCamera == null)
            Debug.LogError("HiddenItemsCamera n�o atribu�da!");
    }

    void Update()
    {
        if (hiddenItemsCamera == null) return;

        // Pega a posi��o do mouse em pixels na tela
        Vector3 mouseScreenPos = Input.mousePosition;

        // Define a dist�ncia da m�scara para a c�mera
        mouseScreenPos.z = Mathf.Abs(hiddenItemsCamera.transform.position.z - maskTransform.position.z);

        // Converte a posi��o da tela para posi��o no mundo da c�mera da m�scara
        Vector3 mouseWorldPos = hiddenItemsCamera.ScreenToWorldPoint(mouseScreenPos);

        // Move a m�scara
        maskTransform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, maskTransform.position.z);
    }
}
