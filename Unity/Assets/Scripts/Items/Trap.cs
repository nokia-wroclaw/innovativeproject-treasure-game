using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Trap")]
public class Trap : Item
{
    [SerializeField]
    private GameObject itemToDrop;

    public override IEnumerator Use(Action<bool> result)
    {   
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        Instantiate(itemToDrop, playerObject.transform.position, itemToDrop.transform.rotation);
        result(true);
        yield return null;
    }
}
