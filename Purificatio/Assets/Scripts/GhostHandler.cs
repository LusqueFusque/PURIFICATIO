using UnityEngine;
using System.Collections.Generic;

public class GhostHandler : MonoBehaviour
{
    public static GhostHandler Instance;

    // Dicionário para mapear nomes de personagens para seus GameObjects
    private Dictionary<string, GhostSpriteManager> ghostDict = new Dictionary<string, GhostSpriteManager>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Inicializa todos os fantasmas presentes na cena
        GhostSpriteManager[] allGhosts = FindObjectsOfType<GhostSpriteManager>();
        foreach (var ghost in allGhosts)
        {
            string nameKey = ghost.gameObject.name;
            if (!ghostDict.ContainsKey(nameKey))
            {
                ghostDict.Add(nameKey, ghost);
            }
        }
    }

    /// <summary>
    /// Mostra apenas o fantasma com o nome especificado.
    /// </summary>
    public void ShowGhost(string characterName)
    {
        HideAllGhosts();

        if (ghostDict.TryGetValue(characterName, out GhostSpriteManager ghost))
        {
            ghost.Show();
            Debug.Log($"[GhostHandler] Mostrando fantasma: {characterName}");
        }
        else
        {
            Debug.LogWarning($"[GhostHandler] Fantasma '{characterName}' não encontrado!");
        }
    }

    /// <summary>
    /// Esconde todos os fantasmas ativos.
    /// </summary>
    public void HideAllGhosts()
    {
        foreach (var ghost in ghostDict.Values)
        {
            ghost.Hide();
        }
    }
}