using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [System.Serializable]
    public class RoomPanel
    {
        public string roomName;              // Nome para identificar (ex: "escritorio", "armazem")
        public GameObject panel;             // Referência ao GameObject do painel
    }

    public List<RoomPanel> rooms;           // Lista configurada pelo inspetor
    private string currentRoom;             // Nome da sala atual

    void Start()
    {
        if (rooms != null && rooms.Count > 0)
        {
            // Começa na primeira sala por padrão
            GoToRoom(rooms[0].roomName);
        }
    }

    public void GoToRoom(string roomName)
    {
        foreach (var room in rooms)
        {
            bool isActive = room.roomName == roomName;
            room.panel.SetActive(isActive);
            if (isActive) currentRoom = roomName;
        }
    }

    public string GetCurrentRoom()
    {
        return currentRoom;
    }
}
