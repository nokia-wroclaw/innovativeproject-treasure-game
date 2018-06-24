using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour {

	public GameObject resultCanvas;
	public Sprite winImage;
	public Sprite loseImage;
	// Use this for initialization
	void Start () 
	{
		resultCanvas.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void Win()
	{
		resultCanvas.GetComponent<Image>().sprite = winImage;
		Pause();
	}

	public void Lose()
	{
		resultCanvas.GetComponent<Image>().sprite = loseImage;
		Pause();
	}

	public void Continue()
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene("MainMenu");
	}

	private void Pause()
	{
		resultCanvas.SetActive(true);
		Time.timeScale = 0.0f;
	}
}
