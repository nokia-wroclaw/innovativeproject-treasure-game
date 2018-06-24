using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour {

	public GameObject resultCanvas;
	public Sprite winImage;
	public Sprite loseImage;
	public bool isEnd;
	// Use this for initialization
	void Start () 
	{
		resultCanvas.SetActive(false);
		isEnd = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void Win()
	{
		resultCanvas.GetComponent<Image>().sprite = winImage;
		isEnd = true;
		Pause();
	}

	public void Lose()
	{
		resultCanvas.GetComponent<Image>().sprite = loseImage;
		isEnd = true;
		Pause();
	}

	public void Continue()
	{
		Time.timeScale = 1.0f;
		isEnd = false;
		SceneManager.LoadScene("MainMenu");
	}

	private void Pause()
	{
		resultCanvas.SetActive(true);
		Time.timeScale = 0.0f;
	}
}
