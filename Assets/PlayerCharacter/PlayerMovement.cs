using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("For input actions")]
    public PlayerInput playerInput;

    private int attackCount=3;
    public bool pause;

    [Header("For collision checks")]
    public CapsuleCollider2D capsuleCollider;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Rigidbody2D rb2D;

    [Header("For animations")]
    public Animator animator;

    [Header("For Magic Skills")]
    public ParticleSystem magicCircle1;
    public ParticleSystem magicCircle2;
    public ParticleSystem magicCircle3;
    public ParticleSystem magicCircle4;
    public ParticleSystem magicCircle5;
    public ParticleSystem magicFog;
    public UnityEngine.Experimental.Rendering.Universal.Light2D light2D;
    private float magicTimer = 0f;
    private bool charge = false;
    private bool skill = false;

    [Header("For Player movement and attributes")]
    public float airMoveSpeed = 30f;
    public StaminaBarScript staminaBar;
    public int maxStamina;
    private int stamina;
    public HealthBarScript healthBar;
    public int maxHealth;
    public float health;
    public bool isAlive;
    private float moveSpeed = 10f;
    private float jumpForce = 25f;
    private float horizontal;
    private bool isJumping;
    private bool isFacingRight = true;
    private bool dash = false;
    private bool isBlocking;
    private float dashTimer;
    private float dashCoolDownTimer;
    private bool caida;
    [SerializeField] ParticleSystem trailEffect;
    [SerializeField] SpriteRenderer spriteRenderer;
    private RigidbodyConstraints2D originalConstraints;

    [Header("For time chronometer")]
    private float timeRemaining;

    [Header("For WallSliding")]
    [SerializeField] float wallSlideSpeed;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform wallCheckPoint;
    [SerializeField] Vector2 wallCheckSize;

    [Header("For WallJumping")]
    [SerializeField] float walljumpforce;
    [SerializeField] Vector2 walljumpAngle;
    [SerializeField] float walljumpDirection = -1;

    [Header("For DeadScreen")]
    [SerializeField] Image img;
    [SerializeField] Animator deadTextAnimation;
    [SerializeField] GameObject interactuarText;
    private bool interactuarActive;
    [SerializeField] GameObject datosGuardadosText;
    [SerializeField] GameObject reintentarButton;

    [Header("For pause")]
    public GameObject pauseObject;

    [Header("For Sounds")]
    [SerializeField] PlaySound pies;
    [SerializeField] PlaySound caer;
    [SerializeField] PlaySound saltar;
    [SerializeField] PlaySound escudo;
    [SerializeField] PlaySound espadazo1;
    [SerializeField] PlaySound espadazo2;
    [SerializeField] PlaySound espadazo3;
    [SerializeField] PlaySound deadSound;
    [SerializeField] PlaySound youDied;
    [SerializeField] PlaySound hitSound;
    [SerializeField] PlaySound magicCirclesSound;
    [SerializeField] PlaySound dashSound;
    private PlaySound statueSound;

    void Awake()
    {
        PlayerData pData = SaveManager.LoadPlayerData();
        if (pData != null)
        {
            health = pData.salud;
            transform.position = new Vector3(pData.position[0], pData.position[1], pData.position[2]);
        }
    }

    void Start()
    {
        pauseObject.SetActive(false);
        datosGuardadosText.SetActive(false);
        interactuarText.SetActive(false);
        isAlive = true;
        originalConstraints = rb2D.constraints;
        reintentarButton.SetActive(false);
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth((int)Mathf.Round(health));
        walljumpAngle.Normalize();
        staminaBar.SetMaxStamina(maxStamina);
        stamina = 100;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Die") 
        {
            Dead();
            //deadTextAnimation.SetBool("Dead", false);
        }
        else if (collision.tag == "Statue")
        {
            statueSound = collision.gameObject.GetComponent<PlaySound>();
            interactuarActive = true;
            interactuarText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Statue")
        {
            datosGuardadosText.SetActive(false);
            interactuarActive = false;
            interactuarText.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string name = collision.collider.name;
        if (name.Contains("DropSpike"))
        {
            Dead();
        }
    }

    private bool canAttack;
    // Update is called once per frame
    void Update()
    {

        if (!IsGrounded())
        {
            caida = true;
        }

        if (caida && IsGrounded())
        {
            caida = false;
            caerSonido();
        }
        //------------------------------------------------------------------------------
        //UTILIZADO PARA RESTRINGIR EL MOVIMIENTO MIESTRAS EL PERSONAJE BLOQUEA
        //------------------------------------------------------------------------------

        if (isBlocking)
        {
            rb2D.velocity = new Vector2(0, 0);
        }

        //------------------------------------------------------------------------------
        //UTILIZADO PARA EL SISTEMA DEL DASH (HABILIDAD PARA AVANZAR HACIA ADELANTE)
        //------------------------------------------------------------------------------
        if (dashCoolDownTimer <= 0 && stamina <=100 && !isBlocking)
        {
            Debug.Log("Chargin stamina");
            stamina += 1;
            staminaBar.SetStamina(stamina);
        }


        if (dashCoolDownTimer >= 0)
        {
            dashCoolDownTimer-= Time.deltaTime;
        }

        if (dash == true)
        {
            dashCoolDownTimer = 5f;
            dashTimer += Time.deltaTime;
            if (dashTimer >= .15f)
            {
                dash = false;
                spriteRenderer.color = new Color(214, 214, 214, 255);
                rb2D.constraints = originalConstraints;
                capsuleCollider.enabled = true;
                dashTimer = 0f;
            }
        }

        //------------------------------------------------------------------------------
        //UTILIZADO PARA EL SISTEMA DE MUERTE, HEALTH = 0 => DEAD
        //------------------------------------------------------------------------------

        if (health <= 0)
        {
            Dead();
        }

        //------------------------------------------------------------------------------
        //UTILIZADO PARA EL SISTEMA DE MOVIMIENTO 
        //------------------------------------------------------------------------------
        if (IsGrounded() && !dash && !isBlocking && isAlive)
        {
            rb2D.velocity = new Vector2(horizontal * moveSpeed, rb2D.velocity.y);
        } else if (!IsGrounded() && !isTouchingWall()) 
        {
            rb2D.AddForce(new Vector2(airMoveSpeed * horizontal, 0));
            if (Mathf.Abs(rb2D.velocity.x) > moveSpeed) 
            {
                rb2D.velocity = new Vector2(horizontal * moveSpeed, rb2D.velocity.y);
            }
        }
        //------------------------------------------------------------------------------
        //UTILIZADO PARA LA HABILIDAD MÁGICA DEL PERSONAJE
        //------------------------------------------------------------------------------
        if (charge == true && IsGrounded())
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            magicCharge();
        }
        else if (skill)
        {
            magicTimer += Time.deltaTime;
            Debug.Log("" + magicTimer);
            if (magicTimer < 24.2f)
            {
                animator.SetBool("isCharging", false);
                jumpForce = 30f;
                moveSpeed = 13f;
            }
            else
            {
                animator.SetBool("isCharging", false);
                skill = false;
                moveSpeed = 10f;
                jumpForce = 25f;
                magicTimer = 0f;
                light2D.color = Color.white;
                light2D.intensity = .4f;
            }
        }
        //------------------------------------------------------------------------------
        //ANIMACIONES PARA CORRER
        //------------------------------------------------------------------------------
        if (Mathf.Abs(rb2D.velocity.x) > 0f && charge == false)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        //------------------------------------------------------------------------------
        //COMPRUEBA SI DEBE DAR LA VUELTA AL PERSONAJE O NO
        //------------------------------------------------------------------------------
        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }
        //------------------------------------------------------------------------------
        //COMPRUEBA SI EL PERSONAJE ESTÁ TOCANDO EL SUELO Y ASIGNA LA ANIMACIÓN
        //------------------------------------------------------------------------------
        if (IsGrounded())
        {
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }

        //------------------------------------------------------------------------------
        //COMPRUEBA SI EL PERSONAJE ESTÁ TOCANDO UNA PARED Y ASIGNA LA ANIMACIÓN
        //------------------------------------------------------------------------------
        if (isTouchingWall())
        {
            if (dash == true)
            {
                rb2D.velocity = new Vector2(0, 0);
            }
            Debug.Log("IS TOUCHING A WALL"); 
            animator.SetBool("isTouchingWall", true);
        }
        else
        {
            animator.SetBool("isTouchingWall", false);
        }

        if (isTouchingWall() && !IsGrounded()) 
        {
            rb2D.velocity = new Vector2(0, -wallSlideSpeed);
        }

        //------------------------------------------------------------------------------
        //USADO PARA ESTABLECER LA ANIMACIÓN DE SALTO
        //------------------------------------------------------------------------------
        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
        }
        else if (isJumping && !pause)
        {
            animator.SetBool("isJumping", false);
            isJumping = false;
        }
    }

    public void Attack(InputAction.CallbackContext context) {

        if (context.performed && isAlive && !pause)
        {
            if (attackCount == 3)
            {
                animator.SetTrigger("Attack3");
                attackCount = 1;
            }
            else if (attackCount == 2)
            {
                animator.SetTrigger("Attack2");
                attackCount = 3;
            }
            else if (attackCount == 1)
            {
                animator.SetTrigger("Attack1");
                attackCount = 2;
            }
        } 
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !isBlocking && isAlive && !pause)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
            isJumping = true;
            timeRemaining = 0.2f;
            animator.SetBool("isJumping", true);
            animator.SetBool("isGrounded", false);
        }

        if (context.performed && !IsGrounded() && !pause)
        {
            if (isTouchingWall())
            {
                rb2D.AddForce(new Vector2(walljumpforce * walljumpAngle.x * walljumpDirection, walljumpforce * walljumpAngle.y), ForceMode2D.Impulse);
                Flip();
            }
        }

        if (context.canceled && rb2D.velocity.y > 0f && !pause)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y * 0.5f);
        }

    }

    public void Charge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            charge = true;
            Debug.Log(charge);
        }

        if (context.canceled)
        {
            charge = false;
            if (magicTimer < 2.8f)
            {
                animator.SetBool("isCharging", false);
                magicTimer = 0f;
                Debug.Log(charge);
                magicCircle1.Stop();
                magicCircle2.Stop();
                magicCircle3.Stop();
                magicCircle4.Stop();
                magicCircle5.Stop();
                magicFog.Stop();
                light2D.color = Color.white;
                light2D.intensity = 0.4f;
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    bool isTouchingWall()
    {
        return Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);
    }

    private void Flip()
    {
        if (!pause)
        {
            walljumpDirection *= -1;
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void Interactuar() 
    { 
        if (interactuarActive && !pause)
        {
            SaveManager.SavePlayerData(this);
            datosGuardadosText.SetActive(true);
            statueSound.ReproducirSonido();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isAlive && !pause)
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Block(InputAction.CallbackContext context)
    {
        if (context.performed && stamina >= 10 && IsGrounded() && !pause)
        {
            isBlocking = true;
            rb2D.velocity = new Vector2(0, 0);
            animator.SetBool("Block",true);
        } 
        if (context.canceled)
        {
            isBlocking = false;
            animator.SetBool("Block", false);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (dashCoolDownTimer <= 4.5 && stamina >=32 && !pause) {
            trailEffect.Play();
            isBlocking = false;
            if (isFacingRight)
            {
                spriteRenderer.color = Color.black;
                Debug.Log("R dash");
                dashSound.ReproducirSonido();
                rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
                capsuleCollider.enabled = false;
                dash = true;
                rb2D.AddForce(transform.right * 15f, ForceMode2D.Impulse);
                stamina = stamina - 15;
                staminaBar.SetStamina(stamina);
            } else
            {
                spriteRenderer.color = Color.black;
                Debug.Log("L dash");
                dashSound.ReproducirSonido();
                rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
                capsuleCollider.enabled = false;
                dash = true;
                rb2D.AddForce(-transform.right * 15f, ForceMode2D.Impulse);
                stamina = stamina - 15;
                staminaBar.SetStamina(stamina);
            }
        }
        
    }

    public void TakeDamage() 
    {
        if (!isBlocking && isAlive && !pause)
        {
            health = health - 34f;
            animator.SetTrigger("damage");
            hitSound.ReproducirSonido();
            healthBar.SetHealth((int)Mathf.Round(health));
        } else if (isAlive)
        {
            Debug.Log(stamina);
            stamina = stamina - 20;
            staminaBar.SetStamina(stamina);
            animator.SetTrigger("HitBlocked");
            escudoSonido();
            if (stamina < 20)
            {
                isBlocking = false;
                animator.SetBool("Block", false);
                animator.ResetTrigger("HitBlocked");
            }
        }
    }

    public void Dead() {
        if (isAlive)
        {
            deadSound.ReproducirSonido();
            youDied.ReproducirSonido();
            img.color = Color.black;
            isAlive = false;
            reintentarButton.SetActive(true);
            deadTextAnimation.SetBool("Dead", true);
        }
    }

    private void magicCharge()
    {
        Debug.Log("" + magicTimer);
        magicTimer += Time.deltaTime;
        if (magicTimer > 1f && magicTimer < 1.1f)
        {
            animator.SetBool("isCharging", true);
            magicFog.Play();
        }
        else if (magicTimer > 2f && magicTimer < 2.1f)
        {
            magicCirclesSound.ReproducirSonido();
            magicCircle1.Play();
        }
        else if (magicTimer > 2.1f && magicTimer < 2.2f)
        {
            magicCircle2.Play();
        }
        else if (magicTimer > 2.2f && magicTimer < 2.3f)
        {
            magicCircle3.Play();
        }
        else if (magicTimer > 2.3f && magicTimer < 2.4f)
        {
            magicCircle4.Play();
        }
        else if (magicTimer > 2.4f && magicTimer < 2.5f)
        {
            magicCircle5.Play();
            light2D.color = Color.cyan;
        }
        else if (magicTimer < 2.6f && magicTimer > 2.4f)
        {
            light2D.intensity = 2.2f;

        }
        else if (magicTimer < 2.7f && magicTimer > 2.6f)
        {
            light2D.intensity = 1.9f;
        }
        else if (magicTimer < 2.8f && magicTimer > 2.7f)
        {
            light2D.intensity = 1.65f;
        }
        else if (magicTimer > 2.8f)
        {
            light2D.intensity = 1.02f;
            skill = true;
        }
    }

    private void piesSonido()
    {
        pies.ReproducirSonido();
    }

    private void saltarSonido()
    {
        saltar.ReproducirSonido();
    }

    private void caerSonido()
    {
        caer.ReproducirSonido();
    }

    private void escudoSonido()
    {
        escudo.ReproducirSonido();
    }

    private void sonidoEspada1() {
        espadazo1.ReproducirSonido();
    }

    private void sonidoEspada2() {
        espadazo2.ReproducirSonido();
    }

    private void sonidoEspada3() {
        espadazo3.ReproducirSonido();
    }

    private void saveData()
    {
        SaveManager.SavePlayerData(this);
        Debug.Log("Datos guardados");
    }

    public void pausarJuego()
    {
        if (!pause)
        {
            pause = true;
            pauseObject.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pause = false;
            pauseObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(wallCheckPoint.position, wallCheckSize);
    }
}
