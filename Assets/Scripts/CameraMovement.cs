using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //player position values
    public Transform target;
    public float xDistance = -8f;
    public float yDistance = 10f;
    public float zDistance = -5f;

    //NOTE: Desired camera rotation: (40, 50, 0)

    void LateUpdate()
    {
        var temp = target.position;
        temp.x += xDistance;
        temp.y += yDistance;
        temp.z += zDistance;
        transform.position = temp;
    }
}