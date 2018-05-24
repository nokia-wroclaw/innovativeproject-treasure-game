using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneVision : Visibility 
{
	public float visibilityDistance;
	public bool canRespond;
	private IEnumerable<GameObject> _guardsList;
    private Light _light;

	private GameObject GetNearestGuard()
    {
        var guard = _guardsList.FirstOrDefault();
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

    protected override void InitComponents()
    {
        _light = GetComponentInChildren<Light>();
		_guardsList = GameObject.FindGameObjectsWithTag("Enemy");
		canRespond = true;
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

            if (Physics.Raycast(transform.position, rayDirection, out hit))
            {
                if (hit.transform.CompareTag("Player") && hit.distance < visibilityDistance && !stunned)
                {
                    Chasing = true;
                    if(canRespond)
                    {
                        var guard = GetNearestGuard();
                        var patroller = guard.GetComponent<Patroller>();
                        StartCoroutine(patroller.Alarm(transform.position, this));                                
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    protected override void SignalStateChange()
    {
        if (_chasing)
			_light.color = new Color(1, 0, 0);
		else
			_light.color = new Color(1, 1, 0);
    }
}
