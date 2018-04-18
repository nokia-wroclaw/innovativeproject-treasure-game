using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVisibility : MonoBehaviour
{   
    public float fieldOfViewDegrees;
    public float visibilityDistance;
    public float endGameDistance;
    public bool stunned = false;

    public bool Chasing
    {
        get
        {
            return _chasing;
        }
        private set
        {
            _chasing = value;

            if (_chasing && !_playerSpottedObject.GetComponent<Renderer>().enabled)
            {
                _playerSpottedObject.GetComponent<Renderer>().enabled = true;
            }

            if (!_chasing && _playerSpottedObject.GetComponent<Renderer>().enabled)
            {
                _playerSpottedObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }
  
    public GameObject Player { get; private set; }

    private GameObject _playerSpottedObject;

    private bool _chasing;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        _playerSpottedObject = Instantiate((GameObject)Resources.Load("Prefabs/ExclamationMark"));
       // _playerSpottedObject.transform.localPosition = gameObject.transform.position + new Vector3(0, 4, 0);
        //_playerSpottedObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _playerSpottedObject.GetComponent<Renderer>().enabled = false;
        StartCoroutine(PlayerVisibilityCheck());
    }

    private void FixedUpdate()
    {
        _playerSpottedObject.transform.localPosition = gameObject.transform.position + new Vector3(0, 4, 0);
    }

    private IEnumerator PlayerVisibilityCheck()
    {
        RaycastHit hit;
        Vector3 rayDirection;

        while (true)
        {
            rayDirection = Player.transform.position - transform.position;
            Debug.DrawRay(transform.position, rayDirection, Color.red);

            Chasing = false;

            if (!stunned && Vector3.Angle(rayDirection, transform.forward) <= fieldOfViewDegrees * 0.5f)
            {
                if (Physics.Raycast(transform.position, rayDirection, out hit))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        if (hit.distance <= endGameDistance)
                            break;

                        if (hit.distance <= visibilityDistance)
                            Chasing = true;
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
        SwitchScene();
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

