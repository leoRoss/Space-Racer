using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

public class AvatarScript : MonoBehaviour {

	private CharacterController controller;
	private Vector3 moveVector;

	private ImmediateStateMachine stateMachine = new ImmediateStateMachine ();
	private CollisionFlags collisionFlags;

	private float normalFwdAccel = 250f;
	private float normalSidesAccel = 700f;
	private float normalFwdMaxSpeed = 750f;
	private float normalSidesMaxSpeed = 300f;

	private float boostFwdAccelFactor = 6.0f;
	private float boostSidesAccelFactor = 2.0f;
	private float boostFwdMaxSpeedFactor = 1.5f;
	private float boostSidesMaxSpeedFactor = 1.3f;

	private float turnDecelPerSecond = 0.05f; //after one second, only 0.05 of you side or up moment are left if you stop pressing button
	private float naturalDecelFromBoostPerSecond = 0.3f;

	public static float boostTime = 3.2f; //each boost lasts 3.2 seconds

	private int boosts;
	private float boostTimeLeft;
	private int bullets;
	
	private float up;
	private float side;

	private Quaternion appearanceQuat;
	private Quaternion movementQuat = Quaternion.identity;

	GameLogic gameLogicScript;
	GenerateEnvironment genEnv;

	// Use this for initialization
	void Start () {
		gameLogicScript = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
		genEnv = GameObject.Find ("EnvironmentGenerator").GetComponent<GenerateEnvironment> ();
		controller = gameObject.GetComponent<CharacterController>();
	}

	public void StartGame() {
		resetGameVariables ();
		switchToRunFSM ();
	}

	void resetGameVariables () {
		moveVector = new Vector3(0,0,0);
		boostTimeLeft = 0f;
		boosts = 3;
		bullets = 0;
		collisionFlags = CollisionFlags.None;
		appearanceQuat = Quaternion.identity;
		transform.position = new Vector3 (0f, 0f, 0f);
	}
	
	public void EndGame() {}
		
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
	



	public void AvatarCollidedWithStrongAstroid (){
		moveVector.z = moveVector.z * 0.2f;
		moveVector.y = moveVector.y*-1.0f;
		moveVector.x = moveVector.x*-1.2f; 
		moveTowardsCenter ();	
	}

	public void moveTowardsCenter () {
		Vector3 temp = transform.position;
		temp.x = temp.x * 0.95f;
		temp.y = temp.y * 0.95f;
		transform.position = temp;
	}

	public void AvatarCollidedWithWeakAstroid (){
		moveVector.z = moveVector.z*0.5f; //slow down, keep going
	}



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
		if (Mathf.Abs (moveVector.x) > max) {
			moveVector.x = moveVector.x * Mathf.Pow (naturalDecelFromBoostPerSecond, Time.deltaTime);
		} else if (Mathf.Abs (moveVector.x) < max) {
			moveVector.x = Mathf.Min(moveVector.x+xVec,max);
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
		if (Mathf.Abs (moveVector.y) > max) {
			moveVector.y = moveVector.y * Mathf.Pow (naturalDecelFromBoostPerSecond, Time.deltaTime);
		} else if (Mathf.Abs (moveVector.y) < max) {
			moveVector.y = Mathf.Min(moveVector.y+yVec,max);
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
		if (Mathf.Abs (moveVector.z) > max) {
			moveVector.z = moveVector.z * Mathf.Pow (naturalDecelFromBoostPerSecond, Time.deltaTime);
		} else if (Mathf.Abs (moveVector.z) < max) {
			moveVector.z = Mathf.Min(moveVector.z+zVec,max);
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



	//KEITHS PLAY BOX

	//when i press B
	void triggerBoostRequest() {
		if (boosts>0) {
			boostTimeLeft+=boostTime;
			boosts--;
		}
	}
	
	//when I hit a free boost ring
	void addFreeBoostTime () {
		boostTimeLeft += boostTime;
	}




	//GETTERS
	public int getBoosts () {return boosts;}
	public float getBoostTimeLeft () {return boostTimeLeft;}
	public int getBullets () {return bullets;}
	public float getZPos () {return transform.position.z; }
}
