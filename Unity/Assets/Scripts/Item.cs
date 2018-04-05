using UnityEngine;

public class Item : ScriptableObject {

	public bool isTreasure;
	new public string name = "Item";
	public Sprite icon;
	public virtual void Use()
	{
	}
}
