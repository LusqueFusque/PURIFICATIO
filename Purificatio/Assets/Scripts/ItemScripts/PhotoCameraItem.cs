using UnityEngine;
public class PhotoCameraItem : MonoBehaviour
{ 
    public GameObject photoMask;
    private bool isActive = false;
    public void OnCameraButtonClicked()
    {
        isActive = true;
        photoMask.SetActive(true);
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
        }
    }
}