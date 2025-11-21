using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Muda o cursor para mão ao passar por cima
/// </summary>
public class CursorPointerChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Cursor")]
    [Tooltip("Textura do cursor (mãozinha)")]
    public Texture2D pointerCursor;
    
    private Vector2 cursorHotspot = Vector2.zero; // Ponto de clique do cursor

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (pointerCursor != null)
        {
            Cursor.SetCursor(pointerCursor, cursorHotspot, CursorMode.Auto);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Se apontei e saí, volta ao padrão
        ResetCursor();
    }
    
    // OPCIONAL: Implementa IPointerDownHandler para limpar imediatamente no clique
    public void OnPointerDown(PointerEventData eventData)
    {
        // Se clicar (e for destruir), já reseta, embora OnDestroy vá fazer o mesmo.
        ResetCursor();
    }

    // Chama o reset do cursor quando o objeto é destruído
    private void OnDestroy()
    {
        ResetCursor();
    }
    
    // Alternativamente, use OnDisable, que também é chamado antes de OnDestroy
    private void OnDisable()
    {
        // Garante o reset caso o componente ou GameObject seja desativado
        // (Isso é chamado antes de OnDestroy, mas pode ser chamado em outros momentos)
        ResetCursor();
    }

    private void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); 
    }
}