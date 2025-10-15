using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // Para leitura/escrita de arquivos
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //---Saves
    private AllSaveSlots allSlotsData;
    private int activeSlotID = 1; // Slot atualmente em uso (padrão Slot 1)
    private string saveFilePath;
    private const string SAVE_FILE_NAME = "SaveData.json";

    // Propriedade para acessar o save do slot ativo
    public SaveData ActiveSaveData => GetSlotData(activeSlotID);

    private void Awake()
    {
        // Singleton: garante que só exista um GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Define o caminho do arquivo de salvamento
            saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

            // Carrega todos os slots ao iniciar o jogo
            LoadAllSlots();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "00. Initialization")
        {
            LoadSplashSequence();
        }
    }

    private void LoadSplashSequence()
    {
        StartCoroutine(SplashCoroutine());
    }

    private System.Collections.IEnumerator SplashCoroutine()
    {
        // Carrega a splash screen
        SceneManager.LoadScene("01. Splash");
        // Espera 3 segundos
        yield return new WaitForSeconds(3f);
        // Carrega o menu
        SceneManager.LoadScene("02. Menu");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //---SALVAMENTO
    // Carrega o arquivo JSON completo. Se não existir, inicializa 3 slots vazios.
    private void LoadAllSlots()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                allSlotsData = JsonUtility.FromJson<AllSaveSlots>(json);
                Debug.Log("Slots de save carregados com sucesso.");
            }
            catch (Exception e)
            {
                Debug.LogError("Erro ao carregar o arquivo de save. Criando novos dados. Erro: " + e.Message);
                InitializeSaveData();
            }
        }
        else
        {
            InitializeSaveData();
        }
    }

    private void InitializeSaveData()
    {
        allSlotsData = new AllSaveSlots
        {
            // Inicializa todos os slots como novos jogos
            slot1 = new SaveData(),
            slot2 = new SaveData(),
            slot3 = new SaveData()
        };
        // Salva imediatamente para criar o arquivo inicial
        SaveAllSlots();
    }

    // Escreve todos os 3 slots no arquivo JSON no disco.
    private void SaveAllSlots()
    {
        try
        {
            // Converte todos os dados para JSON (incluindo os 3 slots)
            string json = JsonUtility.ToJson(allSlotsData, true);
            File.WriteAllText(saveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Falha ao salvar o arquivo: " + e.Message);
        }
    }

    // Retorna os dados de um slot específico para a UI de seleção.
    public SaveData GetSlotData(int slotID)
    {
        return slotID switch
        {
            1 => allSlotsData.slot1,
            2 => allSlotsData.slot2,
            3 => allSlotsData.slot3,
            _ => throw new ArgumentException($"ID de Slot inválido: {slotID}"),
        };
    }

    // Seleciona qual slot o jogador irá usar. Deve ser chamado no menu principal.
    public void SelectActiveSlot(int slotID)
    {
        activeSlotID = slotID;
        Debug.Log($"Slot {slotID} selecionado e ativo.");
    }

    // Reseta o slot especificado e salva no disco.
    public void ResetSlot(int slotID)
    {
        SaveData newData = new SaveData();
        switch (slotID)
        {
            case 1: allSlotsData.slot1 = newData; break;
            case 2: allSlotsData.slot2 = newData; break;
            case 3: allSlotsData.slot3 = newData; break;
            default: return;
        }

        SaveAllSlots();
        Debug.Log($"Slot {slotID} resetado e salvo.");
    }

    // Salva manualmente o progresso do slot ativo.
    public void ManualSave()
    {
        ActiveSaveData.ultimaAtualizacao = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        SaveAllSlots();
        Debug.Log($"Save manual concluído no Slot {activeSlotID}.");
    }

    // Atualiza o progresso da fase e realiza um SAVE AUTOMÁTICO.
    public void UpdateProgressAndAutoSave(int faseID, int proximaFaseID, string decisao)
    {
        SaveData currentData = ActiveSaveData;

        // 1. Fase Concluída
        if (!currentData.fasesConcluidas.Contains(faseID))
        {
            currentData.fasesConcluidas.Add(faseID);
        }

        // 2. Fase Desbloqueada
        if (proximaFaseID > 0 && !currentData.fasesDesbloqueadas.Contains(proximaFaseID))
        {
            currentData.fasesDesbloqueadas.Add(proximaFaseID);
        }

        // 3. Decisão
        decisao = decisao.ToLower();
        DecisionEntry entry = currentData.decisoes.Find(e => e.faseID == faseID);

        if (entry != null)
        {
            entry.decisao = decisao; // Atualiza
        }
        else
        {
            currentData.decisoes.Add(new DecisionEntry(faseID, decisao)); // Adiciona
        }

        // 4. Salva no disco (SAVE AUTOMÁTICO)
        ManualSave();
        Debug.Log($"Progresso da Fase {faseID} com decisão '{decisao}' salvo automaticamente no Slot {activeSlotID}.");
    }
}