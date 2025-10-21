using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Efeito de máquina de escrever para TextMeshProUGUI.
/// O texto aparece letra por letra.
/// </summary>
public class TypewriterEffect : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Velocidade de digitação (caracteres por segundo)")]
    public float typingSpeed = 30f;

    [Tooltip("Se true, clicar pula a animação e mostra o texto completo")]
    public bool canSkipTyping = true;

    [Header("Configurações de Som (Opcional)")]
    [Tooltip("Som de digitação (deixe vazio se não quiser)")]
    public AudioClip typingSound;

    [Tooltip("Tocar som a cada N caracteres (1 = todo caractere)")]
    public int soundEveryNChars = 2;

    private TextMeshProUGUI textComponent;
    private AudioSource audioSource;
    private Coroutine typingCoroutine;
    private string currentFullText = "";
    private bool isTyping = false;
    private int charsSinceLastSound = 0;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        if (textComponent == null)
        {
            Debug.LogError("[TypewriterEffect] TextMeshProUGUI não encontrado!");
            enabled = false;
            return;
        }

        // NOVO: Limpa o texto placeholder imediatamente
        textComponent.text = "";

        // Configura AudioSource se tiver som
        if (typingSound != null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = typingSound;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // Permite pular a animação clicando
        if (isTyping && canSkipTyping)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                SkipTyping();
            }
        }
    }

    /// <summary>
    /// Inicia o efeito de digitação com o texto fornecido.
    /// </summary>
    public void ShowText(string text)
    {
        // Para qualquer animação anterior
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        currentFullText = text;
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    /// <summary>
    /// Pula a animação e mostra o texto completo imediatamente.
    /// </summary>
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        textComponent.text = currentFullText;
        isTyping = false;
    }

    /// <summary>
    /// Verifica se o texto ainda está sendo digitado.
    /// </summary>
    public bool IsTyping()
    {
        return isTyping;
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        textComponent.text = "";
        charsSinceLastSound = 0;

        // Calcula o delay entre caracteres
        float delay = 1f / typingSpeed;

        foreach (char c in text)
        {
            textComponent.text += c;
            charsSinceLastSound++;

            // Toca som de digitação
            if (audioSource != null && typingSound != null)
            {
                if (charsSinceLastSound >= soundEveryNChars)
                {
                    audioSource.PlayOneShot(typingSound, 0.3f);
                    charsSinceLastSound = 0;
                }
            }

            // Aguarda o delay (exceto para espaços e pontuação)
            if (char.IsWhiteSpace(c))
            {
                // Espaços são mais rápidos
                yield return new WaitForSeconds(delay * 0.3f);
            }
            else if (c == '.' || c == '!' || c == '?')
            {
                // Pontuação tem pausa maior
                yield return new WaitForSeconds(delay * 3f);
            }
            else if (c == ',' || c == ';')
            {
                // Vírgulas têm pausa média
                yield return new WaitForSeconds(delay * 2f);
            }
            else
            {
                // Caracteres normais
                yield return new WaitForSeconds(delay);
            }
        }

        isTyping = false;
    }

    /// <summary>
    /// Define a velocidade de digitação dinamicamente.
    /// </summary>
    public void SetTypingSpeed(float newSpeed)
    {
        typingSpeed = newSpeed;
    }

    /// <summary>
    /// Limpa o texto imediatamente.
    /// </summary>
    public void ClearText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        textComponent.text = "";
        isTyping = false;
    }
}