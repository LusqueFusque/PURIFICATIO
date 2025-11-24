using UnityEngine;

/// <summary>
/// Controla a trilha sonora do Menu principal.
/// </summary>
public class MenuMusicHandler : MonoBehaviour
{
    [Header("Trilha Sonora do Menu")]
    public AudioClip menuMusic;
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
        // 🎵 Inicia música do menu
        if (menuMusic != null)
        {
            musicSource.clip = menuMusic;
            musicSource.Play();
            Debug.Log("[Menu] 🎶 Música do menu iniciada em loop.");
        }
        else
        {
            Debug.LogWarning("[Menu] ⚠️ Nenhuma música atribuída em menuMusic.");
        }
    }

    void OnDisable()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("[Menu] 🛑 Música do menu parada.");
        }
    }
}