using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoomSpriteRule
{
    [Tooltip("Nome exato do GameObject da sala (ex: 'Arquivo', 'Escritorio')")]
    public string roomName;

    [Tooltip("Sprites a ativar quando essa sala estiver ligada")]
    public List<GameObject> spritesToEnable = new List<GameObject>();

    [Tooltip("Sprites a desativar quando essa sala estiver ligada")]
    public List<GameObject> spritesToDisable = new List<GameObject>();
}

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("Salas do jogo")]
    public GameObject[] rooms; // Coloque todas as salas aqui no inspetor
    public string startingRoom = "Arquivo";

    [Header("Regras de sprites por sala")]
    public List<RoomSpriteRule> spriteRules = new List<RoomSpriteRule>();

    private GameObject currentRoom;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Desliga todas as salas no início
        foreach (GameObject r in rooms)
            r.SetActive(false);

        // Liga a sala inicial
        ChangeRoom(startingRoom);
    }

    public void ChangeRoom(string roomName)
    {
        bool found = false;

        foreach (GameObject r in rooms)
        {
            if (r.name == roomName)
            {
                if (currentRoom != null)
                    currentRoom.SetActive(false);

                r.SetActive(true);
                currentRoom = r;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning("Room " + roomName + " não encontrada!");
            return;
        }

        // Atualiza sprites conforme as regras
        ApplySpriteRules(roomName);
    }

    private void ApplySpriteRules(string roomName)
    {
        foreach (var rule in spriteRules)
        {
            if (rule.roomName == roomName)
            {
                foreach (var go in rule.spritesToEnable)
                    if (go != null) go.SetActive(true);

                foreach (var go in rule.spritesToDisable)
                    if (go != null) go.SetActive(false);

                return;
            }
        }

        // Se nenhuma regra for encontrada, não faz nada
        Debug.Log($"[MapManager] Nenhuma regra de sprite para '{roomName}'.");
    }

    public string GetCurrentRoom()
    {
        return currentRoom != null ? currentRoom.name : "";
    }
}
