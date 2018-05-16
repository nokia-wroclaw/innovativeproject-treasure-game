using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class PlayerVisibility : MonoBehaviour
{   
    public bool canAlarm = false;
    public bool respond = true;
    public float fieldOfViewDegrees;
    public float visibilityDistance;
    public float endGameDistance;
    public bool stunned = false;
    private NavMeshAgent _agent;
    private List<GameObject> _guardsList = new List<GameObject>();

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
        _guardsList = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        _agent = GetComponent<NavMeshAgent>();
        _playerSpottedObject = Instantiate((GameObject)Resources.Load("Prefabs/ExclamationMark"));
       // _playerSpottedObject.transform.localPosition = gameObject.transform.position + new Vector3(0, 4, 0);
        //_playerSpottedObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _playerSpottedObject.GetComponent<Renderer>().enabled = false;
        StartCoroutine(PlayerVisibilityCheck());
    }

    private void Update()
    {
        _playerSpottedObject.transform.position = gameObject.transform.position + new Vector3(0, 4, 0);
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
                        if (hit.distance - _agent.baseOffset <= endGameDistance && !canAlarm)
                            break;

                        if (hit.distance - _agent.baseOffset <= visibilityDistance)
                        {
                            Chasing = true;
                            if(canAlarm && respond)
                            {
                                var guard = GetNearestGuard();
                                var patroller = guard.GetComponent<Patroller>();
                                //Debug.Log("Calling " + guard.name);
                                StartCoroutine(patroller.Alarm(transform.position, this));                                
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
        SwitchScene();
    }

    private GameObject GetNearestGuard()
    {
        var guard = _guardsList[0];
        var distance = Vector3.Distance(transform.position, guard.transform.position);
        foreach(var tempGuard in _guardsList)
        {
            var tempDistance = Vector3.Distance(transform.position, tempGuard.transform.position); 
            if(tempDistance < distance)
            {
                distance = tempDistance;
                guard = tempGuard;
            }
        }
        return guard;
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

