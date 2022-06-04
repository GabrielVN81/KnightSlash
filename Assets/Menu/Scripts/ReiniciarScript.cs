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

    public void OnPointerEnter(PointerEventData eventData)
    {

        string textoS = "<<" + originalText + ">>";
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        string textoS = originalText;
        char[] texto = textoS.ToCharArray();
        textMeshPro.SetText(texto);
    }

    public void OnPointerClick(PointerEventData eventData) // 3
    {
        SaveManager.DeleteData();
    }

}
