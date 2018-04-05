using System;
using UnityEngine;

public class TrapController : Interactables
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
	    if(pickedUp)
            Destroy(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.tag == "Enemy")
        {
            var patroller = other.GetComponent<Patroller>();
            patroller.StunEnemy();
            Destroy(gameObject);
        }
    }
}
