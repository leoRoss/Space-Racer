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

	private float boostFwdAccelFactor = 1.3f;
	private float boostSidesAccelFactor = 1.3f;
	private float boostFwdMaxSpeedFactor = 1.7f;
	private float boostSidesMaxSpeedFactor = 1.7f;

	private float maxBoostSpeed = 30f; //added speed for pick up boost
	private float boostAddTime = 7f; //each boost adds 7 seconds


	private float boostAccel = 5f;
	private float boostTimeLeft;

	private float up;
	private float side;

	// Use this for initialization
	void Start () {
		controller = gameObject.GetComponent<CharacterController>();
		resetGame ();
		GameStart ();
	}

	void resetGame () {
		moveVector = new Vector3(0,0,0);
		boostTimeLeft = 0f;
		collisionFlags = CollisionFlags.None;
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
		factorVelocityUp (Mathf.Pow(0.05f, Time.deltaTime));
		factorVelocitySide (Mathf.Pow(0.05f, Time.deltaTime));
		updateVelocityFwdWithMax (normalFwdAccel * Time.deltaTime, normalFwdMaxSpeed);
		updateVelocitySideWithMax (side * normalSidesAccel * Time.deltaTime, normalSidesMaxSpeed);
		updateVelocityUpWithMax (up * normalSidesAccel * Time.deltaTime, normalSidesMaxSpeed);
	}
	
	void exitRUN () {}




	// Update is called once per frame
	void Update () {
		up = Input.GetAxis ("Vertical");
		side = Input.GetAxis ("Horizontal");

		stateMachine.Execute();

		collisionFlags = controller.Move(transform.InverseTransformDirection(moveVector) * Time.deltaTime);
	}


	//HELPER METHODS

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
