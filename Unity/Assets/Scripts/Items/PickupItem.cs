using System;
using UnityEngine;

public class PickupItem : Interactables
{
    [SerializeField]
    private Item _itemToPickup;

    protected override Func<bool> InteractCondition => (() => Input.GetKeyDown(KeyCode.E));

    protected override void Interact()
    {
        if (gameObject.tag != "DroppedItem")
        {
            Pickup();
        }  
    }

    private void Pickup()
    {   
        bool pickedUp = Inventory.instance.AddItem(_itemToPickup);
        if (pickedUp)
            Destroy(gameObject);
    }

}
