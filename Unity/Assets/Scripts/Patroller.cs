using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patroller : MonoBehaviour 
{
	NavMeshAgent agent;
	bool patrolling;
	public Transform[] patrolTargets;
	private int destPoint;
	bool arrived;
	private Animator anim;
	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (agent.pathPending)
		{
			Debug.Log("PathPending");
			return;
		}
		if(patrolling)
		{
			if(agent.remainingDistance < agent.stoppingDistance)
			{
				if(!arrived)
				{
					arrived = true;
					StartCoroutine("GoToNextPoint");
				}
			}
			else
			{
				arrived = false;
			}
		}
		else
		{
			StartCoroutine("GoToNextPoint");
		}
		anim.SetFloat("blendSpeed", agent.velocity.sqrMagnitude);
	}
	
	IEnumerator GoToNextPoint()
	{
		if(patrolTargets.Length == 0)
		{
			yield break;
		}
		patrolling = true;
		yield return new WaitForSeconds(2f);
		arrived = false;
		agent.destination = patrolTargets[destPoint].position;
		destPoint = (destPoint + 1) % patrolTargets.Length;
	}
}
