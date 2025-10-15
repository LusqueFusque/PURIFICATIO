// SaveData.cs
using System.Collections.Generic;
using System;
using UnityEngine;

// Classe auxiliar para serializar o "Dictionary" de decisões
[System.Serializable]
public class DecisionEntry
{
    public int faseID;
    public string decisao; // "sim" ou "nao"
    
    public DecisionEntry(int id, string decision)
    {
        faseID = id;
        decisao = decision;
    }
}

// Classe principal que representa um slot de salvamento individual
[System.Serializable]
public class SaveData
{
    public List<int> fasesConcluidas = new List<int>();
    // Começa com a Fase 1 desbloqueada
    public List<int> fasesDesbloqueadas = new List<int> { 1 }; 
    
    // Lista de entradas de decisão
    public List<DecisionEntry> decisoes = new List<DecisionEntry>();
    
    public string ultimaAtualizacao = "Novo Jogo";

    public string CalcularFinal()
    {
        int simCount = 0;
        int naoCount = 0;

        foreach (var entry in decisoes)
        {
            if (entry.decisao.Equals("sim", StringComparison.OrdinalIgnoreCase))
            {
                simCount++;
            }
            else if (entry.decisao.Equals("nao", StringComparison.OrdinalIgnoreCase))
            {
                naoCount++;
            }
        }
        
        // Regras: 3 ou + SIM = Bom; 3 ou + NÃO = Ruim; 2 SIM e 2 NÃO = Neutro
        if (simCount >= 3) return "Final Bom";
        if (naoCount >= 3) return "Final Ruim";
        
        // Se não atingiu nenhum dos limites, é neutro ou incompleto
        return "Final Neutro"; 
    }
}

// Classe que encapsula TODOS os slots (corresponde ao arquivo JSON)
[System.Serializable]
public class AllSaveSlots
{
    public SaveData slot1;
    public SaveData slot2;
    public SaveData slot3;
}