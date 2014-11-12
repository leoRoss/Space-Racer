using UnityEngine;
using System.Collections;

public class Done_DestroyByContactStrong : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	private GameLogic gameLogicScript;
	
	void Start ()
	{
		gameLogicScript = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
	}
	
	void OnTriggerEnter (Collider other)
	{
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
			gameLogicScript.AvatarCollidedWithStrongAstroid();
		}

		//Destroy (gameObject); No, strong border!
	}
}