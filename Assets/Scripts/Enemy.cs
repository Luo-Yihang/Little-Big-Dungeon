using UnityEngine;
using System.Collections;


//Enemy inherits from MovingObject
public class Enemy : MovingObject
{
	public int playerDamage; 							
	public AudioClip attackSound1;						
	public AudioClip attackSound2;						
		
		
	private Animator animator;							
	private Transform target;							
	private bool skipMove;								
		
		

	protected override void Start ()
	{
		GameManager.instance.AddEnemyToList (this);
		animator = GetComponent<Animator> ();
			
		//Find the Player GameObject using it's tag and store a reference to its transform component. (From Unity Documentations)
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		base.Start ();
	}
		
		
	// I let the enemies move 1 time while player move 2 time, so the game could be easier
	// But things will be different for 5* level, just like Plants VS Zombies!
	protected override void AttemptMove <T> (int xDir, int yDir)
	{
        if (GameManager.instance.level%5 ==0) {
			base.AttemptMove<T>(xDir, yDir);
		}
        else
        {
			if (skipMove)
			{
				skipMove = false;
				return;
			}

			base.AttemptMove<T>(xDir, yDir);

			skipMove = true;

		}
		
	}
		
		
	//MoveEnemy is tring to let enemy move towards to users
	public void MoveEnemy ()
	{

		int xDir = 0;
		int yDir = 0;
			
		//If the difference in positions is approximately zero (Epsilon) do the following:
		if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
			yDir = target.position.y > transform.position.y ? 1 : -1;
			
		//If the difference in positions is not approximately zero (Epsilon) do the following:
		else
			xDir = target.position.x > transform.position.x ? 1 : -1;
			
		AttemptMove <Player> (xDir, yDir);
	}


	//Hit the player when Enemy cannot move
	protected override void OnCantMove <T> (T component)
	{
		Player hitPlayer = component as Player;
		hitPlayer.LoseFood (playerDamage);
		animator.SetTrigger ("enemyAttack");
		SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
	}
}

