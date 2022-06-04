using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    public SceneSwitcher sceneSwitcher;
    // Start is called before the first frame update

    //Si el jugador pasa por el final del mapa...
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            //Cambia a la escena de los creditos finales
            sceneSwitcher.OpenScene(3);
        }
    }
}
