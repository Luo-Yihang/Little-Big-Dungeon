using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// The Start UI Script
public class Menu : MonoBehaviour
{

	public GameObject overlay;
	public AudioListener mainListener;
	public static string showAtStartPrefsKey = "showLaunchScreen";
	private static bool alreadyShownThisSession = false;


	void Awake()
	{
		// have we already shown this once?
		if (alreadyShownThisSession)
		{
			StartGame();
		}
		else
		{
			alreadyShownThisSession = true;
			ShowLaunchScreen();
		}

	}
	public void openUrl()
	{
		// Open URL for ClassFront
		Application.OpenURL("https://classfront.co/");
	}

	// show overlay info, pausing game time, disabling the audio listener 
	// and enabling the overlay info parent game object
	public void ShowLaunchScreen()
	{
		Time.timeScale = 0f;
		mainListener.enabled = false;
		overlay.SetActive(true);
	}

	// continue to play, by ensuring the preference is set correctly, the overlay is not active, 
	// and that the audio listener is enabled, and time scale is 1 (normal)
	public void StartGame()
	{
		overlay.SetActive(false);
		mainListener.enabled = true;
		Time.timeScale = 1f;
	}

}
