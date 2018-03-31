using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Patroller : MonoBehaviour 
{
    public Transform[] patrolTargets;

	private NavMeshAgent agent;
    private Animator anim;
    private GameObject player;
    private PlayerVisibility playerVisibility;

    private int destPoint;
    private bool arrived;
    private bool patrolling;

	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
        playerVisibility = GetComponent<PlayerVisibility>();
        player = playerVisibility.Player;
        if (player == null)
            Debug.LogError("Cannot find player object");
    }
	
	void FixedUpdate () 
	{
        if (agent.pathPending)
            return;
        
        if (patrolling)
        {
            if (playerVisibility.Chasing)
            {
                agent.destination = player.transform.localPosition;
            }
            else
            {
                if (agent.remainingDistance < agent.stoppingDistance)
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

        anim.SetFloat("blendSpeed", agent.velocity.sqrMagnitude);
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
        agent.destination = patrolTargets[destPoint].position;
        destPoint = (destPoint + 1) % patrolTargets.Length;
    }
}
