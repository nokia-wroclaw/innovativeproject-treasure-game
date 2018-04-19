using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float smoothing  = 5.0f;
    public float xDistance = -8f;
    public float yDistance = 10f;
    public float zDistance = -5f;

    private GameObject _player;
    private Vector3 _temp;
    private Vector3 _offset;
    private Vector3 _targetCamPosition;
    private Vector3 _rayDirection;
    private RaycastHit _hit;

    void Start()
    { 
        _player = GameObject.FindGameObjectWithTag("Player");
        _temp = _player.transform.position;   
        _temp.x += xDistance;
        _temp.y += yDistance;
        _temp.z += zDistance;
        transform.position = _temp;
        _offset = transform.position - _player.transform.position;
        transform.LookAt(_player.transform);
    }
    
    void FixedUpdate()
    {
        _targetCamPosition = _player.transform.position + _offset;
        transform.position = Vector3.Lerp (transform.position, _targetCamPosition, smoothing*Time.deltaTime);
        //CheckCollisions();
    }

    private void CheckCollisions()
    {
        _rayDirection = _player.transform.position - transform.position + new Vector3(0, 1, 0);
        Debug.DrawRay(transform.position, _rayDirection, Color.red);

        if (Physics.Raycast(transform.position, _rayDirection, out _hit))
        {
            if (!_hit.transform.CompareTag("Player") && !_hit.transform.CompareTag("Canvas"))
            {
                //TODO:
                //Change material alpha value 
                _hit.transform.GetComponent<Renderer>().enabled = false;
                Debug.LogFormat("sth blocks the view {0}", _hit.transform.tag);   
            }
        }
  
    }
}
