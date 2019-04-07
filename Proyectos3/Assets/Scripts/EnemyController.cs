using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update

    public enum States { INITIAL, GOTO_BALL, GOTO_BASE, DIE }
    public float maxSpeed;
    private float currentSpeed;

    private float verticalSpeed;

    private bool canMove = true;

    private bool onGround;

    private CharacterController controller;

    private CollisionFlags collisionFlags;

    public GameObject target;

    void Start()
    {
        currentSpeed = maxSpeed;
        controller = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 l_Movement;

        l_Movement = target.transform.position - transform.position;

        l_Movement.Normalize();

        l_Movement *= Time.deltaTime * currentSpeed;

        //hasMovement
        if (l_Movement != Vector3.zero)
        {

            Quaternion newRotation = Quaternion.LookRotation(new Vector3(l_Movement.x, 0, l_Movement.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 4 * Time.deltaTime);
            //transform.forward = m_CameraController.transform.forward;
        }
      
        //GRAVITY
        if (onGround && verticalSpeed <= 0)
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
            collisionFlags = controller.Move(l_Movement);

        }

        if ((collisionFlags & CollisionFlags.Below) != 0)
        {
            onGround = true;
            verticalSpeed = 0.0f;
        }
        else
        {
            onGround = false;

        }

        if ((collisionFlags & CollisionFlags.Above) != 0 && verticalSpeed > 0.0f)
            verticalSpeed = 0.0f;
    }

    public void UpdateTarget(GameObject targ)
    {
        target = targ;
        PlayerController player = new PlayerController();
        
        //player.has
    }
}
