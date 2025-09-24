using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamController : MonoBehaviour
{
    public Camera mainCamera;
    public float followSpeed = 10f;
    public Button cameraButton;
    public GameObject camObject; // objeto que tem o sprite mask, etc.

    private bool isFollowing = false;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (cameraButton != null)
        {
            cameraButton.onClick.AddListener(EnterCameraMode);
        }

        if (camObject != null)
        {
            camObject.SetActive(false); // começa desativado
        }
    }

    void Update()
    {
        if (isFollowing)
        {
            FollowMouse();

            if (Input.GetMouseButtonDown(1))
            {
                ExitCameraMode();
            }
        }
    }

    void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        targetPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    public void EnterCameraMode()
    {
        isFollowing = true;

        if (camObject != null)
        {
            camObject.SetActive(true);
        }

        Debug.Log("Modo câmera ativado");
    }

    public void ExitCameraMode()
    {
        isFollowing = false;

        if (camObject != null)
        {
            camObject.SetActive(false);
        }

        Debug.Log("Modo câmera desativado");
    }
}
