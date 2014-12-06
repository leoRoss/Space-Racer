using UnityEngine;
using System.Collections;

public class SpeedBoostScript : MonoBehaviour {
	private AvatarScript avatarS;
	
	void Start ()
	{
		avatarS = GameObject.Find ("Avatar").GetComponent<AvatarScript> ();
	}

	void Update()
	{
		transform.Rotate (new Vector3(0, 30, 0) * 5 * Time.deltaTime);
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player")
		{
			avatarS.addFreeBoostTime(2f);
		}
		Destroy (gameObject);

	}
}