using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateEnvironment : MonoBehaviour {
	
	public GameObject a1;
	public GameObject a2;
	public GameObject a3;
	public GameObject aboost;
	public GameObject ahealth;
	public GameObject abomb;
	public GameObject boostring;
	AvatarScript avatarScript;
	GameLogic gameLogic;
	Vector3 avatarPos;
	public GameObject wall;

	//xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
	//    >> 
	//xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
	//|--current--||---next---||--nextnext--|

	//soon as we hit next, we:
	//	1. delete all emelents in current
	//	2. move the current point to next and next to nextnext
	//	3. generate nextnext

	List<GameObject>[] rings;
	int numberOfRings = 13;
	//float boundRockSize = 200f;
	float obstRockSize = 200f;
	float speedRingSize= 100f;
	float ringDist = 320f;
	float ringRadius= 700f;
	//int numberPerRing = 20;
	float obstSpace = 350f; // Vertical/horizontal space between asteroid objects
	float noiseThresh = 0.55f; // Perlin noise value must be greater than this for an asteroid to be placed at that point
	float bombRadius = 700f;
	float currentStart;
	float wallLength;

	// Use this for initialization
	void Start () {
		avatarScript = GameObject.Find ("Avatar").GetComponent<AvatarScript> ();
		gameLogic = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
	}

	public void StartGame () {
		//makeAstroidLine (); //just for now
		//updateMyVariables ();
		//the above must happen b4 anything else
		setUpRings ();
		genAllRings (0f);
		Debug.Log ("course length" + gameLogic.courseLength);
		//makeAsteroidField (0f, 0f, 3200f, ringRadius, rings [10]);

	}

	public void setUpRings() {
		rings = new List<GameObject> [numberOfRings];
		for (int i=0; i<rings.Length; i++) {
			rings[i] = new List<GameObject> ();
		}
	}

	public void EndGame() {
		destroyAll();
	}

	void genAllRings (float z){
		currentStart = z;
		for (int i=4; i<rings.Length; i++) {
			makeAstroidRing (currentStart+ringDist*i, ringRadius, rings[i]);
		}
	}

	void genNextField () {
		currentStart += ringDist;
		destroyList (rings[0]);
		int len = rings.Length;
		for (int i=0; i<len-1; i++) {
			rings[i]=rings[i+1];
		}
		rings[len-1] = new List<GameObject> ();
		makeAstroidRing (currentStart+ringDist*(len-1), ringRadius, rings[len-1]);
	}

	void makeAstroidRing(float z, float radius, List<GameObject> list){
		if (z <= gameLogic.courseLength) {
				makeAsteroidField (z, radius, list);
		}
	}

	void makeAsteroidField(float z, float radius, List<GameObject> list) {
		for (float hor = -1f*radius; hor < radius; hor += obstSpace) {
			for (float ver = -1f*radius; ver < radius; ver += obstSpace) {
				float randSeed = Random.Range (-1.0f, 1.0f); // So that we use a different spot on the perlin noise plane for each ring
				float noise = Mathf.PerlinNoise((hor + radius)/(radius) + randSeed, (ver + radius)/(radius) + randSeed);
				float offset = Random.Range (-1.0f*((obstSpace - 2*obstRockSize)/2.0f), ((obstSpace - 2*obstRockSize)/2.0f)); // So that asteroids don't line up like a grid
				//Debug.Log (noise);
				if (noise > noiseThresh) {
					GameObject pref = randomType();
					GameObject a = Instantiate (pref, new Vector3(hor + offset, ver + offset, z), Quaternion.identity) as GameObject;
					if (pref!=boostring) {
						a.transform.localScale = new Vector3(obstRockSize, obstRockSize, obstRockSize);
					}
					else {
						a.transform.localScale = new Vector3(speedRingSize, speedRingSize, speedRingSize);
					}
					list.Add(a);

				}
			}
		}
	}

	GameObject randomType () {
		float f = Random.Range (0f, 100f);
		if (f > 11) {
			if (f < 40) {
				return a3;
			}
			if (f> 70) {
				return a2;
			}
			return a1;
		}
		if (f<3) return boostring; 
		if (f<6) return abomb;
		if (f<9) return aboost;
		return ahealth;
	}

	void destroyAll() {
		for (int i=0; i<rings.Length; i++) {
			destroyList(rings[i]);
		}
	}

	void destroyList(List<GameObject> list) {
		foreach (GameObject obj in list) {
			if (obj!=null) {
				Destroy(obj);
			}
		}
	}

	public void BombsAway () {
		for (int i=1; i<5; i++) { 
			if (rings[i]!=null) {
				foreach (GameObject ast in rings[i]) {
					if (ast!=null) {
						if (inBombRadius(ast.transform.position, i)) {
							bombDestroy(ast);
						}
					}
				}
			}
		}
	}

	void bombDestroy (GameObject ast){
		if (ast.tag == "Boost") {
			avatarScript.AddBoost();
		}
		if (ast.tag == "Bomb") {
			avatarScript.AddBomb();
		}
		if (ast.tag == "Health") {
			gameLogic.AddHealth();
		}
		Destroy(ast);
	}

	bool inBombRadius (Vector3 p, int i) {
		if (Mathf.Abs (p.x - avatarPos.x) < 380-40*i && Mathf.Abs (p.y - avatarPos.y) < 380-40*i) {
			return true;
		}
		return false;
	}


	// Update is called once per frame
	void Update () {
		updateMyVariables ();
		if (avatarPos.z > currentStart + ringDist) {
			genNextField();
		}
	}

	void updateMyVariables () {
		avatarPos = avatarScript.getPos ();
	}



	// TEMP
//	void makeAstroidLine(){
//		for (float x = -1*ringRadius; x < ringRadius; x+=100) {
//			GameObject a = Instantiate (a1, new Vector3(x, 30f, 3200f), Quaternion.identity) as GameObject;
//			a.transform.localScale = new Vector3(10f, 10f, 10f);
//		}
//		for (int z = 0; z<10000; z+=50) {
//			//GameObject a = Instantiate (weakPrefab1, new Vector3(0f,30f,z), Quaternion.identity) as GameObject;
//			//a.transform.localScale = new Vector3(10f,10f,10f);
//		}
//	}

	//		Vector3 center = new Vector3 (x, y, z);
	//		for (float theta = 0f; theta<2*Mathf.PI; theta+= Mathf.PI*2/num) {
	//			Vector3 myCenter = center;
	//			myCenter.x+= Mathf.Sin(theta)*radius;
	//			myCenter.y+= Mathf.Cos(theta)*radius;
	//			//GameObject a = Instantiate (strongPrefab1, myCenter, Quaternion.identity) as GameObject;
	//			//a.transform.localScale = new Vector3(boundRockSize,boundRockSize,boundRockSize);
	//			//list.Add(a);
	//		}

	/*void makeWalls(float zStart) {
		Vector3 center = new Vector3 (0f, 0f, zStart);
		for (int i=0; i<8; i++) {
			Vector3 myCenter = center;
			myCenter.x+= Mathf.Sin(theta)*radius;
			myCenter.y+= Mathf.Cos(theta)*radius;
			float angle = i*45;
			GameObject w = Instantiate (wall, 
		}
	}*/
}
