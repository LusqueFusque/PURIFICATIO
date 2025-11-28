using UnityEngine;

public class PhotoCameraController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject camerafantarma; // O objeto que contem a camera + Sprite Mask

    private bool isActive = false;

    void Update()
    {
        if (!isActive)
            return;

        // Desativa a camera com botao direito
        if (Input.GetMouseButtonDown(1))
        {
            DeactivateCamera();
        }
    }

    // Chamado pelo botao de inventario para ativar a camera
    public void ActivateCamera()
    {
        isActive = true;
        camerafantarma.SetActive(true);
    }

    private void DeactivateCamera()
    {
        isActive = false;
        camerafantarma.SetActive(false);
    }
}
