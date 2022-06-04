using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        originalConstraints = rb2D.constraints; 
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            //DISTANCIA ENTRE EL JUGADOR Y LA ENTIDAD ENEMIGA
            float distToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (isAPlayer())
            {
                if (transform.position.x < player.position.x)
                {
                    transform.localScale = new Vector2(2.2f, 2.2f);
                }
                else
                {
                    //Rotar enemigo a la izquierda
                    transform.localScale = new Vector2(-2.2f, 2.2f);
                }
                rb2D.velocity = new Vector2(0, 0);
                rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
                animator.SetBool("Attack", true);
            }
            else
            {
                rb2D.constraints = originalConstraints;
                animator.SetBool("Attack", false);
            }


            if (distToPlayer < agroRange && distToPlayer > 2 && IsGrounded() && (Mathf.Round(player.transform.position.x) != Mathf.Round(transform.position.x)) && !isAlly())
            {
                BeginChase();
            }
            else
            {
                StopChase();
                if (isAlly()) 
                {
                    if (transform.position.x < player.position.x)
                    {
                        transform.localScale = new Vector2(2.2f, 2.2f);
                    }
                    else
                    {
                        //Rotar enemigo a la izquierda
                        transform.localScale = new Vector2(-2.2f, 2.2f);
                    }
                }
                if (!IsGrounded()) 
                {
                    if (transform.position.x < player.position.x)
                    {
                        transform.localScale = new Vector2(2.2f, 2.2f);
                    }
                    else
                    {
                        //Rotar enemigo a la izquierda
                        transform.localScale = new Vector2(-2.2f, 2.2f);
                    }
                }
            }

            if (distToPlayer < agroRange)
            {
                animator.SetBool("chasing", true);
            }

            if (Mathf.Abs(rb2D.velocity.x) > 0 && !isGonnaCollide())
            {
                animator.SetBool("run", true);
            }
            else
            {
                animator.SetBool("run", false);
            }
        }
        else
        {
            rb2D.velocity = new Vector2 (0,0);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.GetType() == typeof(BoxCollider2D))
        {
            TakeDamage();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "DropSpike")
        {
            health = 0;
            TakeDamage();
        }
    }

    bool isAlly ()
    {
        return Physics2D.OverlapCircle(allyCheck.position, 0.2f, layerEnemigo);
    }

    bool isAPlayer() 
    {
        return Physics2D.OverlapBox(playerCheck.position, playerCheckSize, 0, playerLayer);
    }

    bool isGonnaCollide()
    {
        return Physics2D.OverlapBox(colliderCheck.position, colliderCheckSize, 0, colliderLayer);
    }

    bool IsGrounded() 
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void BeginChase() 
    {
        //Si el enemigo se encuentra a la izquierda del jugador
        if (transform.position.x < player.position.x)
        {
            rb2D.velocity = new Vector2(moveSpeed, 0);

            //Rotar enemigo a la derecha
            transform.localScale = new Vector2(2.2f, 2.2f);
        } 
        //Si el enemigo se encuentra a la derecha del jugador
        else 
        {
            rb2D.velocity = new Vector2(-moveSpeed, 0);

            //Rotar enemigo a la izquierda
            transform.localScale = new Vector2(-2.2f, 2.2f);
        }
    }

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

    void Dead()
    {
        animator.SetBool("Dead", true);
        deadSound.ReproducirSonido();
        StopChase();
        rb2D.gravityScale = 0;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }

    void Attack() {
        if (player.GetComponent<PlayerMovement>().isAlive)
        espadazo.ReproducirSonido();
        player.GetComponent<PlayerMovement>().TakeDamage();
    }

    public void StopChase() 
    {
        rb2D.velocity = new Vector2(0 , 0);
        animator.SetBool("chasing", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(colliderCheck.position,colliderCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(playerCheck.position, playerCheckSize);
    }
}
