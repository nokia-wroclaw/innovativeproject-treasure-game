using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //player position values
    public Transform target;
    public float smoothing  = 5.0f;
    Vector3 offset;
    Vector3 targetCamPosition;
    public float xDistance = -8f;
    public float yDistance = 10f;
    public float zDistance = -5f;
    Vector3 temp;

    void Start()
    {             
        temp = target.position;   
        temp.x += xDistance;
        temp.y += yDistance;
        temp.z += zDistance;
        transform.position = temp;
        offset = transform.position - target.position;
        transform.LookAt(target.transform);
    }
    //NOTE: Desired camera rotation: (40, 50, 0)
    
    void FixedUpdate()
    {
        targetCamPosition = target.position + offset;
        transform.position = Vector3.Lerp (transform.position, targetCamPosition, smoothing*Time.deltaTime);
    }
}
