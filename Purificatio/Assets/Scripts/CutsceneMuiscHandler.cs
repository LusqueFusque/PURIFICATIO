using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla a trilha sonora da cena de cutscenes.
/// Permite voltar ao menu pressionando a tecla Espaço.
/// </summary>
public class CutsceneMusicHandler : MonoBehaviour
{
    [Header("Trilha Sonora da Cutscene")]
    public AudioClip cutsceneMusic;
    private AudioSource musicSource;

    void Awake()
    {
        // Garante que exista um AudioSource configurado
        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = 0.6f;
        musicSource.spatialBlend = 0f; // 2D
    }

    void OnEnable()
    {
        // 🎵 Inicia música da cutscene
        if (cutsceneMusic != null)
        {
            musicSource.clip = cutsceneMusic;
            musicSource.Play();
            Debug.Log("[Cutscene] 🎶 Música da cutscene iniciada em loop.");
        }
        else
        {
            Debug.LogWarning("[Cutscene] ⚠️ Nenhuma música atribuída em cutsceneMusic.");
        }
    }

    void OnDisable()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("[Cutscene] 🛑 Música da cutscene parada.");
        }
    }

    void Update()
    {
        // Se apertar espaço, volta para o menu
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("[Cutscene] ⌨️ Espaço pressionado — voltando ao Menu.");
            SceneManager.LoadScene("02. Menu");
        }
    }
}