using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController playerCC;

    public KeyCode UpKeyCode;
    public KeyCode DownKeyCode;
    public KeyCode LeftKeyCode;
    public KeyCode RightKeyCode;
    public KeyCode RunKeyCode;
    public KeyCode PassKeyCode;
    public KeyCode JumpKeyCode;

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

    //public GameController gameControler;


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

    private bool hasBall;

    public Camera_Controller noBallCC;
    public Camera_Controller_3rdPerson hasBallCC;

    private Rigidbody ballRb;


    float speedX = 600f;
    float speedY = 400f;

    bool startPushEnemyTimer;
    float pushHitTimer = 2f;
    float pushSpeed=5f;
    public CharacterController enemyCC;

    public Camera cam;

    // Use this for initialization
    void Start()
    {
        playerCC = gameObject.GetComponent<CharacterController>();
        iniSpeed = l_Speed;
        canMove = true;
        respawnPosition = transform.position;
        
        dashSpeed = l_Speed * 3;
        
        //hasTheBall = false;
        totalDashes = 3;
        

        rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.blue);

        ball = GameObject.FindGameObjectWithTag("ball");
        hasBall = false;

        noBallCC.enabled = true;
        hasBallCC.enabled = false;

        ballRb = ball.GetComponent<Rigidbody>();
        startPushEnemyTimer = false;

    }

    // Update is called once per frame
    void Update()
    {



        Vector3 l_Movement = Vector3.zero;
        Vector3 l_Forward = cam.transform.forward;
        Vector3 l_Right = cam.transform.right;
        l_Forward.y = 0.0f;
        l_Forward.Normalize();
        l_Right.y = 0.0f;
        l_Right.Normalize();
        if (Input.GetKey(UpKeyCode))
            l_Movement = l_Forward;
        else if (Input.GetKey(DownKeyCode))
            l_Movement = -l_Forward;
        if (Input.GetKey(LeftKeyCode))
            l_Movement = -l_Right;
        else if (Input.GetKey(RightKeyCode))
            l_Movement = l_Right;

        /////////////
        ///

        if (hasBall)
        {
            hasBallCC.enabled = true;
            noBallCC.enabled = false;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 direction = Camera.main.transform.forward;
                ball.transform.parent = null;
                ballRb.isKinematic = false;
                ballRb.AddForce(new Vector3(speedX *transform.forward.normalized.x, speedY, 0.0f));
                hasBall = false;
            }

        }
        else
        {
            hasBallCC.enabled = false;
            noBallCC.enabled = true;
        }

        //transform.LookAt(transform.forward);    


        if(Input.GetKey(JumpKeyCode))
        {
            verticalSpeed = JumpSpeed;
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
        

     
        /////LONGJUMP
        //if (startLongJumpTimer)
        //{
        //    longJumpTimer -= Time.deltaTime;

        //    if (longJumpTimer <= 0)
        //    {
        //        startLongJumpTimer = false;
        //        longJumpTimer = 1f;
        //        canMove = true;
        //    }
        //    else
        //    {
        //        canMove = false;
        //        verticalSpeed = JumpSpeed / 6;
        //        playerCC.Move(transform.forward * longJumpImpulseForce * Time.deltaTime);
        //        playerCC.Move(transform.up * verticalSpeed * Time.deltaTime);

        //    }


        //}


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
            verticalSpeed = -playerCC.stepOffset / Time.deltaTime;
        }
        else
        {
            verticalSpeed += Physics.gravity.y * Time.deltaTime;
        }


        l_Movement.y = verticalSpeed * Time.deltaTime;

        //CollisionFlags + controller Move
        if (canMove)
        {
            l_CollisionFlags = playerCC.Move(l_Movement);

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


        if (startPushEnemyTimer)
        {
            pushHitTimer -= Time.deltaTime;
            if (pushHitTimer > 0)
            {

                pushEnemy();
            }
            else
            {
                startPushEnemyTimer = false;
                pushHitTimer = 1f;
            }
        }
        else
        {

        }

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    PlayerController pc = other.gameObject.GetComponent<PlayerController>();
    //    if (other.gameObject.tag == "Player")
    //    {
    //        startPushEnemyTimer = true;
    //        if (hasBall)
    //        {
    //            ball.transform.parent = null;
 
    //                ball.GetComponent<Rigidbody>().isKinematic = false;
    //                ball.GetComponent<Rigidbody>().AddForce(Vector3.up * 200f);
    //            hasBall = false;
    //        }
    //    }
    //}
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //PlayerController pc = hit.gameObject.GetComponent<PlayerController>();
        //if (hit.gameObject.tag=="Player")
        //{
        //    startPushEnemyTimer = true;
        //    if(hasBall)
        //    {
        //        ball.transform.parent = null;
        //        hasBall = false;
        //        if (!hasBall)
        //        {
        //            ball.GetComponent<Rigidbody>().isKinematic = false;
        //            ball.GetComponent<Rigidbody>().AddForce(Vector3.up * 40f);

        //        }
        //    }
        //}

        //if(hit.gameObject.tag == "Player" && pc.hasBall)
        //{
        //    ball.transform.parent = null;
        //    hasBall = false;
        //    if (!hasBall)
        //    {
        //    ball.GetComponent<Rigidbody>().isKinematic = false;
        //    ball.GetComponent<Rigidbody>().AddForce(Vector3.up * 40f);

        //    }

        //}
        PlayerController pc = hit.gameObject.GetComponent<PlayerController>();
        if (hit.gameObject.tag == "Player")
        {
            startPushEnemyTimer = true;
            if (hasBall)
            {
                ball.transform.parent = null;

                ball.GetComponent<Rigidbody>().isKinematic = false;
                ball.GetComponent<Rigidbody>().AddForce(Vector3.up * 200f);
                hasBall = false;
            }
        }

        if (hit.gameObject.tag == "ball")
        {
            hasBall = true;
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


    public void pushEnemy()
    {

        enemyCC.Move(transform.forward * pushSpeed * Time.deltaTime);


    }





} 


