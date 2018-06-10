using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Patroller : MonoBehaviour 
{
    public bool usePredefinedPatrolTargets;
    public Vector3[] patrolTargets;
    public float patrolSpeed = 2f;
    public float chasingSpeed = 3.5f;

    private NavMeshAgent _agent;
    private Animator _anim;
    private GameObject _player;
    private Visibility _playerVisibility;

    private int _destPoint;
    private bool _arrived;
    private bool _patrolling;

	void Start () 
	{
		_agent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
        _playerVisibility = GetComponent<Visibility>();
        _player = GameObject.FindGameObjectWithTag("Player");
        if (!usePredefinedPatrolTargets)
            patrolTargets = GeneratePatrolTargets();
    }
	
	void FixedUpdate () 
	{
        if (_agent.pathPending)
            return;
        if (_patrolling)
        {
            if (_playerVisibility.Chasing)
            {
                _agent.destination = _player.transform.localPosition;
                if (!_playerVisibility.stunned)
                    _agent.speed = chasingSpeed;
            }
            else
            {
                if ((_agent.remainingDistance  - _agent.baseOffset) < _agent.stoppingDistance)
                {
                    if (!_arrived)
                    {
                        _arrived = true;
                        if (!_playerVisibility.stunned)
                            _agent.speed = patrolSpeed;
                        StartCoroutine(GoToNextPoint());
                    }
                }
                else
                    _arrived = false;
            }       
        }
        else
            StartCoroutine(GoToNextPoint());

        _anim.SetFloat("blendSpeed", _agent.velocity.sqrMagnitude);
	}

    public void StunEnemy() => 
        StartCoroutine(Stun());

    private Vector3[] GeneratePatrolTargets()
    {
        RaycastHit hit;
        var patrolTargets = new Vector3[4];
        var mapSize = Terrain.activeTerrain.terrainData.size;
        var counter = 0;
        for (int i = 0; i < 4; i++)
        {
            Vector3 point;
            bool found = false;

            do
            {
                point = new Vector3(((float)Random.Range(0,101) / 100) * mapSize.x,
                                    1,
                                    ((float)Random.Range(0,101) / 100) * mapSize.z);
                
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
        }

        return patrolTargets;
    }

	private IEnumerator GoToNextPoint()
	{
        if (patrolTargets.Length == 0)
        {
            yield break;
        }
        _patrolling = true;

        yield return new WaitForSeconds(2.0f);

        _arrived = false;
        _agent.destination = patrolTargets[_destPoint];
        _destPoint = (_destPoint + 1) % patrolTargets.Length;        
    }

    private IEnumerator Stun()
    {
        _agent.speed = 0;
        _playerVisibility.stunned = true;

        yield return new WaitForSeconds(10.0f);

        _playerVisibility.stunned = false;
        _agent.speed = 2;
    }

    public IEnumerator Alarm(Vector3 position, DroneVision drone)
    {
        if (_agent != null && !_playerVisibility.stunned)
        {
            _agent.destination = position;
            _agent.speed = chasingSpeed;
            drone.canRespond = false;

            yield return new WaitForSeconds(5.0f);

            drone.canRespond = true;
        }
        
    }
}
