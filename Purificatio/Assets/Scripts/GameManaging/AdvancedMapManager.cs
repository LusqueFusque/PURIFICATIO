using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Sistema avançado de gerenciamento de mapa com suporte a:
/// - Ativação/desativação de objetos por cômodo
/// - Condições baseadas em missões
/// - Condições baseadas em variáveis globais
/// - Sistema de exceções altamente configurável
/// </summary>
public class AdvancedMapManager : MonoBehaviour
{
    public static AdvancedMapManager Instance;

    [Header("=== CONFIGURAÇÃO DE CÔMODOS ===")]
    [Tooltip("Lista de todos os cômodos do jogo")]
    public List<RoomData> rooms = new List<RoomData>();

    [Header("=== CONFIGURAÇÃO INICIAL ===")]
    [Tooltip("Nome do cômodo que começa ativo")]
    public string startingRoomName = "Arquivo";

    [Header("=== CONFIGURAÇÃO DE OBJETOS GLOBAIS ===")]
    [Tooltip("Objetos que aparecem/desaparecem em múltiplos cômodos")]
    public List<ConditionalObject> conditionalObjects = new List<ConditionalObject>();

    [Header("=== DEBUG ===")]
    [SerializeField] private string currentRoomName;
    public bool showDebugLogs = true;

    // Dicionário para acesso rápido aos cômodos
    private Dictionary<string, RoomData> roomDict = new Dictionary<string, RoomData>();
    private RoomData currentRoom;

    // Sistema de variáveis globais para condições customizadas
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
        // Desativa todos os cômodos
        foreach (var room in rooms)
        {
            if (room.roomObject != null)
            {
                room.roomObject.SetActive(false);
            }
        }

        // Ativa o cômodo inicial
        ChangeRoom(startingRoomName);
    }

    /// <summary>
    /// Inicializa o dicionário de cômodos para acesso rápido
    /// </summary>
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
                    Debug.LogWarning($"[AdvancedMapManager] Cômodo duplicado encontrado: {room.roomName}");
                }
            }
        }
    }

    /// <summary>
    /// Muda para um novo cômodo
    /// </summary>
    public void ChangeRoom(string roomName)
    {
        if (!roomDict.ContainsKey(roomName))
        {
            Debug.LogError($"[AdvancedMapManager] Cômodo '{roomName}' não encontrado!");
            return;
        }

        // Desativa cômodo anterior
        if (currentRoom != null && currentRoom.roomObject != null)
        {
            currentRoom.roomObject.SetActive(false);
            if (showDebugLogs)
                Debug.Log($"[AdvancedMapManager] Desativando cômodo: {currentRoom.roomName}");
        }

        // Ativa novo cômodo
        currentRoom = roomDict[roomName];
        currentRoomName = roomName;

        if (currentRoom.roomObject != null)
        {
            currentRoom.roomObject.SetActive(true);
            if (showDebugLogs)
                Debug.Log($"[AdvancedMapManager] Ativando cômodo: {roomName}");
        }

        // Aplica regras do cômodo
        ApplyRoomRules(currentRoom);

        // Atualiza objetos condicionais globais
        UpdateConditionalObjects();
    }

    /// <summary>
    /// Aplica as regras de ativação/desativação de objetos do cômodo
    /// </summary>
    private void ApplyRoomRules(RoomData room)
    {
        // 1. Objetos que sempre aparecem neste cômodo
        foreach (var obj in room.alwaysActiveObjects)
        {
            if (obj != null && ShouldActivateObject(obj, room.alwaysActiveObjects))
            {
                obj.SetActive(true);
                if (showDebugLogs)
                    Debug.Log($"[AdvancedMapManager] ✓ Ativando objeto: {obj.name}");
            }
        }

        // 2. Objetos que sempre somem neste cômodo
        foreach (var obj in room.alwaysInactiveObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                if (showDebugLogs)
                    Debug.Log($"[AdvancedMapManager] ✗ Desativando objeto: {obj.name}");
            }
        }

        // 3. Objetos condicionais do cômodo
        foreach (var conditional in room.conditionalObjects)
        {
            UpdateConditionalObjectState(conditional);
        }
    }

    /// <summary>
    /// Verifica se um objeto deve ser ativado (considerando exceções)
    /// </summary>
    private bool ShouldActivateObject(GameObject obj, List<GameObject> objectList)
    {
        // Verifica se há exceções para este objeto no cômodo atual
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

        return true; // Sem exceções, ativa normalmente
    }

    /// <summary>
    /// Atualiza o estado de um objeto condicional
    /// </summary>
    private void UpdateConditionalObjectState(ConditionalObject conditional)
    {
        if (conditional.targetObject == null) return;

        bool conditionMet = EvaluateCondition(conditional.condition);

        conditional.targetObject.SetActive(conditionMet);

        if (showDebugLogs)
        {
            string status = conditionMet ? "✓ Ativado" : "✗ Desativado";
            Debug.Log($"[AdvancedMapManager] {status}: {conditional.targetObject.name} (Condição: {conditional.condition.conditionType})");
        }
    }

    /// <summary>
    /// Atualiza todos os objetos condicionais globais
    /// </summary>
    private void UpdateConditionalObjects()
    {
        foreach (var conditional in conditionalObjects)
        {
            // Verifica se o objeto deve aparecer no cômodo atual
            bool shouldShowInRoom = conditional.showInRooms.Count == 0 ||
                                   conditional.showInRooms.Contains(currentRoomName);

            bool shouldHideInRoom = conditional.hideInRooms.Contains(currentRoomName);

            if (shouldHideInRoom || !shouldShowInRoom)
            {
                if (conditional.targetObject != null)
                    conditional.targetObject.SetActive(false);
                continue;
            }

            // Avalia a condição
            UpdateConditionalObjectState(conditional);
        }
    }

    /// <summary>
    /// Avalia uma condição e retorna true/false
    /// </summary>
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

    /// <summary>
    /// Avalia múltiplas condições com lógica AND/OR
    /// </summary>
    private bool EvaluateMultipleConditions(ActivationCondition condition)
    {
        if (condition.multipleConditions == null || condition.multipleConditions.Count == 0)
            return false;

        if (condition.logicOperator == LogicOperator.AND)
        {
            // Todas devem ser verdadeiras
            foreach (var subCondition in condition.multipleConditions)
            {
                if (!EvaluateCondition(subCondition))
                    return false;
            }
            return true;
        }
        else // OR
        {
            // Pelo menos uma deve ser verdadeira
            foreach (var subCondition in condition.multipleConditions)
            {
                if (EvaluateCondition(subCondition))
                    return true;
            }
            return false;
        }
    }

    // ==================== API PÚBLICA ====================

    /// <summary>
    /// Define uma flag global (útil para condições customizadas)
    /// </summary>
    public void SetGlobalFlag(string flagName, bool value)
    {
        globalFlags[flagName] = value;
        if (showDebugLogs)
            Debug.Log($"[AdvancedMapManager] Flag '{flagName}' = {value}");

        // Atualiza objetos condicionais após mudança de flag
        if (currentRoom != null)
        {
            ApplyRoomRules(currentRoom);
            UpdateConditionalObjects();
        }
    }

    /// <summary>
    /// Obtém o valor de uma flag global
    /// </summary>
    public bool GetGlobalFlag(string flagName)
    {
        return globalFlags.ContainsKey(flagName) && globalFlags[flagName];
    }

    /// <summary>
    /// Força atualização de todos os objetos condicionais
    /// </summary>
    public void RefreshAllConditionals()
    {
        if (currentRoom != null)
        {
            ApplyRoomRules(currentRoom);
            UpdateConditionalObjects();
        }
    }

    /// <summary>
    /// Retorna o nome do cômodo atual
    /// </summary>
    public string GetCurrentRoomName()
    {
        return currentRoomName;
    }

    /// <summary>
    /// Verifica se um cômodo existe
    /// </summary>
    public bool RoomExists(string roomName)
    {
        return roomDict.ContainsKey(roomName);
    }

    // ==================== DEBUG ====================

    [ContextMenu("Refresh Current Room")]
    public void DebugRefreshRoom()
    {
        if (currentRoom != null)
        {
            Debug.Log($"[DEBUG] Atualizando cômodo: {currentRoomName}");
            ApplyRoomRules(currentRoom);
            UpdateConditionalObjects();
        }
    }

    [ContextMenu("List All Rooms")]
    public void DebugListRooms()
    {
        Debug.Log("=== LISTA DE CÔMODOS ===");
        foreach (var room in rooms)
        {
            Debug.Log($"- {room.roomName} (Ativo: {room.roomObject?.activeSelf})");
        }
    }

    [ContextMenu("List Global Flags")]
    public void DebugListFlags()
    {
        Debug.Log("=== FLAGS GLOBAIS ===");
        foreach (var flag in globalFlags)
        {
            Debug.Log($"- {flag.Key} = {flag.Value}");
        }
    }
}

// ==================== COMPONENTE AUXILIAR PARA BOTÕES ====================

/// <summary>
/// Adicione este componente aos botões de mudança de cômodo
/// </summary>
public class RoomButton : MonoBehaviour
{
    [Header("=== CONFIGURAÇÃO ===")]
    [Tooltip("Nome do cômodo para onde este botão leva")]
    public string targetRoomName;

    [Header("=== CONDIÇÕES DE VISIBILIDADE (OPCIONAL) ===")]
    [Tooltip("Se marcado, o botão só aparece se a condição for atendida")]
    public bool useCondition = false;
    public ActivationCondition visibilityCondition;

    private UnityEngine.UI.Button button;
    private GameObject buttonObject;

    void Start()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        buttonObject = gameObject;

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError($"[RoomButton] Componente Button não encontrado em {gameObject.name}!");
        }

        UpdateVisibility();
    }

    void OnButtonClick()
    {
        if (AdvancedMapManager.Instance != null)
        {
            AdvancedMapManager.Instance.ChangeRoom(targetRoomName);
        }
        else
        {
            Debug.LogError("[RoomButton] AdvancedMapManager não encontrado!");
        }
    }

    void Update()
    {
        if (useCondition)
        {
            UpdateVisibility();
        }
    }

    private void UpdateVisibility()
    {
        if (!useCondition)
        {
            buttonObject.SetActive(true);
            return;
        }

        // Usa o método público da AdvancedMapManager para avaliar
        if (AdvancedMapManager.Instance != null)
        {
            // Esta é uma simplificação - você pode expandir se necessário
            bool shouldShow = EvaluateConditionSimple();
            buttonObject.SetActive(shouldShow);
        }
    }

    private bool EvaluateConditionSimple()
    {
        if (visibilityCondition.conditionType == ConditionType.MissionCompleted)
        {
            if (MissionManager.Instance != null)
                return MissionManager.Instance.IsCompleted(visibilityCondition.missionId);
        }
        else if (visibilityCondition.conditionType == ConditionType.GlobalFlag)
        {
            if (AdvancedMapManager.Instance != null)
                return AdvancedMapManager.Instance.GetGlobalFlag(visibilityCondition.flagName);
        }

        return true;
    }
}

// ==================== CLASSES DE DADOS ====================

/// <summary>
/// Dados de um cômodo
/// </summary>
[System.Serializable]
public class RoomData
{
    [Header("=== IDENTIFICAÇÃO ===")]
    [Tooltip("Nome único do cômodo")]
    public string roomName;

    [Tooltip("GameObject do cômodo (geralmente uma Image no Canvas)")]
    public GameObject roomObject;

    [Header("=== OBJETOS SEMPRE ATIVOS ===")]
    [Tooltip("Objetos que sempre aparecem neste cômodo")]
    public List<GameObject> alwaysActiveObjects = new List<GameObject>();

    [Header("=== OBJETOS SEMPRE INATIVOS ===")]
    [Tooltip("Objetos que sempre somem neste cômodo")]
    public List<GameObject> alwaysInactiveObjects = new List<GameObject>();

    [Header("=== OBJETOS CONDICIONAIS ===")]
    [Tooltip("Objetos que aparecem/somem baseado em condições")]
    public List<ConditionalObject> conditionalObjects = new List<ConditionalObject>();

    [Header("=== EXCEÇÕES ===")]
    [Tooltip("Regras especiais que sobrescrevem comportamento padrão")]
    public List<ObjectException> objectExceptions = new List<ObjectException>();
}

/// <summary>
/// Objeto que aparece/some baseado em condições
/// </summary>
[System.Serializable]
public class ConditionalObject
{
    [Tooltip("Objeto alvo")]
    public GameObject targetObject;

    [Tooltip("Condição para ativar o objeto")]
    public ActivationCondition condition;

    [Header("=== RESTRIÇÕES DE CÔMODOS (OPCIONAL) ===")]
    [Tooltip("Se preenchido, só aparece nestes cômodos. Vazio = todos")]
    public List<string> showInRooms = new List<string>();

    [Tooltip("Nunca aparece nestes cômodos (sobrescreve showInRooms)")]
    public List<string> hideInRooms = new List<string>();
}

/// <summary>
/// Exceção para sobrescrever comportamento de um objeto
/// </summary>
[System.Serializable]
public class ObjectException
{
    [Tooltip("Objeto que terá exceção")]
    public GameObject targetObject;

    [Tooltip("Tipo de exceção")]
    public ExceptionType exceptionType;

    [Tooltip("Condição para aplicar a exceção")]
    public ActivationCondition condition;
}

/// <summary>
/// Condição para ativar/desativar objetos
/// </summary>
[System.Serializable]
public class ActivationCondition
{
    [Tooltip("Tipo de condição")]
    public ConditionType conditionType;

    [Header("=== CONFIGURAÇÃO (depende do tipo) ===")]
    [Tooltip("ID da missão (para tipos relacionados a missões)")]
    public string missionId;

    [Tooltip("Nome da flag global (para tipo GlobalFlag)")]
    public string flagName;

    [Header("=== MÚLTIPLAS CONDIÇÕES ===")]
    [Tooltip("Operador lógico para combinar condições")]
    public LogicOperator logicOperator = LogicOperator.AND;

    [Tooltip("Lista de subcondições (para tipo MultipleConditions)")]
    public List<ActivationCondition> multipleConditions = new List<ActivationCondition>();
}

// ==================== ENUMS ====================

public enum ConditionType
{
    Always,                 // Sempre ativo
    Never,                  // Nunca ativo
    MissionCompleted,       // Ativo se missão completa
    MissionActive,          // Ativo se missão ativa
    MissionNotStarted,      // Ativo se missão não começou
    GlobalFlag,             // Ativo se flag global = true
    MultipleConditions      // Combina múltiplas condições
}

public enum ExceptionType
{
    OnlyShowIf,     // Só mostra se condição for verdadeira
    HideIf          // Esconde se condição for verdadeira
}

public enum LogicOperator
{
    AND,    // Todas as condições devem ser verdadeiras
    OR      // Pelo menos uma condição deve ser verdadeira
}
