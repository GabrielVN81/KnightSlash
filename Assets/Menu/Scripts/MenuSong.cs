using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSong : MonoBehaviour
{

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
        else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        Debug.Log("Awake: " + this.gameObject);
        _audioSource.Play();
    }
}
