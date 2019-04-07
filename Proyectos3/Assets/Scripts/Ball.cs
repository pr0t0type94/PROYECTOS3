using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;

    private bool attached;
    private bool player1HastheBall;
    private bool player2HastheBall;
    public float impulseForceClash;
    private Vector3 positionToAttach;

    void Start()
    {
        impulseForceClash = 800f;
        rb = GetComponent<Rigidbody>();
        attached = false;
        positionToAttach = new Vector3(0, 1, 1);

      
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            rb.AddForce(Vector3.up * impulseForceClash);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.tag =="PLAYER2") && !attached)
        {
            attachToObject(other.gameObject);
            player2HastheBall = true;
        }
        if ((other.gameObject.tag == "Player") && !attached)
        {
            attachToObject(other.gameObject);
            player1HastheBall = true;
        }


        if ((other.gameObject.tag == "PLAYER2") && attached && player1HastheBall)
        {
            dettachFromObject();
            rb.AddForce(Vector3.up * impulseForceClash);
            player1HastheBall = false;
        }

        if ((other.gameObject.tag == "Player") && attached && player2HastheBall)
        {
            dettachFromObject();
            rb.AddForce(Vector3.up * impulseForceClash);
            player2HastheBall = false;
        }


    }

    private void attachToObject(GameObject targ)
    {
        attached = !attached;
        
        transform.parent = targ.transform;
        transform.position = positionToAttach;
        rb.isKinematic = true;
        //targ.GetComponent<PlayerController>().hasTheBall(true);
        
        
    }
    public void dettachFromObject()
    {
        attached = !attached;
        transform.parent = null;
        rb.isKinematic = false;
    }
    public void addForce(Vector3 direction)
    {
        rb.AddForce(direction);
    }
   
}
