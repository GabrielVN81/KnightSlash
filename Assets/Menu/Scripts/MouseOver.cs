using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MouseOver : MonoBehaviour, IPointerEnterHandler
     , IPointerExitHandler
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

        string textoS = "<< Jugar >>";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        string textoS = "Jugar";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

}
