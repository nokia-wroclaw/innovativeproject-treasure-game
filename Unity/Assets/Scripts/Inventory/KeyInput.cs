using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput : MonoBehaviour {
	private InventorySlot[] _itemSlots;
	public bool isPaused = false;
	void Start () {
		var inventory = GameObject.Find("ItemsParent");
		_itemSlots = inventory.GetComponentsInChildren<InventorySlot>();
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1))
			_itemSlots[0].UseItem();
		if(Input.GetKeyDown(KeyCode.Alpha2))
			_itemSlots[1].UseItem();
		if(Input.GetKeyDown(KeyCode.Alpha3))
			_itemSlots[2].UseItem();
		if(Input.GetKeyDown(KeyCode.Alpha4))
			_itemSlots[3].UseItem();
		if(Input.GetKeyDown(KeyCode.Alpha5))
			_itemSlots[4].UseItem();	
		if(Input.GetKeyDown(KeyCode.Escape))
			PauseScene();
				
	}

	private void PauseScene()
	{
		Time.timeScale = 0.0f;
		isPaused = true;
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
		//Time.timeScale = 1.0f;
	}

}
