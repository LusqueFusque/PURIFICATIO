using UnityEngine;
using UnityEngine.UI;

public class ItemCrowbar : MonoBehaviour
{
    public bool isActive = false; // se o item está "selecionado"
    public Image salaPanel; // imagem principal da sala
    public Sprite madeiraRemovidaSprite; // sprite com a madeira já removida
    private Sprite madeiraOriginalSprite;

    void Start()
    {
        // guarda sprite original pra restaurar se quiser
        if (salaPanel != null)
            madeiraOriginalSprite = salaPanel.sprite;
    }

    void Update()
    {
        // desativar com botão direito do mouse
        if (isActive && Input.GetMouseButtonDown(1))
        {
            isActive = false;
            Debug.Log("Crowbar desativado.");
        }
    }

    // chamado quando o botão do inventário é clicado
    public void OnItemClicked()
    {
        isActive = !isActive;
        Debug.Log(isActive ? "Crowbar ativado." : "Crowbar desativado.");
    }

    // este método será chamado quando clicar em algo interativo no cenário
    public void TryUseOn(GameObject target)
    {
        if (!isActive) return;

        if (target.CompareTag("WoodLoose"))
        {
            Debug.Log("Madeira removida com o pé de cabra!");
            salaPanel.sprite = madeiraRemovidaSprite;
            isActive = false; // desativa após uso
        }
        else
        {
            Debug.Log("Nada a fazer com o pé de cabra aqui.");
        }
    }
}
