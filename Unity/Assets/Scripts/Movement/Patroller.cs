using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class Patroller : MonoBehaviour 
{
    private Vector3[] patrolTargets;

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
        player = GameObject.FindGameObjectWithTag("Player");
        patrolTargets = GeneratePatrolTargets();
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
        agent.destination = patrolTargets[destPoint];
        destPoint = (destPoint + 1) % patrolTargets.Length;        
    }

    private IEnumerator Stun()
    {
        Debug.Log("Stunned");
        agent.speed = 0;
        yield return new WaitForSeconds(10.0f);
        agent.speed = 2;
    }

    public void StunEnemy()
    {
        var visibility = GetComponent<PlayerVisibility>();
        visibility.stunned = true;
        StartCoroutine(Stun());
    }

}
