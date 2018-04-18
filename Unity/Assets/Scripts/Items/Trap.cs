using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Trap")]
public class Trap : Item
{
    [SerializeField]
    private GameObject _itemToDrop;

    public override IEnumerator Use(Action<bool> result)
    {   
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        Instantiate(_itemToDrop, playerObject.transform.position, _itemToDrop.transform.rotation);
        result(true);
        yield return null;
    }
}
