using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void NewGame()
	{
		SceneManager.LoadScene("FirstScene");
	}

    public void LoadGame()
    {
        SceneManager.LoadScene("LoadedScene");
    }

	public void QuitGame()
	{
		Application.Quit();
	}
}
