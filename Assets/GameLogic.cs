﻿using UnityEngine;
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
	
//	public GameObject starField;
//	GameObject myStar;

	//course stuff
	public float courseLength = 50000f;
	
	AvatarScript avatarScript;
	GenerateEnvironment genEnv;
	
	
	void OnGUI () {
		GUI.backgroundColor = Color.red;
		GUI.Button(new Rect(20, 660, 510 * healthScale, 30), "Health", skin.button);
		if(numberOfBoostsLeft == 0)
			GUI.backgroundColor = Color.black;
		else
			GUI.backgroundColor = Color.blue;
		GUI.Button(new Rect(740, 660, 510 * boostScale, 30), "Boost", skin.button);
		float roundedTime = Mathf.Round (time * 100) / 100;
		timeText.text = "Time : " + (roundedTime).ToString();
		boost.text = "Boosts Left : " + numberOfBoostsLeft;
	}
	
	// Use this for initialization
	void Start () {
		timeText.text = "" + 0;
		boost.text = "" + 0;
		//	highScore = 0.0f;
		avatarScript = GameObject.Find ("Avatar").GetComponent<AvatarScript> ();
		genEnv = GameObject.Find ("EnvironmentGenerator").GetComponent<GenerateEnvironment> ();
	//	myStar = Instantiate (starField, new Vector3(0f,0f,0f), Quaternion.identity) as GameObject;
		StartGame ();
	}
	
	// Update is called once per frame
	void Update () {
		
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
	
	//Asteroids will triger this when appropriate
	public void AvatarCollidedWithStrongAstroid() {
		avatarScript.AvatarCollidedWithStrongAstroid ();
		health -= 20;
		healthScale = Mathf.Clamp01(health / 100.0f);
	}
	
	//Asteroids will triger this fun when appropriate
	public void AvatarCollidedWithWeakAstroid() {
		avatarScript.AvatarCollidedWithWeakAstroid ();
		health -= 20;
		healthScale = Mathf.Clamp01(health / 100.0f);
	}
	
	void AvatarCompletedTheCourse() {
		//Be sure to call EndGame so other scripts do their shit
		if (time < highScore)
			highScore = time;
		EndGame ();
		Application.LoadLevel (0);
	}
	
	void AvatarFailedTheCourse() {
		//Be sure to call EndGame so other scripts do their shit
		EndGame ();
		Application.LoadLevel (0);
	}
}