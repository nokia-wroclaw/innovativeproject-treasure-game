using UnityEngine;

public class PopUpControl : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		Time.timeScale = 0.0f;
	}
	
	public void Continue()
	{
		Time.timeScale = 1.0f;
		var menu = GetComponent<Canvas>();
		menu.enabled = false;
	}
}
