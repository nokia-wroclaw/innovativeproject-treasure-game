using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	public Transform itemsParent;

	private Inventory _inventory;
    private InventorySlot[] _slots;

	void Start()
	{
		_inventory = Inventory.instance;
		_inventory.onItemChangedCallback += UpdateUI;
		_slots = itemsParent.GetComponentsInChildren<InventorySlot>();
	}

	void UpdateUI()
	{
		for(int i = 0; i < _slots.Length; i++)
		{
			if(i < _inventory.listOfItems.Count)
			{
				_slots[i].AddItem(_inventory.listOfItems[i]);
			}
			else
			{
				_slots[i].ClearSlot();
			}
		}
	}
}
