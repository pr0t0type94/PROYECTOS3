using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GlobalStats
{
    private CharacterController controller;

    public KeyCode m_UpKeyCode;
    public KeyCode m_DownKeyCode;
    public KeyCode LeftKeyCode;
    public KeyCode RightKeyCode;
    public KeyCode RunKeyCode;
    public KeyCode PassKeyCode;

    public float l_Speed;
    public float m_RunSpeed;
    private float iniSpeed;

    public float JumpSpeed;

    private float verticalSpeed;

    public bool OnGround;

    private GameObject m_CurrentPlatform;


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


    public Transform m_AttachingPosition;
    public bool m_AttachedObject;
    private Rigidbody m_ObjectAttached;
    public float m_AttachingObjectSpeed;
    private Quaternion m_AttachingObjectStartRotation;
    private bool m_AttachingObject;
    public float impulseForce = 50f;

    //public RestartGame resetController;

    private bool onLava;

    public Vector3 respawnPosition;

    // Use this for initialization
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        iniSpeed = l_Speed;
        canMove = true;
        respawnPosition = transform.position;
        //hasTheBall = false;
        
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
        if (m_AttachedObject)
        {
            canAttack = false;
            m_ObjectAttached.isKinematic = true;
            m_ObjectAttached.GetComponent<SphereCollider>().enabled = false;
            l_Speed /= 2;
        }
   
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
        
    }
}
