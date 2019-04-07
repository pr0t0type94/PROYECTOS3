using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : GlobalStats
{
    private CharacterController controller;

    public KeyCode m_UpKeyCode;
    public KeyCode m_DownKeyCode;
    public KeyCode LeftKeyCode;
    public KeyCode RightKeyCode;
    public KeyCode RunKeyCode;
    public KeyCode PassKeyCode;

    private GameObject ball;




    public float l_Speed;
    public float m_RunSpeed;
    private float iniSpeed;

    public float JumpSpeed;

    private float verticalSpeed;

    public bool OnGround;

    private GameObject m_CurrentPlatform;


    //For DASH
    public Vector3 moveDirection;
    public float maxDashTime;
    private float dashSpeed;
    private float currentDashTime;
    public float  dashReloadmaxTime;
    private float currentDashReloadTime;
    public Text dash1;
    public Text dash2;
    public Text dash3;
    private int totalDashes = 3;
    private bool DashingBool = false;
    private bool DashReloadBool = false;
    private bool dashCooldownBool;
    public float dashCooldownTime;
    private float currentDashCooldown;
    //


    

    public float jumpTimer = 3f;
    private bool startjumpTimer;
    public float doubleJumpTimer = 4f;
    private bool startDoubleJumpTimer;

    //public GameController gameControler;

    private bool startLongJumpTimer;
    public float longJumpTimer;
    public float longJumpImpulseForce;
    private bool onWall = false;
    private bool startWallTimer;
    public float wallTimer = 1f;

    public bool canMove;
    public bool canAttack;

    CollisionFlags l_CollisionFlags;


    public GameObject m_AttachingPosition;
    public bool m_AttachedObject;
    private Rigidbody m_ObjectAttached;
    public float m_AttachingObjectSpeed;
    private Quaternion m_AttachingObjectStartRotation;
    private bool m_AttachingObject;
    public float impulseForce = 50f;

    //public RestartGame resetController;

    private bool onLava;

    public Vector3 respawnPosition;

    Renderer rend;


    // Use this for initialization
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        iniSpeed = l_Speed;
        canMove = true;
        respawnPosition = transform.position;
        
        dashSpeed = l_Speed * 3;
        
        //hasTheBall = false;
        totalDashes = 3;
        

        rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.blue);




    }

    // Update is called once per frame
    void Update()
    {

        Vector3 l_Movement = Vector3.zero;
        Vector3 l_Forward = Camera.main.transform.forward;
        Vector3 l_Right = Camera.main.transform.right;
        l_Forward.y = 0.0f;
        l_Forward.Normalize();
        l_Right.y = 0.0f;
        l_Right.Normalize();
        if (Input.GetKey(m_UpKeyCode))
            l_Movement = l_Forward;
        else if (Input.GetKey(m_DownKeyCode))
            l_Movement = -l_Forward;
        if (Input.GetKey(LeftKeyCode))
            l_Movement = -l_Right;
        else if (Input.GetKey(RightKeyCode))
            l_Movement = l_Right;

        /////////////
        ///
      

        //transform.LookAt(transform.forward);    

        if (startjumpTimer)
        {

            jumpTimer -= Time.deltaTime;

            if (jumpTimer <= 0)
            {
                startjumpTimer = false;
                jumpTimer = 1.5f;
            }
            else if (jumpTimer <= 0.9f && jumpTimer >= 0)
            {
                if (OnGround && Input.GetKeyDown(KeyCode.Space))
                {
                    verticalSpeed = JumpSpeed * 1.5f;
                    startDoubleJumpTimer = true;
                }
            }
        }


        //RUN
        if (OnGround && Input.GetKey(RunKeyCode))
        {
            l_Speed = m_RunSpeed;

        }
        else
        {
            l_Speed = iniSpeed;
        }


        //DASH
        performDash();
        updateDashText();
        //

     
        


        ///LONGJUMP
        if (startLongJumpTimer)
        {
            longJumpTimer -= Time.deltaTime;

            if (longJumpTimer <= 0)
            {
                startLongJumpTimer = false;
                longJumpTimer = 1f;
                canMove = true;
            }
            else
            {
                canMove = false;
                verticalSpeed = JumpSpeed / 6;
                controller.Move(transform.forward * longJumpImpulseForce * Time.deltaTime);
                controller.Move(transform.up * verticalSpeed * Time.deltaTime);

            }


        }


        //MOVEMENT

        l_Movement.Normalize();

        l_Movement *= Time.deltaTime * l_Speed;

        //hasMovement
        if (l_Movement != Vector3.zero)
        {

            Quaternion newRotation = Quaternion.LookRotation(new Vector3(l_Movement.x, 0, l_Movement.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 4 * Time.deltaTime);
        }

        //GRAVITY
        if (OnGround && verticalSpeed <= 0)
        {
            verticalSpeed = -controller.stepOffset / Time.deltaTime;
        }
        else
        {
            verticalSpeed += Physics.gravity.y * Time.deltaTime;
        }


        l_Movement.y = verticalSpeed * Time.deltaTime;

        //CollisionFlags + controller Move
        if (canMove)
        {
            l_CollisionFlags = controller.Move(l_Movement);

        }

        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            OnGround = true;
            verticalSpeed = 0.0f;
        }
        else
        {
            OnGround = false;

        }

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && verticalSpeed > 0.0f)
            verticalSpeed = 0.0f;

      

    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "PLAYER2" && ball)
        {
            ball.transform.parent = null;

            ball.GetComponent<Rigidbody>().isKinematic = false;
            ball.GetComponent<Rigidbody>().AddForce(Vector3.up * 700f);
            ball = null;

        }

        if(hit.gameObject.tag == "ball")
        {
            ball = hit.gameObject;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.transform.position = m_AttachingPosition.transform.position;
            ball.transform.parent = gameObject.transform;


        }
    }

    

    void performDash()
    {


        if (totalDashes != 0)
        {




            if (Input.GetKeyDown(KeyCode.Z) && dashCooldownBool == false)
            {
                currentDashTime = 0.0f;
                currentDashCooldown = 0.0f;

                totalDashes -= 1;
                DashingBool = true;



                dashCooldownBool = true;

            }
        }

            if (dashCooldownBool)
            {


                currentDashCooldown += Time.deltaTime;
                if (currentDashCooldown >= dashCooldownTime)
                {
                    dashCooldownBool = false;
                }
            }


            if (DashingBool)
            {

                currentDashTime += Time.deltaTime;

                rend.material.SetColor("_Color", Color.green);
                l_Speed = dashSpeed;

                if (currentDashTime >= maxDashTime)
                {

                    
                    DashingBool = false;


                }

            }

        



        if (DashingBool == false)
        {
            rend.material.SetColor("_Color", Color.blue);
            l_Speed = iniSpeed;
        }



        //DASH RELOAD

        if (totalDashes != 3)
        {
            




            currentDashReloadTime += Time.deltaTime;


            if (currentDashReloadTime >= dashReloadmaxTime)
            {
                totalDashes += 1;

                currentDashReloadTime = 0.0f;

            }
        }
        




    }



    private void updateDashText()
    {
        if (totalDashes == 3)
        {
            dash3.enabled = true;
            dash2.enabled = true;
            dash1.enabled = true;
        }

        else if (totalDashes == 2)
        {
            dash3.enabled = false;
            dash2.enabled = true;
            dash1.enabled = true;
        }
        else if (totalDashes == 1)
        {
            dash3.enabled = false;
            dash2.enabled = false;
            dash1.enabled = true;
        }
        else if (totalDashes == 0)
        {
            dash3.enabled = false;
            dash2.enabled = false;
            dash1.enabled = false;
        }

    }






} 


