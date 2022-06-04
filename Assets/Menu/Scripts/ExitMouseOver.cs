using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ExitMouseOver : MonoBehaviour, IPointerEnterHandler
     , IPointerExitHandler, IPointerClickHandler
{

    public TMP_Text textMeshPro;

    //Si el raton esta encima del boton lo resalta
    public void OnPointerEnter(PointerEventData eventData)
    {

        string textoS = "<< Salir >>";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    //Si el jugador quita el raton de encima, vuelve el boton a la normalidad
    public void OnPointerExit(PointerEventData eventData)
    {

        string textoS = "Salir";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    //Si el jugador pulsa el boton, cierra el juego
    public void OnPointerClick(PointerEventData eventData) // 3
    {
        Debug.Log("Exit request");
        Application.Quit();
    }

}