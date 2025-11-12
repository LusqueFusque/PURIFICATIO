using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// ==================== CLASSES DE DADOS (NO TOPO PARA EVITAR ERROS) ====================

/// <summary>
/// Regra de sprites (versão do AdvancedMapManager - não conflita com MapManager)
/// </summary>
[System.Serializable]
public class AdvancedRoomSpriteRule
{
    [Tooltip("Nome exato do GameObject da sala")]
    public string roomName;

    [Tooltip("Sprites a ativar quando essa sala estiver ligada")]
    public List<GameObject> spritesToEnable = new List<GameObject>();

    [Tooltip("Sprites a desativar quando essa sala estiver ligada")]
    public List<GameObject> spritesToDisable = new List<GameObject>();
}

/// <summary>
/// Dados de um cômodo (SISTEMA AVANÇADO)
/// </summary>
[System.Serializable]
public class RoomData
{
    [Header("=== IDENTIFICAÇÃO ===")]
    public string roomName;
    public GameObject roomObject;

    [Header("=== OBJETOS SEMPRE ATIVOS ===")]
    public List<GameObject> alwaysActiveObjects = new List<GameObject>();

    [Header("=== OBJETOS SEMPRE INATIVOS ===")]
    public List<GameObject> alwaysInactiveObjects = new List<GameObject>();

    [Header("=== OBJETOS CONDICIONAIS ===")]
    public List<ConditionalObject> conditionalObjects = new List<ConditionalObject>();

    [Header("=== EXCEÇÕES ===")]
    public List<ObjectException> objectExceptions = new List<ObjectException>();
}

/// <summary>
/// Objeto que aparece/some baseado em condições
/// </summary>
[System.Serializable]
public class ConditionalObject
{
    public GameObject targetObject;
    public ActivationCondition condition;

    [Header("=== RESTRIÇÕES DE CÔMODOS ===")]
    public List<string> showInRooms = new List<string>();
    public List<string> hideInRooms = new List<string>();
}

/// <summary>
/// Exceção para sobrescrever comportamento de um objeto
/// </summary>
[System.Serializable]
public class ObjectException
{
    public GameObject targetObject;
    public ExceptionType exceptionType;
    public ActivationCondition condition;
}

/// <summary>
/// Condição para ativar/desativar objetos
/// </summary>
[System.Serializable]
public class ActivationCondition
{
    public ConditionType conditionType;

    [Header("=== CONFIGURAÇÃO ===")]
    public string missionId;
    public string flagName;

    [Header("=== MÚLTIPLAS CONDIÇÕES ===")]
    public LogicOperator logicOperator = LogicOperator.AND;
    
    [SerializeReference] // ← FIX: Usa SerializeReference para evitar loop infinito
    public List<ActivationCondition> multipleConditions = new List<ActivationCondition>();
}

// ==================== ENUMS ====================

public enum ConditionType
{
    Always,
    Never,
    MissionCompleted,
    MissionActive,
    MissionNotStarted,
    GlobalFlag,
    MultipleConditions
}

public enum ExceptionType
{
    OnlyShowIf,
    HideIf
}

public enum LogicOperator
{
    AND,
    OR
}

// ==================== MANAGER PRINCIPAL ====================

/// <summary>
/// Sistema avançado de gerenciamento de mapa
/// </summary>
public class AdvancedMapManager : MonoBehaviour
{
    public static AdvancedMapManager Instance;

    [Header("=== MINIMAPA ===")]
    public GameObject[] minimap_rooms;
    public List<AdvancedRoomSpriteRule> minimap_spriteRules = new List<AdvancedRoomSpriteRule>();

    [Header("=== SISTEMA AVANÇADO ===")]
    public List<RoomData> rooms = new List<RoomData>();

    [Header("=== CONFIGURAÇÃO ===")]
    public string startingRoomName = "Arquivo";

    [Header("=== OBJETOS CONDICIONAIS GLOBAIS ===")]
    public List<ConditionalObject> conditionalObjects = new List<ConditionalObject>();

    [Header("=== DEBUG ===")]
    [SerializeField] private string currentRoomName;
    public bool showDebugLogs = true;

    private GameObject minimap_currentRoom;
    private Dictionary<string, RoomData> roomDict = new Dictionary<string, RoomData>();
    private RoomData currentRoom;
    private Dictionary<string, bool> globalFlags = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeRoomDictionary();
    }

    void Start()
    {
        if (minimap_rooms != null && minimap_rooms.Length > 0)
        {
            foreach (GameObject r in minimap_rooms)
            {
                if (r != null)
                    r.SetActive(false);
            }
        }

        foreach (var room in rooms)
        {
            if (room.roomObject != null)
            {
                room.roomObject.SetActive(false);
            }
        }

        ChangeRoom(startingRoomName);
    }

    private void InitializeRoomDictionary()
    {
        roomDict.Clear();
        foreach (var room in rooms)
        {
            if (!string.IsNullOrEmpty(room.roomName))
            {
                if (!roomDict.ContainsKey(room.roomName))
                {
                    roomDict[room.roomName] = room;
                }
                else
                {
                    Debug.LogWarning($"[AdvancedMapManager] Cômodo duplicado: {room.roomName}");
                }
            }
        }
    }

    public void ChangeRoom(string roomName)
    {
        if (showDebugLogs)
            Debug.Log($"[AdvancedMapManager] ChangeRoom: {roomName}");

        MinimapChangeRoom(roomName);

        if (roomDict.ContainsKey(roomName))
        {
            if (currentRoom != null && currentRoom.roomObject != null)
            {
                currentRoom.roomObject.SetActive(false);
            }

            currentRoom = roomDict[roomName];
            currentRoomName = roomName;

            if (currentRoom.roomObject != null)
            {
                currentRoom.roomObject.SetActive(true);
            }

            ApplyRoomRules(currentRoom);
            UpdateConditionalObjects();
        }
        else
        {
            currentRoomName = roomName;
        }
    }

    private void MinimapChangeRoom(string roomName)
    {
        if (minimap_rooms == null || minimap_rooms.Length == 0)
            return;

        bool found = false;

        foreach (GameObject r in minimap_rooms)
        {
            if (r != null && r.name == roomName)
            {
                if (minimap_currentRoom != null)
                {
                    minimap_currentRoom.SetActive(false);
                }

                r.SetActive(true);
                minimap_currentRoom = r;
                found = true;
                break;
            }
        }

        if (!found && showDebugLogs)
        {
            Debug.LogWarning($"[AdvancedMapManager] Room '{roomName}' não encontrada!");
        }

        MinimapApplySpriteRules(roomName);
    }

    private void MinimapApplySpriteRules(string roomName)
    {
        if (minimap_spriteRules == null || minimap_spriteRules.Count == 0)
            return;

        foreach (var rule in minimap_spriteRules)
        {
            if (rule.roomName == roomName)
            {
                foreach (var go in rule.spritesToEnable)
                {
                    if (go != null)
                        go.SetActive(true);
                }

                foreach (var go in rule.spritesToDisable)
                {
                    if (go != null)
                        go.SetActive(false);
                }

                return;
            }
        }
    }

    private void ApplyRoomRules(RoomData room)
    {
        foreach (var obj in room.alwaysActiveObjects)
        {
            if (obj != null && ShouldActivateObject(obj, room.alwaysActiveObjects))
            {
                obj.SetActive(true);
            }
        }

        foreach (var obj in room.alwaysInactiveObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        foreach (var conditional in room.conditionalObjects)
        {
            UpdateConditionalObjectState(conditional);
        }
    }

    private bool ShouldActivateObject(GameObject obj, List<GameObject> objectList)
    {
        if (currentRoom != null)
        {
            foreach (var exception in currentRoom.objectExceptions)
            {
                if (exception.targetObject == obj)
                {
                    bool conditionMet = EvaluateCondition(exception.condition);

                    if (exception.exceptionType == ExceptionType.OnlyShowIf)
                    {
                        return conditionMet;
                    }
                    else if (exception.exceptionType == ExceptionType.HideIf)
                    {
                        return !conditionMet;
                    }
                }
            }
        }

        return true;
    }

    private void UpdateConditionalObjectState(ConditionalObject conditional)
    {
        if (conditional.targetObject == null) return;

        bool conditionMet = EvaluateCondition(conditional.condition);
        conditional.targetObject.SetActive(conditionMet);
    }

    private void UpdateConditionalObjects()
    {
        foreach (var conditional in conditionalObjects)
        {
            bool shouldShowInRoom = conditional.showInRooms.Count == 0 ||
                                   conditional.showInRooms.Contains(currentRoomName);

            bool shouldHideInRoom = conditional.hideInRooms.Contains(currentRoomName);

            if (shouldHideInRoom || !shouldShowInRoom)
            {
                if (conditional.targetObject != null)
                    conditional.targetObject.SetActive(false);
                continue;
            }

            UpdateConditionalObjectState(conditional);
        }
    }

    private bool EvaluateCondition(ActivationCondition condition)
    {
        switch (condition.conditionType)
        {
            case ConditionType.Always:
                return true;

            case ConditionType.Never:
                return false;

            case ConditionType.MissionCompleted:
                if (MissionManager.Instance != null && !string.IsNullOrEmpty(condition.missionId))
                {
                    return MissionManager.Instance.IsCompleted(condition.missionId);
                }
                return false;

            case ConditionType.MissionActive:
                if (MissionManager.Instance != null && !string.IsNullOrEmpty(condition.missionId))
                {
                    return MissionManager.Instance.IsActive(condition.missionId);
                }
                return false;

            case ConditionType.MissionNotStarted:
                if (MissionManager.Instance != null && !string.IsNullOrEmpty(condition.missionId))
                {
                    return !MissionManager.Instance.IsActive(condition.missionId) &&
                           !MissionManager.Instance.IsCompleted(condition.missionId);
                }
                return true;

            case ConditionType.GlobalFlag:
                if (!string.IsNullOrEmpty(condition.flagName))
                {
                    return globalFlags.ContainsKey(condition.flagName) && globalFlags[condition.flagName];
                }
                return false;

            case ConditionType.MultipleConditions:
                return EvaluateMultipleConditions(condition);

            default:
                return false;
        }
    }

    private bool EvaluateMultipleConditions(ActivationCondition condition)
    {
        if (condition.multipleConditions == null || condition.multipleConditions.Count == 0)
            return false;

        if (condition.logicOperator == LogicOperator.AND)
        {
            foreach (var subCondition in condition.multipleConditions)
            {
                if (!EvaluateCondition(subCondition))
                    return false;
            }
            return true;
        }
        else
        {
            foreach (var subCondition in condition.multipleConditions)
            {
                if (EvaluateCondition(subCondition))
                    return true;
            }
            return false;
        }
    }

    // ==================== API PÚBLICA ====================

    public void SetGlobalFlag(string flagName, bool value)
    {
        globalFlags[flagName] = value;

        if (currentRoom != null)
        {
            ApplyRoomRules(currentRoom);
            UpdateConditionalObjects();
        }
    }

    public bool GetGlobalFlag(string flagName)
    {
        return globalFlags.ContainsKey(flagName) && globalFlags[flagName];
    }

    public void RefreshAllConditionals()
    {
        if (currentRoom != null)
        {
            ApplyRoomRules(currentRoom);
            UpdateConditionalObjects();
        }
    }

    public string GetCurrentRoomName()
    {
        return currentRoomName;
    }

    public string GetCurrentRoom()
    {
        return minimap_currentRoom != null ? minimap_currentRoom.name : currentRoomName;
    }

    public bool RoomExists(string roomName)
    {
        return roomDict.ContainsKey(roomName);
    }
}