using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {
	
	public GUISkin skin;
	
	//KEITH: Tie in the Main Menu Scene with this game logic
	//ie: going back and forth between the two scenes, getting a high score, etc
	
	//take care of the logic and GUI for these, 
	public float time;
	public float health;
	public float healthScale = 1.0f;
	public float boostScale = 1.0f;
	public GUIText timeText;
	public GUIText boost;
	
	//these are updated every frame, you take care of the GUI for them
	public int numberOfBoostsLeft;
	public float timeLeftOnTheCurrentBoost;
	public int numberOfBulletsLeft;
	public Vector3 avatarPos;
	public static float highScore;
	public static float lastScore;

	public AudioSource explosion;
	public AudioSource backgroundMusic;

	public static bool played;

	//course stuff
	public float courseLength = 50000f;

	private bool drawGUI = true;
	public static bool startedPlay = false;

	public GUIText instructionsText;

	public Texture2D boostTexture;
	public Texture2D bombTexture;
	
	AvatarScript avatarScript;
	GenerateEnvironment genEnv;

	void OnGUI () {
		if (drawGUI) {
						GUI.backgroundColor = Color.red;
			GUI.Button (new Rect (0, Screen.height - 30, Screen.width * healthScale, 30), "Health", skin.button);
						if (numberOfBoostsLeft == 0)
								GUI.backgroundColor = Color.black;
						else
								GUI.backgroundColor = Color.blue;
			GUI.Button (new Rect (0, Screen.height - 60, Screen.width * boostScale, 30), "Boost", skin.button);
						float roundedTime = Mathf.Round (time * 100) / 100;
						timeText.text = "Time : " + (roundedTime).ToString ();

			for (int i = 0 ; i < numberOfBoostsLeft ; i++)
				GUI.DrawTexture(new Rect(i * 55, Screen.height - 110, boostTexture.width, boostTexture.height), boostTexture);

			for (int i = 0 ; i < avatarScript.getBullets () ; i++)
				GUI.DrawTexture(new Rect(i * 55 + (numberOfBoostsLeft * 55), Screen.height - 110, bombTexture.width, bombTexture.height), bombTexture);

				} else {
			timeText.text = "";
			boost.text = "";
				}
	}

	IEnumerator FadeInstructions() {
		for (float f = 5f; f >= 0; f -= 0.05f) {
			Color c = instructionsText.color;
			c.a = f/5f;
			instructionsText.color = c;
			yield return new WaitForSeconds(.01f);
		}
		StartCoroutine(FadeInstructions());
	}

	// Use this for initialization
	void Start () {
		instructionsText.enabled = false;
		startedPlay = true;
		DontDestroyOnLoad (backgroundMusic);
		if (!played) {
			backgroundMusic.Play ();
			startedPlay = true;
				}
		played = true;
		timeText.text = "" + 0;
		boost.text = "" + 0;
		avatarScript = GameObject.Find ("Avatar").GetComponent<AvatarScript> ();
		genEnv = GameObject.Find ("EnvironmentGenerator").GetComponent<GenerateEnvironment> ();
	//	myStar = Instantiate (starField, new Vector3(0f,0f,0f), Quaternion.identity) as GameObject;
		StartGame ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R)) {
			EndGame ();
			Application.LoadLevel (1);
		}
		
		time += Time.deltaTime;
		
		updateMyVariables ();
		
	//	myStar.transform.position = avatarPos;
		
		if (timeLeftOnTheCurrentBoost == 0)
			boostScale = 1.0f;
		else
			boostScale = Mathf.Clamp01 (timeLeftOnTheCurrentBoost / AvatarScript.boostTime);
		
		if (health <= 0) {
			AvatarFailedTheCourse();
		}
		//check AvatarZ to see if we completed the course
		if (avatarPos.z > courseLength) {
			AvatarCompletedTheCourse ();
		}
	}

	public void BombsAway() {
		//please make an explosion occur
	}
	//use numberOfBulletsLeft to make the UI :)

	public void AddHealth() {
		health += 40;
		healthScale = Mathf.Clamp01(health / 100.0f);
	}

	void updateMyVariables () {
		numberOfBoostsLeft = avatarScript.getBoosts ();
		timeLeftOnTheCurrentBoost = avatarScript.getBoostTimeLeft ();
		numberOfBulletsLeft = avatarScript.getBullets ();
		avatarPos = avatarScript.getPos ();
	}
	
	void StartGame() {
		avatarScript.StartGame();
		genEnv.StartGame ();
		updateMyVariables ();
		//the above 3 must happen b4 anything else
		resetGameLogicVariables ();
	}
	
	void resetGameLogicVariables () {
		health = 100f;
		time = 0f;
	}
	
	void EndGame() {
		avatarScript.EndGame();
		genEnv.EndGame ();
	}

    void pauseGame() {
		instructionsText.enabled = true;
		StartCoroutine(FadeInstructions());
		avatarScript.pauseGame ();
		genEnv.EndGame ();
		drawGUI = false;
		}


	//Asteroids will triger this when appropriate
	public void AvatarCollidedWithStrongAstroid() {
		avatarScript.AvatarCollidedWithStrongAstroid ();
		health -= 20;
		healthScale = Mathf.Clamp01(health / 100.0f);
		explosion.Play();
	}
	
	//Asteroids will triger this fun when appropriate
	public void AvatarCollidedWithWeakAstroid() {
		avatarScript.AvatarCollidedWithWeakAstroid ();
		health -= 20;
		healthScale = Mathf.Clamp01(health / 100.0f);
		explosion.Play();
	}
	
	void AvatarCompletedTheCourse() {
		float roundedTime = Mathf.Round (time * 100) / 100;
		lastScore = roundedTime;
		if (time < highScore || highScore == 0) {
						highScore = roundedTime;
				}
		EndGame ();
		Application.LoadLevel (0);
	}
	
	void AvatarFailedTheCourse() {
		pauseGame();
	}
}