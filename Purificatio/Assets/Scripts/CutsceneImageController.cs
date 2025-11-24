using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Importante para usar TMP_Text

/// <summary>
/// Controla qual imagem e texto aparecem na cena "10. Cutscene"
/// baseado no número de exorcismos feitos.
/// </summary>
public class CutsceneImageController : MonoBehaviour
{
    [Header("Referência ao componente Image da UI")]
    public Image cutsceneImage;

    [Header("Referência ao componente TMP_Text da UI")]
    public TMP_Text cutsceneText;

    [Header("Sprites possíveis")]
    public Sprite spriteIgual2;   // Quando total == 2
    public Sprite spriteMaior2;   // Quando total > 2
    public Sprite spriteMenor2;   // Quando total < 2

    [Header("Textos possíveis")]
    [TextArea] public string textoIgual2;   // Texto para == 2
    [TextArea] public string textoMaior2;   // Texto para > 2
    [TextArea] public string textoMenor2;   // Texto para < 2

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "10. Cutscene")
        {
            MostrarResultado();
        }
    }

    void MostrarResultado()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogWarning("[CutsceneImageController] SaveSystem não encontrado!");
            return;
        }

        int totalExorcismos = SaveSystem.Instance.ContarExorcismos();

        if (totalExorcismos == 2)
        {
            if (cutsceneImage != null) cutsceneImage.sprite = spriteIgual2;
            if (cutsceneText != null) cutsceneText.text = textoIgual2;
        }
        else if (totalExorcismos > 2)
        {
            if (cutsceneImage != null) cutsceneImage.sprite = spriteMaior2;
            if (cutsceneText != null) cutsceneText.text = textoMaior2;
        }
        else // totalExorcismos < 2
        {
            if (cutsceneImage != null) cutsceneImage.sprite = spriteMenor2;
            if (cutsceneText != null) cutsceneText.text = textoMenor2;
        }

        Debug.Log("[CutsceneImageController] Resultado exibido para " + totalExorcismos + " exorcismos.");
    }
}
