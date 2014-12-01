using UnityEngine;
using System.Collections;

public class Done_DestroyByContact : MonoBehaviour
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
		}

		Destroy (gameObject);
	}
}