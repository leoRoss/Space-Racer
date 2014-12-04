using UnityEngine;
using System.Collections;

public class BombDestroyContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	private GameLogic gameLogicScript;
	private AvatarScript aScript;
	
	void Start ()
	{
		gameLogicScript = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
		aScript = GameObject.Find ("Avatar").GetComponent<AvatarScript> ();
	}
	
	void OnTriggerEnter (Collider other)
	{
		
		Start ();
		if (other == null)
			return;
		
		if (other.tag == "Boundary" || other.tag == "Enemy")
		{
			return;
		}
		
		if (explosion != null)
		{
			Instantiate(explosion, transform.position, transform.rotation);
		}
		
		if (other.tag == "Player")
		{
			Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			gameLogicScript.AvatarCollidedWithWeakAstroid();
			aScript.AddBomb();
			GameLogic.notifications.Add("BOMB");
		}
		
		Destroy (gameObject);
	}
}