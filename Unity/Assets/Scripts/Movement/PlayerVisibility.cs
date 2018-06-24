using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVisibility : Visibility
{   
    public float fieldOfViewDegrees;
    public bool canAlarm = false;
    public bool respond = true;
    public float visibilityDistance;
    public float endGameDistance;   
    private GameObject _playerSpottedObject;

    private void Update()
    {
        _playerSpottedObject.transform.position = gameObject.transform.position + new Vector3(0, 4, 0);
    }   

    protected override void SignalStateChange()
    {
        if (_chasing && !_playerSpottedObject.GetComponent<Renderer>().enabled)
        {
            _playerSpottedObject.GetComponent<Renderer>().enabled = true;
        }

        if (!_chasing && _playerSpottedObject.GetComponent<Renderer>().enabled)
        {
            _playerSpottedObject.GetComponent<Renderer>().enabled = false;

        }
    }

    protected override void InitComponents()
    {
        _playerSpottedObject = Instantiate((GameObject)Resources.Load("Prefabs/Old/ExclamationMark"));
        _playerSpottedObject.GetComponent<Renderer>().enabled = false;
    }

    protected override IEnumerator PlayerVisibilityCheck()
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

                        if (hit.distance <= visibilityDistance)
                            Chasing = true;
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
        SwitchScene();
    }

    private void SwitchScene() => 
        _gameplayManager.Lose();

}

