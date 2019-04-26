using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  
    Programador:                    Murilo Bueno Julião Lemos
    Data da última atualização:     26/04/2019          
    Versão:                         0.6.3
    Empresa:                        NinGameOuver Studio
*/


public class Player_Movement : MonoBehaviour {

    private enum typeMovement                                   //Declaração de enum com os tipos de movimentos
    {
        Plataforma, TopDown, InfiniteRunner, ClickMove
    }                               

    delegate void func_chosedMovement();                        //Declarando um delegate, um tipo de "variável" para funções
    func_chosedMovement Movement;                               //"Variável" que guarda uma função do tipo func_chosedMovement

    void func_MenuMovement(typeMovement opcao)                  //Menu de escolha do tipo de movimento
    {
        switch(opcao)
        {
            case typeMovement.Plataforma:                       //Movimento do tipo: Plataforma
                {
                    Movement = func_MovePlataforma;
                    break;
                }
            case typeMovement.TopDown:                          //Movimento do tipo: TopDown
                {
                    Movement = func_MoveTopDown;
                    break;
                }
            case typeMovement.InfiniteRunner:                   //Movimento do tipo: InfiniteRunner
                {
                    Movement = func_MoveInfiniteRunner;
                    break;
                }
            case typeMovement.ClickMove:                        //Movimento do tipo: ClickMove
                {
                    Movement = func_MoveClickMove;
                    break;
                }
        }

    }               


    [SerializeField]                                                                 
    [Header("Player")]
    [Tooltip("Escolha o GameObject a ser contolado.\n" +
    "Choose the GameObject to be controlled.")]
    GameObject  player = null;                                  //Variável que recebe o player. OBS: se torna redundante quando o script é usado direto no player. My Bad :)

    [SerializeField]
    [Header("GroundCheck")]
    [Tooltip("Coloque aqui o GroundCheck GameObject.\n" +
    "Put here the GroundCheck GameObject.")]
    private Transform   groundCheck;                            //Variável que recebe o componente Transform do GameObject usado para verificar o chão.
    public  float       checkRadius;                            //Variável usada para definir o tamanho do raio de colisão no groundCheck.
    public  bool        isGround;                               //Variável que recebe a verificação de que o player está no chão.
    public  LayerMask   theGround;                              //Variável que recebe um tipo de layer. Usado para Layer do tipo ground neste caso.

    Rigidbody2D playerRB;

    [SerializeField]
    [Header("Tipo de Movimento")]
    typeMovement movementOption = typeMovement.Plataforma;      //Variável do tipo enum. Usado para receber o tipo de movimento escolhido. Está Plataforma como padrão.

    [SerializeField]
    [Header("Velocidade")]
    [Tooltip("Player's walk speed.\nVelocidade do jogador andando.")]
    float speedWalking;                                         //Velocidade do player andando.

    [SerializeField]
    [Tooltip("Velocidade do jogador correndo")]
    float speedRunning;                                         //Velocidade do player correndo


    public float speed;
    [Tooltip("Gravidade para o pulo")]
    public  float   gravity;                                    //Variável que define a gravity scale no rigidBody do player.
    private float   horizontal;                                 //Variável que recebe o valor do input "Horizontal".
    private float   vertical;                                   //Variável que recebe o valor do input "Vertical".
    public  float   forceJump;                                  //Variável que define um Vector2 com a força do pulo.
    private float   jumpTimeCounter;                            //Variável que define o tempo de acréscimo para aumentar o pulo.
    public  float   jumpTime;                                   //Variável que decresce ao com o Time.deltaTime para verificar se jumpTimeCounter é menor que 0.
    private bool    isJumping;                                  //Variável que recebe o valor true quando o personagem aperta o botão de pulo. ObS: Diferente de estar fora do chão.

    // Joystick Buttons**********************************************************************************
    /// <summary>
    ///                 Joystick Unity                      Joystick playstation
    ///                 
    ///                 joystick button 0                   Triângulo
    ///                 joystick button 1                   Círculo (Bolinha para os mais chegados!)
    ///                 joystick button 2                   Cruz    (Xis ou X)
    ///                 joystick button 3                   Quadrado
    ///                 
    /// </summary>
    string runButton = "joystick button 3";                     //Botão de correr.
    string jumpButton = "joystick button 2";                    //Botão de pular.
    string actionButton;
    string menuButton;
    string attackButton;

    //****************************************************************************************************


    // Use this for first of all
    private void Awake()
    {
        func_MenuMovement(movementOption);			//Chama a função func_MenuMovement e passa o 
    }								//enum movementOption para usar no switch

    // Use this for initialization
    void Start ()
    {
        if (player.tag == "Player")
        {
            playerRB = player.GetComponent<Rigidbody2D>();	//Pega o componente Rigidbody2D do player
        }
        playerRB.gravityScale = gravity;                        //Dar valor para a gravidade
    }

    private void Update()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, theGround);	//Testa se um círculo com a posição do groundCheck, com 
												//raio checkRadius colide com a layer mask theGround
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void func_movement_horizontal(float horizontal)
	{
        if (Input.GetKey("left shift") || Input.GetKey(runButton)) speed = speedRunning;
        else if (!Input.GetKey("left shift") || Input.GetKey(runButton)) speed = speedWalking;
        if (horizontal != 0.0f && vertical != 0.0f) speed = speed / 1.3f;
        playerRB.velocity = new Vector2(speed * horizontal, playerRB.velocity.y);
        if (horizontal > 0) transform.eulerAngles = new Vector3(0, 0, 0);                       //Vira o Sprite para direita
        else if (horizontal < 0) transform.eulerAngles = new Vector3(0, 180, 0);                //Vira o Sprite para esquerda
	}
       
    void func_movement_vertical(float vertical)
    {
        if (Input.GetKey("left shift") || Input.GetKey(runButton)) speed = speedRunning;
        else if (!Input.GetKey("left shift") || Input.GetKey(runButton)) speed = speedWalking;
        if (horizontal != 0.0f && vertical != 0.0f) speed = speed / 1.3f;
        playerRB.velocity = new Vector2(playerRB.velocity.x, speed * vertical);
    }

    void func_jump()
    {
        //if (Input.GetKeyDown(jumpButton)) print("pulei eu acho");    Usado para constatar que o problema de não pular as vezes, era no botão do joystick
        if (Input.GetKeyDown(jumpButton) && isGround)
        {
            //print("pulei");
            isJumping = true;
            playerRB.velocity = new Vector2(playerRB.velocity.x, 1 * forceJump);
            jumpTimeCounter = jumpTime;
        }

        if (Input.GetKey(jumpButton) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, 1 * forceJump);
                jumpTimeCounter -= Time.deltaTime;
            }
         
        }
        else
        {
            isJumping = false;
        }

        if (Input.GetKeyUp(jumpButton))
        {
            isJumping = false;
        }
    }

    void func_MovePlataforma()
    {
        horizontal = Input.GetAxis("Horizontal");
        func_movement_horizontal(horizontal);
        func_jump();

    }

    void func_MoveTopDown()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        func_movement_vertical(vertical);
        func_movement_horizontal(horizontal);
    }


    void func_MoveInfiniteRunner()
    {

    }

    void func_MoveClickMove()
    {

    }
/*
    private void OnTriggerEnter2D(Collider2D obj)
    {
        print(obj);
        if( obj.gameObject.layer == 8)
        {
            print("Estou colidindo");
            isGround = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 8)
        {
            print("Sai do trigger");
            isGround = false;
        }
    }
*/



}





