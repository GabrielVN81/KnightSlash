using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ExitMouseOver : MonoBehaviour, IPointerEnterHandler
     , IPointerExitHandler, IPointerClickHandler
{

    public TMP_Text textMeshPro;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        string textoS = "<< Salir >>";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        string textoS = "Salir";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    public void OnPointerClick(PointerEventData eventData) // 3
    {
        Debug.Log("Exit request");
        Application.Quit();
    }

}