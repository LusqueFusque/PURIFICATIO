using UnityEngine;

public class CamItemTut : MonoBehaviour
{
    public GameObject photoMask;
    private bool isActive = false;

    // Evento para notificar quando a câmera é fechada
    public static System.Action OnCameraClosed;

    private void Start()
    {
        if (photoMask == null)
        {
            Debug.LogError("[PhotoCameraItem] photoMask não foi atribuída no Inspector!");
            enabled = false;
        }
    }

    public void OnCameraButtonClicked()
    {
        if (photoMask == null)
        {
            Debug.LogError("[PhotoCameraItem] Não é possível ativar câmera sem photoMask!");
            return;
        }

        isActive = true;
        photoMask.SetActive(true);
        Debug.Log("[PhotoCameraItem] Câmera ativada.");
    }

    private void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(1)) // botão direito desativa
        {
            isActive = false;
            photoMask.SetActive(false);

            if (MissionManager.Instance != null)
                MissionManager.Instance.CompleteMission("useCamera");

            CloseCamera();
        }
    }

    private void CloseCamera()
    {
        isActive = false;

        if (photoMask != null)
            photoMask.SetActive(false);

        Debug.Log("[PhotoCameraItem] Câmera desativada.");

        // Completa missão básica de usar câmera
        if (MissionManager.Instance != null)
            MissionManager.Instance.CompleteMission("useCamera");

        // Dispara evento para SaltMissionChecker escutar
        OnCameraClosed?.Invoke();
    }

    // Método público para verificar se está ativa
    public bool IsActive() => isActive;
}