using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public float AudioSourcePitch;
    public float volume;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }

    public void ReproducirSonido() 
    { 
        audioSource.pitch = AudioSourcePitch;
        audioSource.PlayOneShot(audioSource.clip, volume);
    }

}
