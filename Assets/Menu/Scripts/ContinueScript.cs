using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ContinueScript : MonoBehaviour, IPointerEnterHandler
     , IPointerExitHandler, IPointerClickHandler
{

    public TMP_Text textMeshPro;
    private string originalText;

    //Obtiene los datos guardados del jugador, si hay, para mostrar "Continuar" o "Nueva partida"
    void Start()
    {
        PlayerData pData = SaveManager.LoadPlayerData();
        if (pData == null)
        {
            originalText = "Nueva partida";
        }
        else
        {
            originalText = "Continuar";
        }
        textMeshPro.SetText(originalText);
    }

    //Si el jugador pone el raton encima del boton, lo resalta
    public void OnPointerEnter(PointerEventData eventData)
    {

        string textoS = "<<"+ originalText +">>";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    //Devuelve el boton al estado original si el jugador quita el raton de encima
    public void OnPointerExit(PointerEventData eventData)
    {

        string textoS = originalText;
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    //Quita la musica si el jugador pulsa click
    public void OnPointerClick(PointerEventData eventData) // 3
    {
        Debug.Log("Continue request");
        Destroy(GameObject.Find("Music"));
    }

}