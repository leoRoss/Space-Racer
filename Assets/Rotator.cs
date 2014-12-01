using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	// Update is called once per frame

	public AudioSource backgroundMusic;

	void Update () {
		if (GameLogic.startedPlay)
			backgroundMusic.Stop();
		transform.Rotate (new Vector3 (0, 0, 30) * Time.deltaTime);
	}
}