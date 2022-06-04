using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSong : MonoBehaviour
{
    //Script para que la musica del menu se ejecute incluso si cambiamos de escena

    public AudioSource _audioSource;
    private static MenuSong instance = null;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        //Establece la "instancia" de la entidad como la actual
        //como es estatico, se podra acceder desde cualquier lugar
        else {
            instance = this;
        }
        //Establece la musica como un objeto que no se destruye al cambiar de escena
        DontDestroyOnLoad(this.gameObject);
        //Empieza la musica
        _audioSource.Play();
    }
}
