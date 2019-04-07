using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller_3rdPerson : MonoBehaviour
{

    public static float m_Yaw;
    float m_Pitch;
    public float m_YawRotationalSpeed;
    public float m_PitchRotationalSpeed;
    public float m_MinPitch;
    public float m_MaxPitch;

    public float m_DistanceToLookAt;
    public float m_OffsetOnCollision;

    public LayerMask m_RaycastLayerMask;
    public bool m_AngleLocked = false;
    public Transform m_LookAt;

    private bool startTimer = false;
    private float timer = 5f;
    private bool repositionCamera = false;
    // Use this for initialization
    void Start()
    {
        startTimer = true;
    }

    // Update is called once per frame
    void Update()
    {
        //..
        Vector3 l_Direction = m_LookAt.position - transform.position;
        float l_Distance = l_Direction.magnitude;
        float l_MouseAxisX = Input.GetAxis("Mouse X");
        float l_MouseAxisY = Input.GetAxis("Mouse Y");

        //Debug.Log("rs "+l_MouseAxisX+" "+l_MouseAxisY);

        //..
        Vector3 l_DesiredPosition = transform.position;

        if (!m_AngleLocked && (l_MouseAxisX > 0.01f || l_MouseAxisX < -0.01f || l_MouseAxisY > 0.01f || l_MouseAxisY < -0.01f))
        {
            Vector3 l_EulerAngles = transform.eulerAngles;
            float l_Yaw = (l_EulerAngles.y + 180.0f);
            float l_Pitch = l_EulerAngles.x;

            l_Yaw += m_YawRotationalSpeed * l_MouseAxisX * Time.deltaTime;
            l_Yaw *= Mathf.Deg2Rad;
            if (l_Pitch > 180.0f)
                l_Pitch -= 360.0f;
            l_Pitch += m_PitchRotationalSpeed * (-l_MouseAxisY) * Time.deltaTime;
            l_Pitch = Mathf.Clamp(l_Pitch, m_MinPitch, m_MaxPitch);
            l_Pitch *= Mathf.Deg2Rad;
            l_DesiredPosition = m_LookAt.transform.position + new Vector3(Mathf.Sin(l_Yaw) * Mathf.Cos(l_Pitch) * l_Distance, Mathf.Sin(l_Pitch) * l_Distance, Mathf.Cos(l_Yaw) * Mathf.Cos(l_Pitch) * l_Distance);
            l_Direction = (m_LookAt.position - l_DesiredPosition);
        }
        l_Direction /= l_Distance;

        if (l_Distance > m_DistanceToLookAt)
        {
            l_DesiredPosition = m_LookAt.position - l_Direction * m_DistanceToLookAt;
            l_Distance = m_DistanceToLookAt;
        }

        RaycastHit l_RaycastHit;
        Ray l_Ray = new Ray(m_LookAt.transform.position, -l_Direction);
        if (Physics.Raycast(l_Ray, out l_RaycastHit, l_Distance, m_RaycastLayerMask.value))
            l_DesiredPosition = l_RaycastHit.point + l_Direction * m_OffsetOnCollision; 

        transform.forward = l_Direction;

        transform.position = l_DesiredPosition;

    }
}

