using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    public bool isPaused = false;
	private GameplayManager _gameplayManager;
    private InventorySlot[] _itemSlots;

	void Start()
    {
		var inventory = GameObject.Find("ItemsParent");
		_itemSlots = inventory.GetComponentsInChildren<InventorySlot>();
		_gameplayManager = GameObject.FindGameObjectWithTag("GameplayManager").GetComponent<GameplayManager>();
	}

	void Update()
    {
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
		if(Input.GetKeyDown(KeyCode.Space))
			_gameplayManager.Continue();
				
	}

	private void PauseScene()
	{
		Time.timeScale = 0.0f;
		isPaused = true;
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
	}

}
