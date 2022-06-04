using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

//Multiherencia, utilizada para el funcionamiento de los botones del menu de pausa
public class PauseScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //-----------------------------------------------------------------------
    //Variables y objetos necesarios para el funcionamiento del script
    //-----------------------------------------------------------------------
    public TMP_Text textMeshPro;
    public PlayAudio pAudio;
    private string originalText;
    public GameObject player;
    public SceneSwitcher sceneSwitcher;
    //-----------------------------------------------------------------------

    //Metodo llamado cuando el raton esta encima de un boton
    public void OnPointerEnter (PointerEventData eventData) 
    {
        //Resalta el texto del boton y le sube el tamaño, ademas de reproducir el sonido correspondiente

        originalText = textMeshPro.GetParsedText();
        string texto ="<<" + textMeshPro.GetParsedText() + ">>";
        char[] text = texto.ToCharArray();
        textMeshPro.SetText(text);
        pAudio.DropAudio();
        textMeshPro.fontSize = 60;

    }

    //Metodo llamado cuando el raton ya no esta encima de un boton, devuelve el boton a la normalidad
    public void OnPointerExit(PointerEventData eventData)
    {
        string texto = originalText;
        char[] text = texto.ToCharArray();
        textMeshPro.SetText(text);
        textMeshPro.fontSize = 50;
    }

    //Metodo para cuando un boton es pulsado
    public void OnPointerClick(PointerEventData eventData)
    {
        //Estructura para saber cual de los botones fue pulsado

        //Si se pulsa "Salir del juego"
        if (originalText.Equals("Salir del juego"))
        {
            //Cierra el juego
            Application.Quit();
        } 
        //Si se pulsa "Continuar"
        else if (originalText.Equals("Continuar"))
        {
            //Devuelve el tiempo a la normalidad y establece la variable de pausa
            //y el menu en el script PlayerMovement a false
            Time.timeScale = 1;
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.pause = false;
            playerMovement.pauseObject.SetActive(false);

        } 
        //Si se pulsa "Reiniciar progreso"
        else if (originalText.Equals("Reiniciar progreso"))
        {
            //Borra los datos que haya guardados
            SaveManager.DeleteData();
            //Reinicia la escena
            sceneSwitcher.OpenScene(2);
        } 
        //Si se pulsa "Salir al menu principal"
        else if (originalText.Equals("Salir al menu principal"))
        {
            //Devuelve el tiempo a la normalidad y regresa al menu principal
            Time.timeScale = 1;
            sceneSwitcher.OpenScene(0);
        }
    }

}
