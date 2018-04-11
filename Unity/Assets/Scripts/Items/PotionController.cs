using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionController : Interactables
{
    [SerializeField]
    private Item itemToPickup;

    protected override Func<bool> InteractCondition => (() => Input.GetKeyDown(KeyCode.E));

    protected override void Interact()
    {
        Pickup();
    }

    private void Pickup()
    {
        bool pickedUp = Inventory.instance.AddItem(itemToPickup);
        if (pickedUp)
            Destroy(gameObject);
    }

}
