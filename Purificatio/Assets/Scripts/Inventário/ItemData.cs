using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Dados do Item")]
    public string itemName;
    public Sprite icon;

    [Header("Classe lógica do item (preenchida automaticamente)")]
    public string logicClassName;

#if UNITY_EDITOR
    [Header("Script de lógica (somente no editor)")]
    public UnityEditor.MonoScript itemLogicScript;

    private void OnValidate()
    {
        if (itemLogicScript != null)
        {
            var type = itemLogicScript.GetClass();
            if (type != null)
                logicClassName = type.FullName;
        }
    }
#endif


    // 🔥 O DynamicInventory precisa disso — por isso estava dando erro.
    public virtual void Use()
    {
        if (string.IsNullOrEmpty(logicClassName))
        {
            Debug.LogWarning($"[ItemData] O item '{itemName}' não tem lógica definida.");
            return;
        }

        // Carrega o tipo da classe
        System.Type t = System.Type.GetType(logicClassName);

        if (t == null)
        {
            Debug.LogError($"[ItemData] Classe '{logicClassName}' não encontrada para o item '{itemName}'.");
            return;
        }

        // Cria a instância
        object instance = System.Activator.CreateInstance(t);

        // Executa (espera que a classe tenha Execute())
        var method = t.GetMethod("Execute");

        if (method == null)
        {
            Debug.LogError($"[ItemData] Classe '{logicClassName}' não possui método Execute().");
            return;
        }

        method.Invoke(instance, null);
    }
}