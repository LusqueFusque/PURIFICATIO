using UnityEngine;

public class CamItemTut : MonoBehaviour
{
    public GameObject photoMask;
    private bool isActive = false;

    // Evento para notificar quando a câmera é fechada
    public static System.Action OnCameraClosed;

    public bool WasOpened { get; private set; }
    public bool WasClosed { get; private set; }

    private void Start()
    {
        if (photoMask == null)
        {
            Debug.LogError("[CamItemTut] photoMask não foi atribuída no Inspector!");
            enabled = false;
        }
    }

    public void OnCameraButtonClicked()
    {
        ActivateCamera();
    }

    private void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(1)) // botão direito desativa
        {
            CloseCamera();
        }
    }

    private void ActivateCamera()
    {
        isActive = true;
        photoMask.SetActive(true);
        WasOpened = true;
        WasClosed = false;
        Debug.Log("[CamItemTut] 📷 Câmera ativada.");

        // Se a missão useCamera estiver ativa, marca que foi aberta
        if (MissionManager.Instance != null && MissionManager.Instance.IsActive("useCamera"))
        {
            Debug.Log("[CamItemTut] ✅ Câmera aberta durante missão 'useCamera'.");
        }
    }

    private void CloseCamera()
    {
        isActive = false;
        photoMask.SetActive(false);
        WasClosed = true;
        Debug.Log("[CamItemTut] 📷 Câmera desativada.");

        // Completa missão básica de usar câmera
        if (MissionManager.Instance != null && MissionManager.Instance.IsActive("useCamera"))
        {
            MissionManager.Instance.CompleteMission("useCamera");
            Debug.Log("[CamItemTut] 🎉 Missão 'useCamera' COMPLETA!");
        }

        // Dispara evento para SaltMissionChecker escutar
        OnCameraClosed?.Invoke();
    }

    // Método público para verificar se está ativa
    public bool IsActive() => isActive;
}