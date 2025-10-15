using UnityEngine;

public class PhotoCameraController : MonoBehaviour
{
    [Header("Referências")]
    public GameObject camerafantarma; // O objeto que contém a câmera + Sprite Mask

    private bool isActive = false;

    void Update()
    {
        if (!isActive)
            return;

        // Desativa a câmera com botão direito
        if (Input.GetMouseButtonDown(1))
        {
            DeactivateCamera();
        }
    }

    // Chamado pelo botão de inventário para ativar a câmera
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
