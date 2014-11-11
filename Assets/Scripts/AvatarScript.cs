using UnityEngine;
using System.Collections;

public class AvatarScript : MonoBehaviour {

	private CharacterController controller;
	private Vector3 moveVector;

	private ImmediateStateMachine stateMachine = new ImmediateStateMachine ();
	private CollisionFlags collisionFlags;

	private float normalFwdAccel = 80f;
	private float normalSidesAccel = 500f;
	private float normalFwdMaxSpeed = 300f;
	private float normalSidesMaxSpeed = 200f;

	private float boostFwdAccelFactor = 2.0f;
	private float boostSidesAccelFactor = 2.0f;
	private float boostFwdMaxSpeedFactor = 2.0f;
	private float boostSidesMaxSpeedFactor = 2.0f;

	private float turnDecelPerSecond = 0.05f; //after one second, only 0.05 of you side or up moment are left if you stop pressing button

	private float boostTime = 4f; //each boost lasts 4 seconds
	private int boosts;
	private float boostTimeLeft;
	
	private float up;
	private float side;

	private Quaternion appearanceQuat;
	private Quaternion movementQuat = Quaternion.identity;

	// Use this for initialization
	void Start () {
		controller = gameObject.GetComponent<CharacterController>();
		resetGame ();
		GameStart ();
	}

	void resetGame () {
		moveVector = new Vector3(0,0,0);
		boostTimeLeft = 0f;
		boosts = 3;
		collisionFlags = CollisionFlags.None;
		appearanceQuat = Quaternion.identity;
	}

	void GameStart() {
		switchToRunFSM ();
	}
	
	void GameOver() {
		resetGame ();
	}



	void switchToRunFSM() {
		stateMachine.ChangeState (enterRUN, updateRUN, exitRUN);
	}
	
	void enterRUN() {}
	
	void updateRUN() {
		factorVelocityUp (Mathf.Pow(turnDecelPerSecond, Time.deltaTime));
		factorVelocitySide (Mathf.Pow(turnDecelPerSecond, Time.deltaTime));
		updateVelocityFwdWithMax (normalFwdAccel * Time.deltaTime, normalFwdMaxSpeed);
		updateVelocitySideWithMax (side * normalSidesAccel * Time.deltaTime, normalSidesMaxSpeed);
		updateVelocityUpWithMax (up * normalSidesAccel * Time.deltaTime, normalSidesMaxSpeed);

		if (Input.GetKeyDown(KeyCode.B) ) {
			triggerBoostRequest();
		}

		if (boostTimeLeft > 0) {
			switchToBoostFSM();
		}
	}
	
	void exitRUN () {}

	void triggerBoostRequest() {
		if (boosts>0) {
			boostTimeLeft+=boostTime;
			boosts--;
		}
	}

	void switchToBoostFSM() {
		stateMachine.ChangeState (enterBOOST, updateBOOST, exitBOOST);
	}
	
	void enterBOOST() {
	}
	
	void updateBOOST() {

		factorVelocityUp (Mathf.Pow(turnDecelPerSecond, Time.deltaTime));
		factorVelocitySide (Mathf.Pow(turnDecelPerSecond, Time.deltaTime));
		updateVelocityFwdWithMax (normalFwdAccel * Time.deltaTime * boostFwdAccelFactor, normalFwdMaxSpeed * boostFwdMaxSpeedFactor);
		updateVelocitySideWithMax (side * normalSidesAccel * Time.deltaTime * boostSidesAccelFactor, normalSidesMaxSpeed * boostSidesMaxSpeedFactor);
		updateVelocityUpWithMax (up * normalSidesAccel * Time.deltaTime * boostSidesAccelFactor, normalSidesMaxSpeed * boostSidesMaxSpeedFactor);
		boostTimeLeft -= Time.deltaTime;
		if (boostTimeLeft <= 0) {
			switchToRunFSM();
		}
	}
	
	void exitBOOST () {
		boostTimeLeft = 0;
	}




	// Update is called once per frame
	void Update () {

		applyMovementQuat (); //this assures any movment computed is relative to the identity Quaternion

		up = Input.GetAxis ("Vertical");
		side = Input.GetAxis ("Horizontal");

		stateMachine.Execute();

		collisionFlags = controller.Move(transform.InverseTransformDirection(moveVector) * Time.deltaTime);

		setAppearanceQuat (moveVector.x, moveVector.y);
		applyAppearanceQuat (); //now that we have safely moved, lets change to our appearance Quaternion for special effects (tilting)
	}


	//HELPER METHODS

	//TRANSFORMS
	void applyMovementQuat () {
		transform.rotation = movementQuat;
	}

	void applyAppearanceQuat () {
		transform.rotation = appearanceQuat;
	}

	void setAppearanceQuat (float x, float y) {
		appearanceQuat = Quaternion.Euler(new Vector3(y*-0.17f,0f,x*-0.17f));
	}


	//SIDE
	void updateVelocitySideWithMax (float xVec, float max) {
		moveVector.x += xVec;
		if (Mathf.Abs (moveVector.x) > max) {
			moveVector.x = Mathf.Sign(moveVector.x)*max;
		}
	}

	void updateVelocitySideWithMin (float xVec, float min) {
		moveVector.x += xVec;
		if (Mathf.Abs (moveVector.x) < min) {
			moveVector.x = Mathf.Sign(moveVector.x)*min;
		}
	}

	void factorVelocitySide (float factor) {
		moveVector.x = moveVector.x * factor;
	}

	//UP
	void updateVelocityUpWithMax (float yVec, float max) {
		moveVector.y += yVec;
		if (Mathf.Abs (moveVector.y) > max) {
			moveVector.y = Mathf.Sign(moveVector.y)*max;
		}
	}

	void updateVelocityUpWithMin (float yVec, float min) {
		moveVector.y += yVec;
		if (Mathf.Abs (moveVector.y) < min) {
			moveVector.y = Mathf.Sign(moveVector.y)*min;
		}
	}
	
	void factorVelocityUp (float factor) {
		moveVector.y = moveVector.y * factor;
	}

	//FWD
	void updateVelocityFwdWithMax (float zVec, float max) {
		moveVector.z += zVec;
		if (Mathf.Abs (moveVector.z) > max) {
			moveVector.z = Mathf.Sign(moveVector.z)*max;
		}
	}

	void updateVelocityFwdWithMin (float zVec, float min) {
		moveVector.z += zVec;
		if (Mathf.Abs (moveVector.z) < min) {
			moveVector.z = Mathf.Sign(moveVector.z)*min;
		}
	}

	void factorVelocityFwd (float factor) {
		moveVector.z = moveVector.z * factor;
	}
}
