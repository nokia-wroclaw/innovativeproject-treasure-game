using UnityEngine;

public class ItemPickup : Interactables {

	public Item item;
	public override void Interact()
	{
		Pickup();
	}
	 public void Pickup()
	 {
		 if(!item.isTreasure)
		 {
			Debug.Log("Pickup");
			bool wasPickedUp = Inventory.instance.AddItem(item);
			if(wasPickedUp)
				Destroy(gameObject);
		 }
		 else
		 {
			 item.Use();
		 }
	 }
}
