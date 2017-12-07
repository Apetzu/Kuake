using UnityEngine;
using UnityEngine.UI;// we need this namespace in order to access UI elements within our script
using System.Collections;
using UnityEngine.SceneManagement; // neded in order to load scenes
using UnityEngine.Networking;

public class menu : MonoBehaviour 
{
	public Button quitText;
	public Button startText;
	public Button exitText;

	void Start ()
	{
//		quitText = quitText.GetComponent<Button>();
		startText = startText.GetComponent<Button> ();
		exitText = exitText.GetComponent<Button> ();

	}
		

	public void StartLevel () //this function will be used on our Play button
	{
		SceneManager.LoadScene (1);
		Network.InitializeServer (32, 7777, true);

	}

	public void ExitGame () //This function will be used on our "Yes" button in our Quit menu
	{
		Debug.Log ("asd");
		Application.Quit(); //this will quit our game. Note this will only work after building the game

	}

}