﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class Patroller : MonoBehaviour 
{
    public bool usePredefinedPatrolTargets;
    public Vector3[] patrolTargets;

    private NavMeshAgent _agent;
    private Animator _anim;
    private GameObject _player;
    private PlayerVisibility _playerVisibility;

    private int destPoint;
    private bool arrived;
    private bool patrolling;

	void Start () 
	{
		_agent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
        _playerVisibility = GetComponent<PlayerVisibility>();
        _player = GameObject.FindGameObjectWithTag("Player");
        if (!usePredefinedPatrolTargets)
            patrolTargets = GeneratePatrolTargets();
    }
	
	void FixedUpdate () 
	{
        if (_agent.pathPending)
            return;

        if (patrolling)
        {
            if (_playerVisibility.Chasing)
            {
                _agent.destination = _player.transform.localPosition;
            }
            else
            {
                if (_agent.remainingDistance < _agent.stoppingDistance)
                {
                    if (!arrived)
                    {
                        arrived = true;
                        StartCoroutine(GoToNextPoint());
                    }
                }
                else
                    arrived = false;
            }       
        }
        else
            StartCoroutine(GoToNextPoint());

        _anim.SetFloat("blendSpeed", _agent.velocity.sqrMagnitude);
	}
	
    private Vector3[] GeneratePatrolTargets()
    {
        RaycastHit hit;
        var patrolTargets = new Vector3[4];
        var random = new Random();
        var mapSize = Terrain.activeTerrain.terrainData.size;
        var counter = 0;
        for (int i = 0; i < 4; i++)
        {
            Vector3 point;
            bool found = false;

            do
            {
                point = new Vector3(((float)random.Next(0, 101) / 100) * mapSize.x,
                                    1,
                                    ((float)random.Next(0, 101) / 100) * mapSize.z);
                
                if (Physics.Raycast(new Vector3(point.x, point.y + 10, point.z), Vector3.down, out hit))
                {
                    if (hit.transform.CompareTag("Terrain"))
                    {
                        found = true;
                    }
                }
                counter++;
                if(counter > 10000)
                {
                    Debug.LogError("Too many tries to generate random patrol points");
                    break;
                }
            } while (!found);
            
            patrolTargets[i] = point;
            Debug.LogFormat("Point {0}: {1}", i, point);
        }
        Debug.LogFormat("Number of hits: {0}", counter);

        return patrolTargets;
    }

	private IEnumerator GoToNextPoint()
	{
        if (patrolTargets.Length == 0)
        {
            yield break;
        }
        patrolling = true;
        yield return new WaitForSeconds(2.0f);
        arrived = false;
        _agent.destination = patrolTargets[destPoint];
        destPoint = (destPoint + 1) % patrolTargets.Length;        
    }

    private IEnumerator Stun()
    {
        Debug.Log("Stunned");
        _agent.speed = 0;
        _playerVisibility.stunned = true;
        yield return new WaitForSeconds(10.0f);
        _playerVisibility.stunned = false;
        _agent.speed = 2;
    }

    public void StunEnemy()
    {
        StartCoroutine(Stun());
    }

}