using UnityEngine;
using System.Collections;


public class SoundManager : MonoBehaviour 
{
	public AudioSource efxSource;					//Sound effects.
	public AudioSource musicSource;					
	public static SoundManager instance = null;				
	public float lowPitchRange = .95f;				
	public float highPitchRange = 1.05f;			
		

	// Same as GameManger
	void Awake ()
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}
		
		
	//Used to play single sound clips.
	public void PlaySingle(AudioClip clip)
	{
		efxSource.clip = clip;
		efxSource.Play ();
	}
		
		
	//RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
	//I guess this would make it more of fun to listen the music
	public void RandomizeSfx (params AudioClip[] clips)
	{
		int randomIndex = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		efxSource.pitch = randomPitch;
		efxSource.clip = clips[randomIndex];
		efxSource.Play();
	}
}

