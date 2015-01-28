using UnityEngine;
using System.Collections;

public class BasicAI : MonoBehaviour {

	public Task[] tasks;
	public GameObject[] knownCovers;
	public GameObject[] knownEnemies;
	public GameObject enemyTarget;
	GameObject closestCover;
	public Vector3 target;
	public bool inCover;
	public float speed;
	public bool firstCommand;
	public float distanceToCover;
	public float distanceToEnemy;
	public bool seeking;

	// Use this for initialization
	void Start () 
	{
		//Test
		PopulateKnownCovers ();
		PopulateKnownEnemies ();
		closestCover = null;
		speed = 30.0f;
		distanceToCover = 0;
		firstCommand = true;
	}

	protected void Seek(Vector3 targetPos)
	{
		if(Vector3.Distance(transform.position,targetPos) > 3.0f)
		{
			seeking = true;
			float z = Mathf.Atan2 ((targetPos.y - transform.position.y), 
			                      (targetPos.x - transform.position.x)) * Mathf.Rad2Deg - 90;

			transform.eulerAngles = new Vector3 (0, 0, z);

			rigidbody2D.AddForce (gameObject.transform.up * speed);

			distanceToCover = Vector3.Distance(transform.position, targetPos);

			if(distanceToCover < 5.0f)
			{
				firstCommand = true;
				inCover = true;
				//Add AI to cover list, using closestcover gameobject as a ref
			}
		}
		else
		{
			seeking = false;
			print("No longer seeking");
		}
	}

	protected void ChooseASide()
	{

		BoxCollider2D coverCollider = closestCover.gameObject.GetComponent<BoxCollider2D>();
		
		Vector2 p1 = closestCover.transform.position - new Vector3(((Mathf.Cos(closestCover.transform.rotation.z) * coverCollider.size.x / 2)),(Mathf.Sin(closestCover.transform.rotation.z) * coverCollider.size.x / 2),0);
		Vector2 p2 = closestCover.transform.position + new Vector3(((Mathf.Cos(closestCover.transform.rotation.z) * coverCollider.size.x / 2)),(Mathf.Sin(closestCover.transform.rotation.z) * coverCollider.size.x / 2),0);

		if(Vector2.Distance(p1,new Vector2(enemyTarget.transform.position.x,enemyTarget.transform.position.y)) < Vector2.Distance(p2,new Vector2(enemyTarget.transform.position.x,enemyTarget.transform.position.y)))
		{
			target = new Vector3(p1.x,p1.y,0);

			print("Chose Left Side");
		}
		else
		{
			target = new Vector3(p2.x,p2.y,0);

			print("Chose Right Side");
		}
	}

	void PopulateKnownCovers()
	{
		knownCovers = GameObject.FindGameObjectsWithTag("Cover");
	}

	void PopulateKnownEnemies()
	{
		knownEnemies = GameObject.FindGameObjectsWithTag ("Enemy");
	}

	void SetEnemyTarget()
	{
		float lastDistance = 10000.0f;
		
		for(int i = 0; i < knownEnemies.Length; i++)
		{
			distanceToEnemy = Vector3.Distance(transform.position, knownEnemies[i].transform.position);
			
			if( distanceToEnemy < lastDistance )
			{
				enemyTarget = knownEnemies[i];
			}
			
			//print (knownCovers[i] + " " + distance);
			lastDistance = distanceToEnemy;
		}
	}

	void SetCoverTarget()
	{
		print ("Setting Cover");
		float lastDistance = 10000.0f;

		for(int i = 0; i < knownCovers.Length; i++)
		{
			distanceToCover = Vector3.Distance(transform.position, knownCovers[i].transform.position);
		
			if( distanceToCover < lastDistance )
			{
				closestCover = knownCovers[i];
			}
		
			//print (knownCovers[i] + " " + distance);
			lastDistance = distanceToCover;
		}

		target = closestCover.transform.position;

		//print (closestCover);
	}

	void Update()
	{
		//print (knownCovers [1]);

		//Seek cover, testing
		if(firstCommand && !inCover)
		{
			SetCoverTarget ();	
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(inCover)
		{
			ChooseASide();

			if(distanceToCover > 13.0f)
			{
				inCover = false;
			}
		}

		Seek (target);
	}
}
