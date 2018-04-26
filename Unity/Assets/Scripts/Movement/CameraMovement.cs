using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private class CollidedObject
    {
        public GameObject gameObject;
        public short collisionCounter;
    }

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
        StartCoroutine(CheckCollisions());
    }
    
    void FixedUpdate()
    {
        _targetCamPosition = _player.transform.position + _offset;
        transform.position = Vector3.Lerp (transform.position, _targetCamPosition, smoothing*Time.deltaTime);
    }

    private IEnumerator CheckCollisions()
    {
        var collidedObjects = new List<CollidedObject>();

        while(true)
        {
            Debug.Log(collidedObjects.Count);
            _rayDirection = _player.transform.position - transform.position + new Vector3(0, 1, 0);
            Debug.DrawRay(transform.position, _rayDirection, Color.yellow);

            collidedObjects.ForEach(c => c.collisionCounter--);

            if (Physics.Raycast(transform.position, _rayDirection, out _hit))
            {
                if (_hit.transform.CompareTag("Obstacle"))
                {
                    var collidedObject = collidedObjects.FirstOrDefault(g => ReferenceEquals(g.gameObject, _hit.transform.gameObject));
                    if (collidedObject == null)
                    {
                        _hit.transform.gameObject.GetComponent<Renderer>().material.ChangeAlpha(0.5f);

                        var collisionSource = new CollidedObject { gameObject = _hit.transform.gameObject, collisionCounter = 2 };
                        collidedObjects.Add(collisionSource);
                    }
                    else
                        collidedObject.collisionCounter++;
                }
            }

            collidedObjects.RemoveAll(c => {
                if (c.collisionCounter == 0)
                {
                    c.gameObject.GetComponent<Renderer>().material.ChangeAlpha(1f);
                    return true;
                }
                else
                    return false;
            });

            yield return new WaitForSeconds(0.1f);
        }


    }
}
