using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //VARIABLES Y OBJETOS NECESARIOS PARA EL CORRECTO FUNCIONAMIENTO DEL SCRIPT
    //--------------------------------------------------------------------------
    [Header("Jugador al que persigue")]
    public Transform player;

    [Header("Componentes del enemigo")]
    public LayerMask layerEnemigo;
    public Transform allyCheck;
    public Rigidbody2D rb2D;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform colliderCheck;
    public LayerMask colliderLayer;
    public Vector2 colliderCheckSize;
    public Transform playerCheck;
    public LayerMask playerLayer;
    public Vector2 playerCheckSize;
    public PlaySound bloodHitEffect;
    public PlaySound deadSound;
    public PlaySound espadazo;
    private RigidbodyConstraints2D originalConstraints;

    [Header("Atributos del enemigo")]
    public float agroRange;
    public float moveSpeed;
    public float health;
    public float attackDamage;
    //--------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        //Guarda las constraints originales de la entidad
        originalConstraints = rb2D.constraints; 
    }

    // Update is called once per frame
    void Update()
    {
        //Si el enemigo esta con vida
        if (health > 0)
        {
            //CALCULA LA DISTANCIA ENTRE EL JUGADOR Y LA ENTIDAD ENEMIGA
            float distToPlayer = Vector2.Distance(transform.position, player.transform.position);

            //Comprueba si hay un jugador delante suya
            if (isAPlayer())
            {
                //Condicional para rotar al enemigo segun la posicion del jugador
                if (transform.position.x < player.position.x)
                {
                    //Rotar al enemigo a la derecha
                    transform.localScale = new Vector2(2.2f, 2.2f);
                }
                else
                {
                    //Rotar enemigo a la izquierda
                    transform.localScale = new Vector2(-2.2f, 2.2f);
                }

                //Establece la velocidad a 0 si el jugador esta delante
                rb2D.velocity = new Vector2(0, 0);
                //Congela su posicion para que el jugador no pueda empujar a los enemigos
                rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
                animator.SetBool("Attack", true);
            }
            //Si no hay un jugador delante suya (Justo en el area que tiene delante de si)
            else
            {
                //Devuelve las constraints del RigidBody2D a las originales
                rb2D.constraints = originalConstraints;
                animator.SetBool("Attack", false);
            }

            //Si la distancia con el jugador es menor que la distancia de "agro" (distancia en que lo detecta)
            //si la distancia con el jugador es mayor de 2 (para que no siga acercandose al jugador si esta tan cerca suya)
            //si esta tocando el suelo,si la posicion en el eje x del enemigo es distinta del eje x del jugador (para que no se vuelva loco si el jugador pasa por encima suya)
            //y si no tiene delante justo a una entidad enemiga 
            if (distToPlayer < agroRange && distToPlayer > 2 && IsGrounded() && (Mathf.Round(player.transform.position.x) != Mathf.Round(transform.position.x)) && !isAlly())
            {
                //Comienza a perseguir al jugador
                BeginChase();
            }
            else
            {
                //Deja de perseguir al jugador
                StopChase();
                //Si tiene delante una entidad enemiga
                if (isAlly()) 
                {
                    //Se asegura de estar mirando siempre hacia el jugador
                    if (transform.position.x < player.position.x)
                    {
                        //Rotar enemigo a la derecha
                        transform.localScale = new Vector2(2.2f, 2.2f);
                    }
                    else
                    {
                        //Rotar enemigo a la izquierda
                        transform.localScale = new Vector2(-2.2f, 2.2f);
                    }
                }

                //Si no esta tocando el suelo
                if (!IsGrounded()) 
                {
                    //Se asegura de estar mirando siempre hacia el jugador
                    if (transform.position.x < player.position.x)
                    {
                        //Rotar enemigo a la derecha
                        transform.localScale = new Vector2(2.2f, 2.2f);
                    }
                    else
                    {
                        //Rotar enemigo a la izquierda
                        transform.localScale = new Vector2(-2.2f, 2.2f);
                    }
                }
            }

            //Si la distancia con el jugador es la suficiente como para que lo detecte
            if (distToPlayer < agroRange)
            {
                //Establece la animacion de perseguir
                animator.SetBool("chasing", true);
            }

            //Si el enemigo se esta moviendo y no se va a chocar con nua pared
            if (Mathf.Abs(rb2D.velocity.x) > 0 && !isGonnaCollide())
            {
                animator.SetBool("run", true);
            }
            else
            {
                animator.SetBool("run", false);
            }
        }
        //Si el enemigo esta muerto
        else
        {
            //Establece su velocidad a 0
            rb2D.velocity = new Vector2 (0,0);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Comprueba si el collider que se activa en la espada del jugador cuando este ataca, le ha dado
        if (collision.tag == "Player" && collision.GetType() == typeof(BoxCollider2D))
        {
            //Si le da, llama a TakeDamage()
            TakeDamage();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si una roca que cae del techo le da
        if (collision.collider.name == "DropSpike")
        {
            //Mata al enemigo
            health = 0;
            TakeDamage();
        }
    }

    //Comprueba si tiene delante a una entidad enemiga (que es una entidad aliada para el enemigo)
    bool isAlly ()
    {
        return Physics2D.OverlapCircle(allyCheck.position, 0.2f, layerEnemigo);
    }

    //Comprueba si tiene delante al jugador
    bool isAPlayer() 
    {
        return Physics2D.OverlapBox(playerCheck.position, playerCheckSize, 0, playerLayer);
    }

    //Comprueba si tiene delante una pared
    bool isGonnaCollide()
    {
        return Physics2D.OverlapBox(colliderCheck.position, colliderCheckSize, 0, colliderLayer);
    }

    //Comprueba si tiene suelo delante de sus pies (para evitar que se precipite por una plataforma)
    bool IsGrounded() 
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    //Metodo para dirigirse hacia el jugador
    public void BeginChase() 
    {
        //Si el enemigo se encuentra a la izquierda del jugador
        if (transform.position.x < player.position.x)
        {
            //Se mueve hacia el jugador
            rb2D.velocity = new Vector2(moveSpeed, 0);

            //Rotar enemigo a la derecha
            transform.localScale = new Vector2(2.2f, 2.2f);
        } 
        //Si el enemigo se encuentra a la derecha del jugador
        else 
        {
            //Se mueve hacia el jugador
            rb2D.velocity = new Vector2(-moveSpeed, 0);

            //Rotar enemigo a la izquierda
            transform.localScale = new Vector2(-2.2f, 2.2f);
        }
    }

    //Metodo para recibir daño 
    public void TakeDamage() 
    {
        health = health - 40;
        animator.SetTrigger("Hit");
        bloodHitEffect.ReproducirSonido();
        if (health <= 0)
        {
            Dead();
        }
    }

    //Metodo para hacer que el enemigo muera
    void Dead()
    {
        animator.SetBool("Dead", true);
        deadSound.ReproducirSonido();
        StopChase();
        rb2D.gravityScale = 0;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }

    //Metodo para hacer que el enemigo ataque
    void Attack() {
        //Si el personaje del jugador esta vivo reproduce el sonido del espadazo cuando ataca 
        //y hace al jugador recibir daño
        if (player.GetComponent<PlayerMovement>().isAlive)
        espadazo.ReproducirSonido();
        player.GetComponent<PlayerMovement>().TakeDamage();
    }

    //Metodo para dejar de perseguir al jugador
    public void StopChase() 
    {
        rb2D.velocity = new Vector2(0 , 0);
        animator.SetBool("chasing", false);
    }

    //Metodo para dibujar las areas de comprobaciones, solo se llama en el editor
    //Tiene propositos de Debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(colliderCheck.position,colliderCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(playerCheck.position, playerCheckSize);
    }
}
