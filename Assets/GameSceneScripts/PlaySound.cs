using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    //Clase para reproducir un sonido.
    //Reproduce un sonido que se le asigne desde el editor

    public float AudioSourcePitch;
    public float volume;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }

    //Reproduce el sonido
    public void ReproducirSonido() 
    { 
        audioSource.pitch = AudioSourcePitch;
        audioSource.PlayOneShot(audioSource.clip, volume);
    }

}
