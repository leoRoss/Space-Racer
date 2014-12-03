using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

public class AvatarScript : MonoBehaviour {
	
	private CharacterController controller;
	private Vector3 moveVector;
	
	private ImmediateStateMachine stateMachine = new ImmediateStateMachine ();
	private CollisionFlags collisionFlags;
	
	private float normalFwdAccel = 270f;
	private float normalSideAccel = 370f;
	private float normalUpAccel = 250f;
	private float normalFwdMaxSpeed = 450f;
	private float normalSidesMaxSpeed = float.MaxValue; //unlimited
	
	private float boostFwdAccelFactor = 6.0f;
	private float boostSidesAccelFactor = 1.6f;
	private float boostFwdMaxSpeedFactor = 2.5f;
	private float boostSidesMaxSpeedFactor = 1f; //already unlimited
	
	private float turnDecelPerSecond = 0.05f; //after one second, only 0.05 of you side or up moment are left if you stop pressing button
	private float naturalDecelFromBoostPerSecond = 0.3f;
	
	public static float boostTime = 3.2f; //each boost lasts 3.2 seconds

	public static bool invert = false;
	
	public ParticleSystem engine;
	public ParticleSystem stars;
	public ParticleSystem bigStars;
	
	private int boosts;
	private float boostTimeLeft;
	private int bullets;
	
	private float up;
	private float side;

	private Quaternion appearanceQuat;
	private Quaternion movementQuat = Quaternion.identity;
	
	GameLogic gameLogicScript;
	GenerateEnvironment genEnv;
	private GameObject spaceBack;
	
	// Use this for initialization
	void Start () {
		gameLogicScript = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
		genEnv = GameObject.Find ("EnvironmentGenerator").GetComponent<GenerateEnvironment> ();
		controller = gameObject.GetComponent<CharacterController>();
		spaceBack = GameObject.Find ("Space");
		spaceBack.transform.localScale = new Vector3 (300f, 1f, 300f);
	}
	
	public void StartGame() {
		resetGameVariables ();
		switchToRunFSM ();
	}
	
	void resetGameVariables () {
		moveVector = new Vector3(0,0,0);
		boostTimeLeft = 0f;
		boosts = 5;
		bullets = 0;
		collisionFlags = CollisionFlags.None;
		appearanceQuat = Quaternion.identity;
		transform.position = new Vector3 (0f, 0f, 0f);
	}
	
	public void EndGame() {
		resetGameVariables ();
	}

	public void pauseGame() {
		this.gameObject.SetActive (false);
	}

	public void unPauseGame() {
		this.gameObject.SetActive (true);
	}
	
	void switchToRunFSM() {
		stateMachine.ChangeState (enterRUN, updateRUN, exitRUN);
	}
	
	void enterRUN() {}
	
	void updateRUN() {
		updateMotion(normalFwdAccel, normalSideAccel, normalUpAccel, 
		             naturalDecelFromBoostPerSecond, turnDecelPerSecond, turnDecelPerSecond,
		             normalFwdMaxSpeed,  normalSidesMaxSpeed, normalSidesMaxSpeed);
		
		
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
		this.audio.Play ();
		engine.startColor = new Color (200, 0, 255);
		engine.startSize = 7;
	}
	
	void updateBOOST() {
		updateMotion(normalFwdAccel*boostFwdAccelFactor, normalSideAccel*boostSidesAccelFactor, normalUpAccel*boostSidesAccelFactor, 
		             naturalDecelFromBoostPerSecond, turnDecelPerSecond, turnDecelPerSecond,
		             normalFwdMaxSpeed*boostFwdMaxSpeedFactor,  normalSidesMaxSpeed*boostFwdMaxSpeedFactor, normalSidesMaxSpeed*boostFwdMaxSpeedFactor);
		
		boostTimeLeft -= Time.deltaTime;
		if (boostTimeLeft <= 0) {
			switchToRunFSM();
		}
	}
	
	void exitBOOST () {
		boostTimeLeft = 0;
		engine.startColor = new Color (150, 50, 50);
		engine.startSize = 4;
	}

	//when i press B
	void triggerBoostRequest() {
		if (boosts>0) {
			boostTimeLeft+=boostTime;
			boosts--;
		}
	}
	
	//when I hit a free boost ring
	public void addFreeBoostTime (float time) {
		boostTimeLeft += time;
	}
	
	
	
	// Update is called once per frame
	void Update () {

		stars.transform.position = getPos();
		bigStars.transform.position = getPos();

		spaceBack.transform.position = new Vector3 (0f, 0f, Mathf.Min (transform.position.z + 2800f,20050f));
		applyMovementQuat (); //this assures any movment computed is relative to the identity Quaternion
		
		up = Input.GetAxis ("Vertical");
		side = Input.GetAxis ("Horizontal");

		if(invert)
		up = -up;
		
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
	
	
	public void updateMotion(float aFwd, float aUp, float aSide, 
	                         float decelFwd, float decelUp, float decelSide,
	                         float maxFwd, float maxUp, float maxSide) {
		
		//Fwd
		if (moveVector.z>maxFwd) {
			moveVector.z = maxFwd+(moveVector.z-maxFwd)*Mathf.Pow (decelFwd, Time.deltaTime);
		}
		else {
			moveVector.z = Mathf.Min(moveVector.z + aFwd*Time.deltaTime, maxFwd);
		}
		
		//Side
		if (Mathf.Abs(side)<0.1 || moveVector.x*side<0f) { //decel out of this direction
			moveVector.x = moveVector.x*Mathf.Pow(decelSide, Time.deltaTime);
		}
		if (Mathf.Abs(side)>0.1) { //time to accelerate
			moveVector.x = moveVector.x+(aSide*side)*Time.deltaTime;
		}
		
		//Side
		if (Mathf.Abs(up)<0.1 || moveVector.y*up<0f) { //decel out of this direction
			moveVector.y = moveVector.y*Mathf.Pow(decelUp, Time.deltaTime);
		}
		if (Mathf.Abs(up)>0.1) { //time to accelerate
			moveVector.y = moveVector.y+(aUp*up)*Time.deltaTime;
		}
		
		
	}
	
	
	//TRANSFORMS
	void applyMovementQuat () {
		transform.rotation = movementQuat;
	}
	
	void applyAppearanceQuat () {
		transform.rotation = appearanceQuat;
	}
	
	void setAppearanceQuat (float x, float y) {
		float yangle = y * -0.2f;
		float xangle = x * -0.2f;
		float ysign = Mathf.Sign (yangle);
		float xsign = Mathf.Sign (xangle);
		yangle = ysign * Mathf.Min (Mathf.Abs (yangle), 75f);
		xangle = xsign * Mathf.Min (Mathf.Abs (xangle), 75f);
		appearanceQuat = Quaternion.Euler(new Vector3(yangle,0f,xangle));
	}


	public void AddBoost (){
		boosts = boosts + 1;
	}
	public void AddBomb (){
		bullets = bullets + 1;
	}
	
//	//SIDE
//	void updateVelocitySideWithMax (float xVec, float max) {
//		if (Mathf.Abs (moveVector.x) > max) {
//			moveVector.x = moveVector.x * Mathf.Pow (naturalDecelFromBoostPerSecond, Time.deltaTime);
//		} else if (Mathf.Abs (moveVector.x) < max) {
//			moveVector.x = Mathf.Min(moveVector.x+xVec,max);
//		} 
//	}
//	
//	void updateVelocitySideWithMin (float xVec, float min) {
//		moveVector.x += xVec;
//		if (Mathf.Abs (moveVector.x) < min) {
//			moveVector.x = Mathf.Sign(moveVector.x)*min;
//		}
//	}
//	
//	void factorVelocitySide (float factor) {
//		moveVector.x = moveVector.x * factor;
//	}
//	
//	//UP
//	void updateVelocityUpWithMax (float yVec, float max) {
//		if (Mathf.Abs (moveVector.y) > max) {
//			moveVector.y = moveVector.y * Mathf.Pow (naturalDecelFromBoostPerSecond, Time.deltaTime);
//		} else if (Mathf.Abs (moveVector.y) < max) {
//			moveVector.y = Mathf.Min(moveVector.y+yVec,max);
//		} 
//	}
//	
//	void updateVelocityUpWithMin (float yVec, float min) {
//		moveVector.y += yVec;
//		if (Mathf.Abs (moveVector.y) < min) {
//			moveVector.y = Mathf.Sign(moveVector.y)*min;
//		}
//	}
//	
//	void factorVelocityUp (float factor) {
//		moveVector.y = moveVector.y * factor;
//	}
//	
//	//FWD
//	void updateVelocityFwdWithMax (float zVec, float max) {
//		if (Mathf.Abs (moveVector.z) > max) {
//			moveVector.z = moveVector.z * Mathf.Pow (naturalDecelFromBoostPerSecond, Time.deltaTime);
//		} else if (Mathf.Abs (moveVector.z) < max) {
//			moveVector.z = Mathf.Min(moveVector.z+zVec,max);
//		} 
//	}
//	
//	void updateVelocityFwdWithMin (float zVec, float min) {
//		moveVector.z += zVec;
//		if (Mathf.Abs (moveVector.z) < min) {
//			moveVector.z = Mathf.Sign(moveVector.z)*min;
//		}
//	}
//	
//	void factorVelocityFwd (float factor) {
//		moveVector.z = moveVector.z * factor;
//	}

	
	//GETTERS
	public int getBoosts () {return boosts;}
	public float getBoostTimeLeft () {return boostTimeLeft;}
	public int getBullets () {return bullets;}
	public float getZPos () {return transform.position.z; }
	public Vector3 getPos () {return transform.position; }
}
