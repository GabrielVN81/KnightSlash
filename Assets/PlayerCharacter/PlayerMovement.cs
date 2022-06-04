using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

//-----------------------------------------------------------------------------------------------------------------------
//--VARIABLES Y OBJETOS NECESARIOS PARA EL FUNCIONAMIENTO DEL SCRIPT
//-----------------------------------------------------------------------------------------------------------------------
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
    // private bool canAttack;    
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
//-----------------------------------------------------------------------------------------------------------------------
//--FIN DE VARIABLES Y OBJETOS NECESARIOS PARA EL FUNCIONAMIENTO DEL SCRIPT
//-----------------------------------------------------------------------------------------------------------------------

    //Metodo llamado cada vez que empezamos a jugar, incluso si el script de "PlayerMovement" estuviera ""desactivado""
    void Awake()
    {
        //Carga los datos del jugador, si es que los hay (Salud y posicion)
        PlayerData pData = SaveManager.LoadPlayerData();
        if (pData != null)
        {
            health = pData.salud;
            transform.position = new Vector3(pData.position[0], pData.position[1], pData.position[2]);
        }
    }

    //Metodo llamado cada vez que empezamos a jugar
    void Start()
    {
        //Esconde los objetos del canvas que no debemos ver al comenzar
        pauseObject.SetActive(false);
        datosGuardadosText.SetActive(false);
        interactuarText.SetActive(false);
        reintentarButton.SetActive(false);
        
        //Establece a true la variable que determina si esta o no vivo
        isAlive = true;
        //Establece su aguante inicial a 100
        stamina = 100;

        //Almacena en una variable las "constraints" del RigidBody2D del personaje
        //Son cambiadas mas adelante, por lo que con esta variable les devolvemos el
        //estado original
        originalConstraints = rb2D.constraints;
        
        //Establece las barras de vida y stamina
        healthBar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxStamina(maxStamina);

        //Como es posible que el jugador empiece con menos vida (por datos guardados)
        //muestra su vida actual en la barra de vida desde un principio.
        healthBar.SetHealth((int)Mathf.Round(health));
    
        //Normaliza el angulo en que salta el personaje desde una pared
        walljumpAngle.Normalize();
    }

    //Se llama cada vez que el collider del personaje entra en contacto con otros 
    //colliders que son triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si entra en colisión con un collider cuyo tag es "Die"
        //Nos mata
        if (collision.tag == "Die") 
        {
            Dead();
            //deadTextAnimation.SetBool("Dead", false);
        }
        //Si entra en colisión con un collider cuyo tag es "Statue"
        //Nos muestra que podemos interactuar
        else if (collision.tag == "Statue")
        {
            statueSound = collision.gameObject.GetComponent<PlaySound>();
            interactuarActive = true;
            interactuarText.SetActive(true);
        }
    }

    //Se llama cada vez que el collider del personaje deja de tener contacto con otros 
    //colliders que son triggers
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Si deja de tener contacto con el collider cuyo tag es "Statue"
        //deja de mostrar todos los textos correspondientes del canvas
        if (collision.tag == "Statue")
        {
            datosGuardadosText.SetActive(false);
            interactuarActive = false;
            interactuarText.SetActive(false);
        }
    }

    //Se llama cada vez que el collider del personaje colisiona con 
    //un cuerpo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si el nombre del objeto con el que colisiona el personaje es
        //"DropSpike" entonces nos mata
        string name = collision.collider.name;
        if (name.Contains("DropSpike"))
        {
            Dead();
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Metodos que deben ser llamados cada frame para
        //realizar las correspondientes comprobaciones sobre el estado
        //del personaje, asi como para moverse, saltar, etc..
        sonidoCaida();
        dontMoveOnBlock();
        dashUpdate();
        deadUpdate();
        playerMoveUpdate();
        magicUpdate();
        runAnimationsUpdate();
        flipUpdate();
        isGroundedUpdate();
        isTouchingWallUpdate();
        jumpAnimationUpdate();
    }

//-----------------------------------------------------------------------------------------------------------------------
//COMIENZO DE FUNCIONES QUE SON UTILIZADAS EN EL UPDATE    
//-----------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    //UTILIZADO PARA REPRODUCIR EL SONIDO CORRESPONDIENTE CUANDO EL PERSONAJE CAE
    //------------------------------------------------------------------------------
    private void sonidoCaida() {
        //Si no esta en el suelo
        if (!IsGrounded())
        {
            //Esta cayendo
            caida = true;
        }
        //Si esta cayendo pero si esta en el suelo
        //(Significa que ya ha dejado de caer)
        if (caida && IsGrounded())
        {   
            //Ya no esta cayendo
            caida = false;
            //Reproduce el sonido de caida
            caerSonido();
        }
    }

    //------------------------------------------------------------------------------
    //UTILIZADO PARA RESTRINGIR EL MOVIMIENTO MIESTRAS EL PERSONAJE BLOQUEA
    //------------------------------------------------------------------------------
    private void dontMoveOnBlock(){
        //Si el personaje esta bloqueando
        if (isBlocking)
        {
            //La velocidad es 0, no tiene velocidad
            //(Si el jugador intenta mover al personaje, la velocidad se
            //establecera en 0 con esta funcion, de modo que no se movera)
            rb2D.velocity = new Vector2(0, 0);
        }
    }

    //------------------------------------------------------------------------------
    //UTILIZADO PARA EL SISTEMA DEL DASH (HABILIDAD PARA AVANZAR HACIA ADELANTE)
    //------------------------------------------------------------------------------
    private void dashUpdate() {
        //Comprueba si ya ha pasado el tiempo necesario para volver a usar
        //la habilidad, tambien comprueba si no tiene toda la stamina y si no esta bloqueando
        if (dashCoolDownTimer <= 0 && stamina <=100 && !isBlocking)
        {   
            //En caso de que las anteriores condiciones se cumplan,
            //la stamina del personaje se regenera. (Se gasta con cada uso del 'dash')
            stamina += 1;
            staminaBar.SetStamina(stamina);
        }

        //Si el contador es mayor que 0 (Significa que se acaba de usar la habilidad)
        if (dashCoolDownTimer >= 0)
        {
            //Reduce el contador a 0 poco a poco
            dashCoolDownTimer-= Time.deltaTime;
        }

        //Si acaba de usar la habilidad
        if (dash == true)
        {
            //Establece el contador a 5, que seran 5 segundos
            dashCoolDownTimer = 5f;
            dashTimer += Time.deltaTime;

            //Comprueba si han pasado 0.15 segundos desde 
            //que se uso la habilidad
            if (dashTimer >= .15f)
            {
                //Se establece 'dash' a false, significa que ya no esta usando la habilidad
                dash = false;
                //Devuelve al jugador su color original (Tiene el color con tonos reducidos para dar sensacion de oscuridad)
                spriteRenderer.color = new Color(214, 214, 214, 255);
                //Se establecen las constraints del RigidBody2D a las que tenia antes de usar la habilidad
                rb2D.constraints = originalConstraints;
                //Se habilita el collider del personaje, que fue desactivado al usar la habilidad
                capsuleCollider.enabled = true;
                //Se pone a 0 el contador usado para "dejar de usar" la habilidad
                dashTimer = 0f;
            }
        }
    }

    //------------------------------------------------------------------------------
    //UTILIZADO PARA EL SISTEMA DE MUERTE, HEALTH = 0 => DEAD
    //------------------------------------------------------------------------------
    private void deadUpdate() {
        //Comprueba si la vida del personaje es mayor de 0, si no lo es, muere.
        if (health <= 0)
        {
            Dead();
        }
    }

    //------------------------------------------------------------------------------
    //UTILIZADO PARA EL SISTEMA DE MOVIMIENTO 
    //------------------------------------------------------------------------------
    private void playerMoveUpdate() {
        //Comprueba que el jugador esta tocando el suelo, que no esta bloqueando, que ya no esta usando
        //la habilidad 'dash' (que dura 0.15 segundos), y finalmente que este con vida.     
        if (IsGrounded() && !dash && !isBlocking && isAlive)
        {
            //En caso de ser asi, establece la velocidad del jugador hacia la direccion correspondiente
            rb2D.velocity = new Vector2(horizontal * moveSpeed, rb2D.velocity.y);
        } 
        //Comprueba que el jugador no este tocando el suelo ni tampoco ninguna pared
        else if (!IsGrounded() && !isTouchingWall()) 
        {
            //Da un impulso al jugador cuando esta en el aire
            rb2D.AddForce(new Vector2(airMoveSpeed * horizontal, 0));

            //Controla que su velocidad no sea mayor que el maximo establecido
            if (Mathf.Abs(rb2D.velocity.x) > moveSpeed) 
            {
                //Modifica su velocidad si supera el maximo
                rb2D.velocity = new Vector2(horizontal * moveSpeed, rb2D.velocity.y);
            }
        }
    }

    //------------------------------------------------------------------------------
    //UTILIZADO PARA LA HABILIDAD MAGICA DEL PERSONAJE
    //------------------------------------------------------------------------------
    private void magicUpdate(){
        //Si esta cargando y esta en el suelo
        if (charge == true && IsGrounded())
        {
            //Establece la velocidad a 0 y comienza a cargar
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            magicCharge();
        }
        //Si la habilidad ya esta activada
        else if (skill)
        {
            //Establece un contador para determinar cuando desactivar la habilidad
            magicTimer += Time.deltaTime;
            if (magicTimer < 24.2f)
            {
                //Le da mas fuerza para saltar y mas velocidad de movimiento
                animator.SetBool("isCharging", false);
                jumpForce = 30f;
                moveSpeed = 13f;
            }
            //Cuando el tiempo se acaba
            else
            {
                //Desactiva la habilidad y devuelve la iluminacion y los atributos del jugador
                //a su estado original
                animator.SetBool("isCharging", false);
                skill = false;
                moveSpeed = 10f;
                jumpForce = 25f;
                magicTimer = 0f;
                light2D.color = Color.white;
                light2D.intensity = .4f;
            }
        }
    }

    //------------------------------------------------------------------------------
    //ANIMACIONES PARA CORRER
    //------------------------------------------------------------------------------
    private void runAnimationsUpdate() {
        //Si la velocidad del jugador es positiva y no esta cargando la habilidad    
        if (Mathf.Abs(rb2D.velocity.x) > 0f && charge == false)
        {
            //Establece la animacion de correr
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    //------------------------------------------------------------------------------
    //COMPRUEBA SI DEBE DAR LA VUELTA AL PERSONAJE O NO
    //------------------------------------------------------------------------------
    private void flipUpdate() {
        //Si el personaje no esta mirando a la derecha y su velocidad es mayor de 0
        //(esta corriendo al lado derecho)    
        if (!isFacingRight && horizontal > 0f)
        {
            //da la vuelta al personaje
            Flip();
        }
        //Si el personaje esta mirando a la derecha y su velocidad es menor de 0
        //(esta corriendo al lado izquierdo)
        else if (isFacingRight && horizontal < 0f)
        {
            //da la vuelta al personaje
            Flip();
        }
    }

    //------------------------------------------------------------------------------
    //COMPRUEBA SI EL PERSONAJE ESTA TOCANDO EL SUELO Y ASIGNA LA ANIMACION
    //------------------------------------------------------------------------------
    private void isGroundedUpdate(){
        //Si el personaje esta en el suelo    
        if (IsGrounded())
        {
            //Establece el parametro utilizado en el animador
            //Le dice que el jugador esta tocando el suelo
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }
    }

    //------------------------------------------------------------------------------
    //COMPRUEBA SI EL PERSONAJE ESTA TOCANDO UNA PARED Y ASIGNA LA ANIMACION
    //------------------------------------------------------------------------------
    private void isTouchingWallUpdate(){
        //Si el personaje esta tocando una pared    
        if (isTouchingWall())
        {
            //Si el personaje esta usando la habilidad 'dash'
            if (dash == true)
            {
                //Establece su velocidad a 0
                //(Hecho para evitar que atraviese las paredes)
                rb2D.velocity = new Vector2(0, 0);
            }
            //Establece el parametro para avisar al animator que el personaje esta tocando
            //una pared
            animator.SetBool("isTouchingWall", true);
        }
        else
        {
            animator.SetBool("isTouchingWall", false);
        }

        //Si el personaje esta tocando una pared y no esta tocando el suelo
        if (isTouchingWall() && !IsGrounded()) 
        {
            //Comienza a deslizarse por la pared lentamente
            rb2D.velocity = new Vector2(0, -wallSlideSpeed);
        }
    }

    //------------------------------------------------------------------------------
    //USADO PARA ESTABLECER LA ANIMACION DE SALTO
    //------------------------------------------------------------------------------
    private void jumpAnimationUpdate(){
        //El contador se utiliza en concreto para establecer el momento
        //en que el personaje pasa de tener una animacion de salto a 
        //tener una animacion de caida

        //Si el contador es mayor de 0    
        if (timeRemaining > 0f)
        {
            //Resta tiempo al contador
            timeRemaining -= Time.deltaTime;
        }
        //Si el contador no es mayor de 0, el juego no esta pausado 
        //y el jugador esta saltando
        else if (isJumping && !pause)
        {
            //Establece el parametro necesario en el animator
            animator.SetBool("isJumping", false);
            //Establece a false el salto
            isJumping = false;
        }
    }

//-----------------------------------------------------------------------------------------------------------------------
//FIN DE FUNCIONES QUE SON LLAMADAS EN EL UPDATE
//-----------------------------------------------------------------------------------------------------------------------

    //Función de input para atacar
    public void Attack(InputAction.CallbackContext context) {
        //Si ha pulsado el boton, si esta vivo y no ha pausado la partida
        if (context.performed && isAlive && !pause)
        {
            //Estructura para saber cual de los 3 ataques debe hacer
            //Hace un ataque distinto cada vez
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

    //Función de input para atacar
    public void Jump(InputAction.CallbackContext context)
    {
        //Si el jugador ha pulsado el boton, si el personaje esta tocando el suelo, no esta bloqueando,
        //esta vivo y no ha pausado la partida
        if (context.performed && IsGrounded() && !isBlocking && isAlive && !pause)
        {
            //Hace saltar el personaje
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
            //Establece la variable de salto para que los metodos que lo usan
            //para llamar al animator lo sepan
            isJumping = true;
            //Establece el contador que se usa para saber cuando dejar de saltar y empezar a caer
            timeRemaining = 0.2f;

            //Establece parametros de animaciones correspondientes para hacer saber al animator que esta saltando
            //y ya no esta tocando el suelo
            animator.SetBool("isJumping", true);
            animator.SetBool("isGrounded", false);
        }

        //Si el jugador ha pulsado el boton, pero no esta tocando el suelo y no ha pausado la partida
        if (context.performed && !IsGrounded() && !pause)
        {
            //Si esta tocando una pared
            if (isTouchingWall())
            {
                //Realiza un impulso (Esto sirve para hacer saltos entre paredes)
                rb2D.AddForce(new Vector2(walljumpforce * walljumpAngle.x * walljumpDirection, walljumpforce * walljumpAngle.y), ForceMode2D.Impulse);
                Flip();
            }
        }

        //Si el jugador deja de pulsar el boton, su velocidad en el eje y es mayor de 0 (esta subiendo por el impulso del salto)
        // y no ha pausado la partida
        //(Esto significa que el jugador puede cancelar el salto, saltara mas alto si pulsa mas tiempo y saltara mas bajo
        //si pulsa menos tiempo)
        if (context.canceled && rb2D.velocity.y > 0f && !pause)
        {   
            //Reduce su velocidad a la mitad para que deje de subir tan rapido y caiga antes, la gravedad hace el resto
            rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y * 0.5f);
        }

    }

    //Funcion de input para cargar la habilidad magica del personaje
    public void Charge(InputAction.CallbackContext context)
    {
        //Si esta pulsando el boton
        if (context.performed)
        {
            //Establece que esta cargando (Para que el metodo del Update lo sepa)
            charge = true;
        }

        // Si el jugador deja de pulsar el boton
        if (context.canceled)
        {
            //Establece la carga a false 
            charge = false;

            //Si el contador era menor de 2.8 (Significa que aun no habia terminado 
            //de cargar cuando dejo de pulsar el boton)
            if (magicTimer < 2.8f)
            {
                //Para las animaciones y vuelve todo a la normalidad
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

    //Metodo para comprobar si el personaje esta tocando el suelo
    private bool IsGrounded()
    {
        //Devuelve true si hace contacto con el suelo
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    //Metodo para comprobar si el personaje esta tocando una pared
    bool isTouchingWall()
    {
        //Devuelve true si hace contacto con una pared
        return Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);
    }

    //Metodo para hacer que el personaje se de la vuelta
    private void Flip()
    {
        //Si el juego no esta pausado
        if (!pause)
        {
            //Establece la direccion de salto para cuando estamos tocando una pared 
            //(Porque saltamos al lado contrario de donde miramos)
            walljumpDirection *= -1;
            //Cambia la variable que nos indica si esta mirando a la derecha o no
            isFacingRight = !isFacingRight;
            //Obtiene la escala actual del personaje
            Vector3 localScale = transform.localScale;
            //Le da la vuelta
            localScale.x *= -1f;
            //La establece
            transform.localScale = localScale;
        }
    }

    //Funcion de input para interactuar en caso de poder hacerlo
    public void Interactuar() 
    { 
        //Si el personaje tiene con que interactuar y el juego no esta pausado
        if (interactuarActive && !pause)
        {
            SaveManager.SavePlayerData(this);
            datosGuardadosText.SetActive(true);
            statueSound.ReproducirSonido();
        }
    }

    //Funcion de input para mover al personaje
    public void Move(InputAction.CallbackContext context)
    {
        //Si el personaje esta vivo y la partida no esta pausada
        //Establece la direccion en horizontal como un vector 
        //(En caso de jugarse con teclado y raton, este vector sera constante segun el
        //lado al que queramos movernos, en caso de jugar con un mando o "Gamepad", podra
        //tener mayor o menor valor)
        if (isAlive && !pause)
        horizontal = context.ReadValue<Vector2>().x;
    }

    //Funcion de input para bloquear ataques
    public void Block(InputAction.CallbackContext context)
    {
        //Si el jugador esta pulsando el boton, si hay mas de 10 puntos de estamina, si el
        //personaje esta tocando el suelo y el juego no esta pausado
        if (context.performed && stamina >= 10 && IsGrounded() && !pause)
        {
            //Establece a true la variable que indica a otros metodos si el personaje esta bloqueando
            isBlocking = true;
            //Establece la velocidad del personaje a 0
            rb2D.velocity = new Vector2(0, 0);
            //Indica al animator que esta bloqueando
            animator.SetBool("Block",true);
        } 

        //Si el jugador deja de pulsar el boton
        if (context.canceled)
        {
            //Establece a false los parametros necesarios
            isBlocking = false;
            animator.SetBool("Block", false);
        }
    }

    //Funcion de input para usar la habilidad 'Dash'
    public void Dash(InputAction.CallbackContext context)
    {
        //Si el contador del 'dash' es menor o igual que 4.5 (Este contador va restando tiempo)
        //Si el jugador tiene mas de 32 puntos de stamina y si el juego no esta pausado
        if (dashCoolDownTimer <= 4.5 && stamina >=32 && !pause) {
            //Muestra el efecto de la habilidad
            trailEffect.Play();
            //Establece el bloqueo del jugador a false 
            isBlocking = false;

            //Si esta mirando a la derecha
            if (isFacingRight)
            {
                //Establece el color del jugador a negro para dar el efecto deseado
                spriteRenderer.color = Color.black;
                //Reproduce el sonido de la habilidad
                dashSound.ReproducirSonido();
                //Congela el movimiento del jugador en el eje y
                rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
                //Desactiva su caja de solisiones (Le permite atravesar enemigos)
                capsuleCollider.enabled = false;
                //Establece la habilidad a true
                dash = true;
                //Le da un impulso (Este impulso es el que hace que avancemos)
                rb2D.AddForce(transform.right * 15f, ForceMode2D.Impulse);
                //Nos quita 15 puntos de stamina
                stamina = stamina - 15;
                //Establece el contador de stamina con la que tenemos actualmente
                staminaBar.SetStamina(stamina);
            } 
            //Si esta mirando a la izquierda
            //Hace lo mismo pero mirando al lado contrario
            else
            {
                spriteRenderer.color = Color.black;
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

    //Metodo para hacer que el jugador pierda puntos de vida
    public void TakeDamage() 
    {
        //Si no esta bloqueando, si aun esta con vida y si el juego no esta pausado
        if (!isBlocking && isAlive && !pause)
        {
            //Resta 34 puntos de vida al usuario (Aproximadamente 1/3 de la vida del personaje)
            health = health - 34f;
            //Manda al animator que el personaje se hizo daño
            animator.SetTrigger("damage");
            //Reproduce el sonido del daño
            hitSound.ReproducirSonido();
            //Reduce la vida del contador de vida (La barra de vida)
            healthBar.SetHealth((int)Mathf.Round(health));
        } 
        //Si el jugador esta bloqueando 
        else if (isAlive)
        {   
            //Quita 20 puntos de stamina al jugador
            stamina = stamina - 20;
            //Establece los puntos de stamina en el contador (La barra de stamina)
            staminaBar.SetStamina(stamina);
            //Manda al animator que hemos bloqueado un ataque
            animator.SetTrigger("HitBlocked");
            //Reproduce el sonido de bloquear con el escudo
            escudoSonido();
            //Si el jugador esta bloqueando pero el personaje tiene menos de 20 puntos de stamina
            if (stamina < 20)
            {
                //Ya no bloquea
                isBlocking = false;
                //Establece las animaciones correspondientes
                animator.SetBool("Block", false);
                animator.ResetTrigger("HitBlocked");
            }
        }
    }

    //Metodo para matar al personaje
    public void Dead() {
        //Si esta con vida (ironicamente, porque solo puede morir una vez, entonces solo se ejecuta una vez xD)
        if (isAlive)
        {
            //Reproduce los sonidos de muerte correspondientes
            deadSound.ReproducirSonido();
            youDied.ReproducirSonido();
            //Establece el fondo de pantalla como negro
            img.color = Color.black;
            //Establece que el personaje ha muerto
            isAlive = false;
            //Activa el boton de reintentar
            reintentarButton.SetActive(true);
            //Manda al animator los parametros necesarios para mostrar las letras de "Has Muerto"
            deadTextAnimation.SetBool("Dead", true);
        }
    }

    //Metodo para cargar la habilidad magica
    private void magicCharge()
    {
        //Suma tiempo al contador de la habilidad
        magicTimer += Time.deltaTime;
        //Segun pasan las fracciones de segundo, va reproduciendo
        //animaciones (Activa la animacion de cada circulo magico,
        //reproduce el sonido, el humo, establece la iluminacion
        //en azul para que veamos que estamos usando la habilidad y finalmente, 
        //establece skill a true, que determina que estamos usando esta habilidad)
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

//-----------------------------------------------------------------------------------------------------------------------
//INICIO DE METODOS QUE SON UTILIZADOS PARA REPRODUCIR SONIDOS (LLAMADOS A LO LARGO DEL SCRIPT)
//-----------------------------------------------------------------------------------------------------------------------
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
//-----------------------------------------------------------------------------------------------------------------------
//FIN DE METODOS QUE SON UTILIZADOS PARA REPRODUCIR SONIDOS (LLAMADOS A LO LARGO DEL SCRIPT)
//-----------------------------------------------------------------------------------------------------------------------

    //Metodo para guardar los datos del jugador
    private void saveData()
    {
        SaveManager.SavePlayerData(this);
    }

    //Metodo para pausar la partida
    public void pausarJuego()
    {
        //Si no estaba pausado antes
        if (!pause)
        {
            //Establece la pausa
            pause = true;
            //Activa el menu de pausa
            pauseObject.SetActive(true);
            //Paraliza el tiempo
            Time.timeScale = 0;
        }
        //Si ya estaba pausado
        else
        {
            //Desactiva la pausa
            pause = false;
            //Esconde el menu de pausa
            pauseObject.SetActive(false);
            //Desaparaliza el tiempo
            Time.timeScale = 1;
        }
    }

    //Dibuja el area en donde se puede ver si el personaje detecta o no
    //que esta tocando la pared. Solo se puede ver desde el editor de Unity.
    //Creado con fines de Debugging.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(wallCheckPoint.position, wallCheckSize);
    }
}