using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public GameObject audioObject;

    //Metodo para reproducir el audio que asignemos desde el editor, usado para los botones
    public void DropAudio() {
        Instantiate(audioObject, transform.position, transform.rotation);
    }
}
