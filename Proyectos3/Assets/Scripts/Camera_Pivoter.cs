using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Pivoter : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ball;

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        transform.LookAt(ball.transform.position);
    }
}
