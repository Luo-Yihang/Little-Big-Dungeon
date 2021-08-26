using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;		
using UnityEngine.UI;					
	
public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;						
	public float turnDelay = 0.1f;							
	public int playerFoodPoints = 100;						
	public static GameManager instance = null;				
	[HideInInspector] public bool playersTurn = true;       //HideInInspector means hidden in Unity inspector.

	public int level = 1;                                   //Current level number, at start, it should be 1

	private Text levelText;									
	private GameObject levelImage;							
	private BoardManager boardScript;						
	private List<Enemy> enemies;							 //List of all Enemy units, used to issue them move commands.
	private bool enemiesMoving;								
	private bool doingSetup = true;							//If the board set up finished
		
		
		
	void Awake()
	{
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);	
			
		//This is a cool function to use when creating level game
		DontDestroyOnLoad(gameObject);
			
		enemies = new List<Enemy>();

		boardScript = GetComponent<BoardManager>();
			
		InitGame();
	}

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

	//The set up for game
	void InitGame()
	{
		doingSetup = true;	
		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Day " + level;
		levelImage.SetActive(true);
		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);
		enemies.Clear();
		boardScript.SetupScene(level);
	}
		
		

	void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}
		
	//Update is called every frame.
	void Update()
	{
		if(playersTurn || enemiesMoving || doingSetup)	
			return;

		StartCoroutine (MoveEnemies ());
	}
		
	// We can see from here that the function in the game is reallt detailed
	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}
		
		
		
		
	//Coroutine to move enemies in sequence.  A really perfect Coroutine used here
	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
			
		yield return new WaitForSeconds(turnDelay);
			
		//If there are no enemies spawned (IE in first level):
		if (enemies.Count == 0) 
		{
			yield return new WaitForSeconds(turnDelay);
		}
			
		//Loop through List of Enemy objects.
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].MoveEnemy ();
				
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		playersTurn = true;
		enemiesMoving = false;
	}


	//GameOver is called when the player reaches 0 food points
	public void GameOver()
	{
		levelText.text = "After " + level + " days, you starved.";
		levelImage.SetActive(true);
		//Disable this GameManager.
		enabled = false;
	}
		
}


