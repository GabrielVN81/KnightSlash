using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MouseOver : MonoBehaviour, IPointerEnterHandler
     , IPointerExitHandler
{

    public TMP_Text textMeshPro;

    //Si el raton esta encima del boton, resalta el texto
    public void OnPointerEnter(PointerEventData eventData)
    {

        string textoS = "<< Jugar >>";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    //Si el jugador quita el raton de encima, devuelve el texto a la normalidad
    public void OnPointerExit(PointerEventData eventData)
    {

        string textoS = "Jugar";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

}
