using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	#region Singleton
	public static Inventory instance;
	
	void Awake()
	{
		instance = this;
	}
	#endregion

	public int space = 5;
	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;
	public List<Item> listOfItems;

	public bool AddItem(Item newItem)
	{
		if(listOfItems.Count < space)
		{
			listOfItems.Add(newItem);
			if(onItemChangedCallback != null)
				onItemChangedCallback.Invoke();
			return true;
		}
		return false;
	}

	public void RemoveItem(Item item)
	{
		listOfItems.Remove(item);
		if(onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}
}
