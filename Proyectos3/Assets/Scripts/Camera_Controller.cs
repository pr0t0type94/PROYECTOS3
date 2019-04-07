using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform playerCamRig;
    public GameObject player;
    public Transform ballCamRig;
    public string target;
    public GameObject ball;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BallCam();
       
    }

    void UpdateCamPosAndRot(GameObject target)
    {
        transform.position = playerCamRig.position;
        transform.LookAt(target.transform);
    }

    void PlayerCam()
    {
        transform.position = playerCamRig.position;
        transform.LookAt(player.transform);
    }
    void BallCam()
    {
        transform.position = ballCamRig.position;
        transform.LookAt(ball.transform);
    }
}
