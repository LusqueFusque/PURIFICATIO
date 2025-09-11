using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("Salas do jogo")]
    public GameObject[] rooms; // Coloque "Arquivo" e "Escritorio" aqui no inspetor
    public string startingRoom = "Arquivo";

    private GameObject currentRoom;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Desliga tudo no começo
        foreach (GameObject r in rooms) r.SetActive(false);

        // Liga a sala inicial
        ChangeRoom(startingRoom);
    }

    public void ChangeRoom(string roomName)
    {
        foreach (GameObject r in rooms)
        {
            if (r.name == roomName)
            {
                if (currentRoom != null) currentRoom.SetActive(false);
                r.SetActive(true);
                currentRoom = r;
                return;
            }
        }
        Debug.LogWarning("Room " + roomName + " não encontrada!");
    }

    public string GetCurrentRoom()
    {
        return currentRoom != null ? currentRoom.name : "";
    }
}
