using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Movem2 : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 5f;
    private float verticalSpeed = -10f;
    private CharacterController controller;
    CollisionFlags l_CollisionFlags;

    public Ball ball;
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement.y = verticalSpeed * Time.deltaTime;



        movement.Normalize();

        movement *= Time.deltaTime * speed;

        l_CollisionFlags = controller.Move(movement);

        if ((l_CollisionFlags & CollisionFlags.Below)!=0)
        {
            verticalSpeed = 0.0f;

        }

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && verticalSpeed > 0.0f)
            verticalSpeed = 0.0f;

        //transform.forward = Camera.main.transform.forward;
        //transform.LookAt(ball.transform);


    }
}
