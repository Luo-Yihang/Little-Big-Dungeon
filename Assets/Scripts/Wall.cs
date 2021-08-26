using UnityEngine;
using System.Collections;


public class Wall : MonoBehaviour
{
	public AudioClip chopSound1;				
	public AudioClip chopSound2;				
	public Sprite dmgSprite;					
	public int hp = 3;							//hit points for the wall.
		
		
	private SpriteRenderer spriteRenderer;		//Store a component reference to the attached SpriteRenderer.
		
		
	void Awake ()
	{
		//Get a component reference to the SpriteRenderer.
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
		
		
	//DamageWall is called when the player attacks a wall.
	public void DamageWall (int loss)
	{
		//Call the RandomizeSfx function of SoundManager to play one of two chop sounds.
		SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
			
		spriteRenderer.sprite = dmgSprite;
			
		hp -= loss;
			
		if(hp <= 0)
			gameObject.SetActive (false);
	}
}

