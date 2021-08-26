using UnityEngine;
using System.Collections;


//The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
public abstract class MovingObject : MonoBehaviour
{
	public float moveTime = 0.01f;			
	public LayerMask blockingLayer;		
		
		
	private BoxCollider2D boxCollider; 		
	private Rigidbody2D rb2D;				
	private float inverseMoveTime;			//Used to make movement more efficient, because multiple is faster
	private bool isMoving = false;					
		
		

	protected virtual void Start ()
	{
		boxCollider = GetComponent <BoxCollider2D> ();
		rb2D = GetComponent <Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
	}
		
		
	protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
	{
		Vector2 start = transform.position;	
		Vector2 end = start + new Vector2 (xDir, yDir);
		//In case the ray hit the object itself
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
			

		if(hit.transform == null && !isMoving)
		{
			//Start SmoothMovement co-routine passing in the Vector2 end as destination
			StartCoroutine (SmoothMovement (end));
			return true;
		}

		return false;
	}
		
		
	//Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
	protected IEnumerator SmoothMovement (Vector3 end)
	{

		isMoving = true;
			
		
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while(sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
				
			rb2D.MovePosition (newPostion);
				
			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
			
		rb2D.MovePosition (end);
		isMoving = false;
	}


	
	protected virtual void AttemptMove<T>(int xDir, int yDir)
		where T : Component
	{
		RaycastHit2D hit;

		bool canMove = Move(xDir, yDir, out hit);

		//Check if nothing was hit by linecast
		if (hit.transform == null) 
			return;
			
		T hitComponent = hit.transform.GetComponent <T> ();
			
		if(!canMove && hitComponent != null)
			OnCantMove (hitComponent);
	}
		
		
	protected abstract void OnCantMove <T> (T component)
		where T : Component;
}

