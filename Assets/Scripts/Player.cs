using UnityEngine;
using System.Collections;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;


// Player inherits from MovingObject
public class Player : MovingObject
{
	public float restartLevelDelay = 1f;			
	public int pointsPerFood = 10;				
	public int pointsPerSoda = 20;				
	public int wallDamage = 1;					
	public Text foodText;						

	// Audio for the player Darcy
	public AudioClip moveSound1;					
	public AudioClip moveSound2;					
	public AudioClip eatSound1;					
	public AudioClip eatSound2;					
	public AudioClip drinkSound1;				
	public AudioClip drinkSound2;				
	public AudioClip gameOverSound;				
		
	private Animator animator;					//Used to store a reference to the Player's animator component.
	private int food;                           //Used to store player food points during level.

// For Phoner User Only
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
    private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif
		
		
	protected override void Start ()
	{
		
		animator = GetComponent<Animator>();
		food = GameManager.instance.playerFoodPoints;
		// Show the food points, which is at the bottom at game canvas
		foodText.text = "Food: " + food;
		base.Start ();
	}
		
		
	//This function is called when the behaviour becomes disabled or inactive. (From Unity documentations)
	private void OnDisable ()
	{
		//Store the food points
		GameManager.instance.playerFoodPoints = food;
	}
		
		
	private void Update ()
	{
		
		if(!GameManager.instance.playersTurn) return;
			
		int horizontal = 0;  	//Used to store the horizontal move direction. (1-right, -1-left)
		int vertical = 0;       //Used to store the vertical move direction. (1-up, -1-down)

		
#if UNITY_STANDALONE || UNITY_WEBPLAYER

		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
		vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
		//In case move vertically and horizontally at same time
		if(horizontal != 0)
		{
			vertical = 0;
		}
// For Phoner User Only		
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
		//Check if Input has registered more than zero touches
		if (Input.touchCount > 0)
		{
			//Store the first touch detected.
			Touch myTouch = Input.touches[0];
				
			//Check if the phase of that touch equals Began
			if (myTouch.phase == TouchPhase.Began)
			{
				//If so, set touchOrigin to the position of that touch
				touchOrigin = myTouch.position;
			}
				
			//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
			else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
			{
				//Set touchEnd to equal the position of this touch
				Vector2 touchEnd = myTouch.position;
					
				//Calculate the difference between the beginning and end of the touch on the x axis.
				float x = touchEnd.x - touchOrigin.x;
					
				//Calculate the difference between the beginning and end of the touch on the y axis.
				float y = touchEnd.y - touchOrigin.y;
					
				//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
				touchOrigin.x = -1;
					
				//Check if the difference along the x axis is greater than the difference along the y axis.
				if (Mathf.Abs(x) > Mathf.Abs(y))
					//If x is greater than zero, set horizontal to 1, otherwise set it to -1
					horizontal = x > 0 ? 1 : -1;
				else
					//If y is greater than zero, set horizontal to 1, otherwise set it to -1
					vertical = y > 0 ? 1 : -1;
			}
		}
			
#endif 
		//Move if we get command
		if (horizontal != 0 || vertical != 0)
		{
			AttemptMove<Wall> (horizontal, vertical);
		}
	}
		
	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		//Every time player moves, subtract from food points total.
		food--;
			
		//Update food text
		foodText.text = "Food: " + food;
			
		base.AttemptMove <T> (xDir, yDir);
		
		// Play sound of moving
		SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);

		CheckIfGameOver ();
		GameManager.instance.playersTurn = false;
	}
		

	//Hit the wall when Player cannot move
	protected override void OnCantMove <T> (T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}
		
		
	//for enter food soda or exit
	private void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Exit")
		{
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		}
			
		else if(other.tag == "Food")
		{
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			other.gameObject.SetActive (false);
		}
			
		else if(other.tag == "Soda")
		{
			food += pointsPerSoda;	
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
		}
	}
		
		
	private void Restart ()
	{
		//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
	}
		
		
	public void LoseFood (int loss)
	{
		animator.SetTrigger ("playerHit");
		food -= loss;
		foodText.text = "-"+ loss + " Food: " + food;
		CheckIfGameOver ();
	}
		
		
	private void CheckIfGameOver ()
	{
		if (food <= 0) 
		{
			SoundManager.instance.PlaySingle (gameOverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.instance.GameOver ();
		}
	}
}


