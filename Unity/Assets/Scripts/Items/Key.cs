﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Key")]
public class Key : Item
{
    public override IEnumerator Use(Action<bool> result)
    {
        var doors = GameObject.FindGameObjectsWithTag("Door");
        var player = GameObject.FindGameObjectWithTag("Player");

        double shortestDistance = Int32.MaxValue;
        GameObject closestObject = null;

        foreach (var door in doors)
        {
            var distance = Vector3.Distance(door.transform.localPosition, player.transform.localPosition);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestObject = door;
            }
        }

        if (shortestDistance < 3)
        {
            Destroy(closestObject);
            result(true);

            yield return new WaitForSeconds(0.5f);
            //performance todo
            var navMesh = GameObject.FindGameObjectWithTag("Terrain").GetComponent<NavMeshSurface>();
            navMesh.BuildNavMesh();

            yield break;
        }


        result(false);
        yield break;
    }
}
