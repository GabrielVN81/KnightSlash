using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ContinueScript : MonoBehaviour, IPointerEnterHandler
     , IPointerExitHandler, IPointerClickHandler
{

    public TMP_Text textMeshPro;
    private string originalText;

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

    public void OnPointerEnter(PointerEventData eventData)
    {

        string textoS = "<<"+ originalText +">>";
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
        Debug.Log("Continue request");
        Destroy(GameObject.Find("Music"));
    }

}