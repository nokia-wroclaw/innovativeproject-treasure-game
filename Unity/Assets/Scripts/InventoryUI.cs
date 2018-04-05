using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {

	public Transform itemsParent;
	Inventory inventory;
	InventorySlot[] slots;
	void Start()
	{
		inventory = Inventory.instance;
		inventory.onItemChangedCallback += UpdateUI;
		slots = itemsParent.GetComponentsInChildren<InventorySlot>();
	}
	void Update () {
		
	}

	void UpdateUI()
	{
		for(int i = 0; i < slots.Length; i++)
		{
			if(i < inventory.listOfItems.Count)
			{
				slots[i].AddItem(inventory.listOfItems[i]);
			}
			else
			{
				slots[i].ClearSlot();
			}
		}
	}
}
