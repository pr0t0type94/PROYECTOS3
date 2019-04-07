using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Team1 : MonoBehaviour
{
    // Start is called before the first frame update
    Renderer rend;
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="ball")
        {
            rend.material.color = Color.red; 
        }
    }
}
