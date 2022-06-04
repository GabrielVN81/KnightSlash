using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ReiniciarScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text textMeshPro;
    private string originalText;

    void Start()
    {
        originalText = "Reiniciar";    
    }

    //Si el jugador pone el raton encima del boton, lo resalta
    public void OnPointerEnter(PointerEventData eventData)
    {

        string textoS = "<<" + originalText + ">>";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    //Si el jugador quita de encima del boton el raton, lo devuelve a la normalidad
    public void OnPointerExit(PointerEventData eventData)
    {

        string textoS = originalText;
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    //Si el jugador pulsa el boton, borra los datos guardados
    public void OnPointerClick(PointerEventData eventData) // 3
    {
        SaveManager.DeleteData();
    }

}
