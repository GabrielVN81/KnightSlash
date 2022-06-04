using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLogic : MonoBehaviour
{
	public LayerMask layerMask;
	public Rigidbody2D rb2D;
	private RigidbodyConstraints2D originalConstraints;
	public PlaySound sonidoCaer;
	public PlaySound sonidoRomper;
	private int cont = 0;

    // Start is called before the first frame update
    void Start()
    {
		originalConstraints = rb2D.constraints;
		rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.collider.name == "Player")
		{
			sonidoRomper.ReproducirSonido();
			Destroy(this.gameObject);
		} else if (collision.collider.name == "Suelo") {
			sonidoRomper.ReproducirSonido();
			Destroy(this.gameObject,.4f);
		} else if (collision.collider.name.Substring(0,4) == "Enemy") 
        {
			sonidoRomper.ReproducirSonido();
			Destroy(this.gameObject);
        }
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		//Length of the ray
		float laserLength = 30f;

		//Get the first object hit by the ray
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, laserLength, layerMask);

		//If the collider of the object hit is not NUll
		if (hit.collider != null)
		{
			string name = hit.collider.name;
			if (name == "Player") 
			{
				if (cont == 0)
				{
					sonidoCaer.ReproducirSonido();
					cont ++;
				}
				rb2D.constraints = originalConstraints;
				rb2D.gravityScale = 3;
			}


			Debug.DrawRay(transform.position, Vector2.down * laserLength, Color.yellow);
		} else
        {

			Debug.DrawRay(transform.position, Vector2.down * laserLength, Color.blue);
		}
	}
}
