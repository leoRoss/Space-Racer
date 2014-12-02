using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	// Update is called once per frame
	

	void Update () {
		if(!AvatarScript.invert)
		transform.Rotate (new Vector3 (0, 0, 30) * Time.deltaTime);
		else
		transform.Rotate (new Vector3 (0, 0, -30) * Time.deltaTime);
	}
}