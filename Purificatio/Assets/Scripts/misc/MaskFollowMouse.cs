using UnityEngine;

public class MaskFollowMouse : MonoBehaviour
{
    public Camera hiddenItemsCamera; // A câmera que renderiza os itens ocultos
    private Transform maskTransform;

    void Start()
    {
        if (hiddenItemsCamera == null)
            hiddenItemsCamera = Camera.main;

        maskTransform = transform;
    }

    void Update()
    {
        // Pega a posição do mouse no mundo 2D (sem depender de Z)
        Vector3 mouseWorldPos = hiddenItemsCamera.ScreenToWorldPoint(Input.mousePosition);

        // Força Z = 0 para 2D (ou o mesmo Z da máscara)
        mouseWorldPos.z = maskTransform.position.z;

        // Centraliza a máscara no ponteiro
        maskTransform.position = mouseWorldPos;
    }
}