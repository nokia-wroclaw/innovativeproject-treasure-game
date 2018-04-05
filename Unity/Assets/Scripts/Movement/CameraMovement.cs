using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothing  = 5.0f;
    public float xDistance = -8f;
    public float yDistance = 10f;
    public float zDistance = -5f;

    private Vector3 temp;
    private Vector3 offset;
    private Vector3 targetCamPosition;

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
    
    void FixedUpdate()
    {
        targetCamPosition = target.position + offset;
        transform.position = Vector3.Lerp (transform.position, targetCamPosition, smoothing*Time.deltaTime);
    }
}
