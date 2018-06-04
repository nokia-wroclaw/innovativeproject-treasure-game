using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
	private bool _isPaused = false;
	public void NewGame()
	{
		if(_isPaused)
			Time.timeScale = 1.0f;
		SceneManager.LoadScene("TrainingScene");
	}

    public void LoadGame()
    {
		if(_isPaused)
			Time.timeScale = 1.0f;
        SceneManager.LoadScene("LoadedScene");
    }

	public void QuitGame() => 
        Application.Quit();

	public void ResumeGame()
	{
		Time.timeScale = 1.0f;
		SceneManager.UnloadSceneAsync("MainMenu");
	}

	void Start()
	{
		var resume = GameObject.Find("ResumeGame");
		var resumeButton = resume.GetComponent<Button>();
		KeyInput keyInput = FindObjectOfType<KeyInput>();
		
		if(keyInput != null)
			_isPaused = keyInput.isPaused;

		if(_isPaused)
		{
			resumeButton.interactable = true;
		}
		else
		{
			resumeButton.interactable = false;
		}
		
	}
}
