using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PauseScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text textMeshPro;
    public PlayAudio pAudio;
    private string originalText;
    public GameObject player;
    public SceneSwitcher sceneSwitcher;
    public void OnPointerEnter (PointerEventData eventData) 
    {
        originalText = textMeshPro.GetParsedText();
        string texto ="<<" + textMeshPro.GetParsedText() + ">>";
        char[] text = texto.ToCharArray();
        textMeshPro.SetText(text);
        pAudio.DropAudio();
        textMeshPro.fontSize = 60;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        string texto = originalText;
        char[] text = texto.ToCharArray();
        textMeshPro.SetText(text);
        textMeshPro.fontSize = 50;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (originalText.Equals("Salir del juego"))
        {
            Debug.Log("Quit request");
            Application.Quit();
        } else if (originalText.Equals("Continuar"))
        {
            Time.timeScale = 1;
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.pause = false;
            playerMovement.pauseObject.SetActive(false);

        } else if (originalText.Equals("Reiniciar progreso"))
        {
            SaveManager.DeleteData();
            sceneSwitcher.OpenScene(2);
        } else if (originalText.Equals("Salir al menu principal"))
        {
            Time.timeScale = 1;
            sceneSwitcher.OpenScene(0);
        }
    }

}
