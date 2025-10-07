using UnityEngine;

public class PhotoCameraController : MonoBehaviour
{
    [Header("Refer�ncias")]
    public GameObject cameraMaskObject; // O objeto que cont�m a c�mera + Sprite Mask

    private bool isActive = false;

    void Update()
    {
        if (!isActive)
            return;

        // Desativa a c�mera com bot�o direito
        if (Input.GetMouseButtonDown(1))
        {
            DeactivateCamera();
        }
    }

    // Chamado pelo bot�o de invent�rio para ativar a c�mera
    public void ActivateCamera()
    {
        isActive = true;
        cameraMaskObject.SetActive(true);
    }

    private void DeactivateCamera()
    {
        isActive = false;
        cameraMaskObject.SetActive(false);
    }
}
