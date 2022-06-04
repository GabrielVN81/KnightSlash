using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAudioClone : MonoBehaviour
{
    // Start is called before the first frame update

    //Destruye el objeto al que se le añada este script tras 1 segundo
    //Usado para los sonidos de los botones
    void Start()
    {
        Destroy(this.gameObject, 1f);        
    }

}
