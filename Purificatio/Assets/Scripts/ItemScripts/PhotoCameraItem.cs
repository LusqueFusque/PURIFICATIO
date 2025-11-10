using UnityEngine;

public class PhotoCameraItem : MonoBehaviour
{
    public GameObject photoMask;
    private bool isActive = false;
    private bool wasActivatedDuringMission = false; // Tracking se foi ativada durante findGhost

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
        
        // ===== DEBUG: Verifica MissionManager =====
        if (MissionManager.Instance == null)
        {
            Debug.LogError("[PhotoCameraItem] ❌ MissionManager.Instance é NULL!");
        }
        else
        {
            Debug.Log($"[PhotoCameraItem] MissionManager encontrado. Verificando missão 'findGhost'...");
            
            bool isFindGhostActive = MissionManager.Instance.IsActive("findGhost");
            Debug.Log($"[PhotoCameraItem] Missão 'findGhost' está ativa? {isFindGhostActive}");
            
            if (isFindGhostActive)
            {
                wasActivatedDuringMission = true;
                Debug.Log("[PhotoCameraItem] ✅ Marcado: Câmera ativada durante missão findGhost.");
            }
            else
            {
                Debug.Log("[PhotoCameraItem] ⚠️ Missão 'findGhost' NÃO está ativa no momento.");
            }
        }
    }

    private void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(1)) // botão direito desativa
        {
            CloseCamera();
        }
    }
    
    private void CloseCamera()
    {
        isActive = false;
        
        if (photoMask != null)
            photoMask.SetActive(false);

        Debug.Log("[PhotoCameraItem] Câmera desativada.");
        Debug.Log("========================================");
        Debug.Log($"[PhotoCameraItem] wasActivatedDuringMission = {wasActivatedDuringMission}");

        // ==================== COMPLETAR MISSÕES ====================
        if (MissionManager.Instance != null)
        {
            Debug.Log("[PhotoCameraItem] MissionManager encontrado. Verificando missões...");
            
            // Completa missão findGhost APENAS SE foi ativada durante a missão
            bool isFindGhostActive = MissionManager.Instance.IsActive("findGhost");
            Debug.Log($"[PhotoCameraItem] Missão 'findGhost' está ativa agora? {isFindGhostActive}");
            Debug.Log($"[PhotoCameraItem] Foi ativada durante missão? {wasActivatedDuringMission}");
            
            if (isFindGhostActive && wasActivatedDuringMission)
            {
                Debug.Log("[PhotoCameraItem] 🎉 Completando missão 'findGhost'!");
                MissionManager.Instance.CompleteMission("findGhost");
                Debug.Log("[PhotoCameraItem] ✓ Missão 'findGhost' COMPLETA! (Câmera ligada → desligada)");
                wasActivatedDuringMission = false; // Reset
            }
            else
            {
                if (!isFindGhostActive)
                    Debug.Log("[PhotoCameraItem] ⚠️ Missão 'findGhost' não está ativa.");
                if (!wasActivatedDuringMission)
                    Debug.Log("[PhotoCameraItem] ⚠️ Câmera não foi ativada durante a missão.");
            }
            
            // Completa missão básica de usar câmera do tutorial (independente)
            bool isUseCameraActive = MissionManager.Instance.IsActive("useCamera");
            Debug.Log($"[PhotoCameraItem] Missão 'useCamera' está ativa? {isUseCameraActive}");
            
            if (isUseCameraActive)
            {
                MissionManager.Instance.CompleteMission("useCamera");
                Debug.Log("[PhotoCameraItem] Missão 'useCamera' completa.");
            }
        }
        else
        {
            Debug.LogError("[PhotoCameraItem] ❌ MissionManager.Instance é NULL!");
        }
        
        Debug.Log("========================================");

        // Dispara evento para SaltMissionChecker escutar
        OnCameraClosed?.Invoke();
    }

    // Método público para verificar se está ativa
    public bool IsActive() => isActive;
}