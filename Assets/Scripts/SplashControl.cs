using UnityEngine;
using System.Collections;

public class SplashControl : MonoBehaviour {
	void OnMouseEnter() {
		renderer.material.color = Color.red;
	}

	void OnMouseExit() {
		renderer.material.color = Color.white;
	}

	void OnMouseUp() {
		Application.LoadLevel (1);
		}
}