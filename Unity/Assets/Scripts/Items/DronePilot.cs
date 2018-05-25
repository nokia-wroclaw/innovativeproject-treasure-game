using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Drone Pilot")]
public class DronePilot : Item
{
    public override IEnumerator Use(Action<bool> result)
    {
        var drones = GameObject.FindGameObjectsWithTag("Drone");
        var player = GameObject.FindGameObjectWithTag("Player");

        double shortestDistance = Double.MaxValue;
        GameObject closestObject = null;

        foreach (var drone in drones)
        {
            var distance = Vector3.Distance(new Vector3(drone.transform.position.x, 0, drone.transform.position.z), 
                                            new Vector3(player.transform.position.x, 0, player.transform.position.z));
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestObject = drone;
            }
        }

        if (shortestDistance < 10)
        {
            var animator = closestObject.GetComponent<Animator>();
            var rb = closestObject.GetComponent<Rigidbody>();

            closestObject.GetComponent<NavMeshAgent>().enabled = false;
            closestObject.GetComponent<Patroller>().enabled = false;
            closestObject.GetComponent<DroneVision>().enabled = false;
            Destroy(closestObject.GetComponent<Patroller>());
            Destroy(closestObject.GetComponent<NavMeshAgent>());

            rb.useGravity = true;

            animator.Play("Fall");

            result(true);
            yield return new WaitForSeconds(10f);

            Destroy(closestObject);
            yield return null;
        }
            
        result(false);
        yield return null;
    }
}
