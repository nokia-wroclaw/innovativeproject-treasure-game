using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void NewGame()
	{
		SceneManager.LoadScene("SecondScene");
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
