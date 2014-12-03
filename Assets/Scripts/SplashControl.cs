using UnityEngine;
using System.Collections;

public class SplashControl : MonoBehaviour {

	public GUIText highScore;
	public GUIText lastScore;
	public GUIText score;
	public GUIText lScore;

	public AudioSource backgroundMusic;

	void Start () {
		if (GameLogic.lastScore == 0) {
						lastScore.enabled = false;
						lScore.enabled = false;
				} else {
						lastScore.enabled = true;
			lScore.enabled = true;
				}

		if (GameLogic.highScore == 0) {
						highScore.enabled = false;
						score.enabled = false;
				} else {
						highScore.enabled = true;
						score.enabled = true;
				}

		score.text = "" + GameLogic.highScore;
		lScore.text = ""  + GameLogic.lastScore;
		}
	
	void OnMouseEnter() {
		this.guiText.color = Color.red;
	}

	void OnMouseExit() {
		if(!AvatarScript.invert)
		this.guiText.color = Color.white;
	}

	void OnMouseUp() {
		AvatarScript.invert = !AvatarScript.invert;
		}

	void Update() {
		if (GameLogic.startedPlay)
			backgroundMusic.Stop();

		if(AvatarScript.invert)
			this.guiText.color = Color.red;

				if (Input.GetKeyDown (KeyCode.Return))
						Application.LoadLevel (1);
		}
}