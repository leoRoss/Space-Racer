using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	//KEITH: Tie in the Main Menu Scene with this game logic
	//ie: going back and forth between the two scenes, getting a high score, etc

	//take care of the logic and GUI for these, 
	public float time;
	public float health;
	
	//these are updated every frame, you take care of the GUI for them
	public int numberOfBoostsLeft;
	public float timeLeftOnTheCurrentBoost;
	public int numberOfBulletsLeft;
	public float avatarZPos;

	//course stuff
	public float courseLength = 50000f;

	AvatarScript avatarScript;
	GenerateEnvironment genEnv;

	// Use this for initialization
	void Start () {
		avatarScript = GameObject.Find ("Avatar").GetComponent<AvatarScript> ();
		genEnv = GameObject.Find ("EnvironmentGenerator").GetComponent<GenerateEnvironment> ();
		StartGame ();
	}
	
	// Update is called once per frame
	void Update () {
		updateMyVariables ();
		//TODO
		//update the GUI for the number of boosts left
		//update the GUI for the time left on the current boost (like a draining rectangle)
		if (health < 0) {
			AvatarFailedTheCourse();
		}
		//check AvatarZ to see if we completed the course
		if (avatarZPos > courseLength) {
			AvatarCompletedTheCourse ();
		} 
	}

	void updateMyVariables () {
		numberOfBoostsLeft = avatarScript.getBoosts ();
		timeLeftOnTheCurrentBoost = avatarScript.getBoostTimeLeft ();
		numberOfBulletsLeft = avatarScript.getBullets ();
		avatarZPos = avatarScript.getZPos ();
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

	//AvatarScript will triger this fun when appropriate
	void AvatarCollidedWithAstroid() {
		//TODO
	}

	void AvatarCompletedTheCourse() {
		//TODO
		//Be sure to call EndGame so other scripts do their shit
		EndGame ();
		//Then go back to the Menu Scene with new highscore
	}

	void AvatarFailedTheCourse() {
		//TODO
		//Be sure to call EndGame so other scripts do their shit
		EndGame ();
		//Then go back to the Menu Scene without new highscore
	}


}
