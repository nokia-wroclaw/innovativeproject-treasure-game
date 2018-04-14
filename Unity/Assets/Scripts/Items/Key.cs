using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Key")]
public class Key : Item
{
    public override IEnumerator Use(Action<bool> result)
    {
        var doors = GameObject.FindGameObjectsWithTag("Door");
        var player = GameObject.FindGameObjectWithTag("Player");

        foreach (var door in doors)
        {
            if (Vector3.Distance(door.transform.localPosition, player.transform.localPosition) < 3)
            {
                Destroy(door);
                result(true);
                yield return null;
            }
        }

        result(false);
        yield return null;
    }
}
