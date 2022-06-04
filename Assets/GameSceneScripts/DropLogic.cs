using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLogic : MonoBehaviour
{
	//--------------------------------------------------------------------------
	//Variables y objetos necesarios para el script
	//--------------------------------------------------------------------------
	public LayerMask layerMask;
	public Rigidbody2D rb2D;
	private RigidbodyConstraints2D originalConstraints;
	public PlaySound sonidoCaer;
	public PlaySound sonidoRomper;
	private int cont = 0;
	//--------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
		//Guarda las constraints de la entidad 
		originalConstraints = rb2D.constraints;
		//Automaticamente lo congela en el eje y una vez las guarda
		rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		//Si entra en contacto con el jugador
		if (collision.collider.name == "Player")
		{
			//Destruye el objeto y reproduce el sonido correspondiente
			sonidoRomper.ReproducirSonido();
			Destroy(this.gameObject);
		}
		//Si entra en contacto con el suelo
		else if (collision.collider.name == "Suelo") {
			//Destruye el objeto tras 0.4 segundos y reproduce el sonido correspondiente
			sonidoRomper.ReproducirSonido();
			Destroy(this.gameObject,.4f);
		} 
		//Si entra en contacto con un enemigo
		else if (collision.collider.name.Substring(0,4) == "Enemy") 
        {
			//Destruye el objeto y reproduce el sonido correspondiente
			sonidoRomper.ReproducirSonido();
			Destroy(this.gameObject);
        }
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		//Longitud del rayo que emite constantemente 
		float laserLength = 30f;

		//Obtiene el objeto con el que el rayo ha chocado (Es decir, si el jugador pasa por delante del rayo, lo guarda en esta variable)
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, laserLength, layerMask);

		//Si el objeto con el que el rayo choca tiene collider
		if (hit.collider != null)
		{
			//Si el nombre del objeto con el que choca es "Player"
			string name = hit.collider.name;
			if (name == "Player") 
			{
				//Condicional para reproducir el sonido de que se desprende del techo
				//Usa un contador para que solo sea reproducido una vez
				if (cont == 0)
				{
					sonidoCaer.ReproducirSonido();
					cont ++;
				}
				//Devuelve los valores originales a las constraints del RigidBody2D (Es decir, lo descongela del eje y)
				rb2D.constraints = originalConstraints;
				//Sube la escala de la gravedad para que caiga mas rapido
				rb2D.gravityScale = 3;
			}

			//Muestra un rayo que representa al rayo real y le da color para hacer Debugging
			Debug.DrawRay(transform.position, Vector2.down * laserLength, Color.yellow);
		} else
        {

			Debug.DrawRay(transform.position, Vector2.down * laserLength, Color.blue);
		}
	}
}
