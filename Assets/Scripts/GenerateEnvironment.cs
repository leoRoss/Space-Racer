﻿using UnityEngine;
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
	public GameLogic gameLogic;
	public float avatarZPos;
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
	float boundRockSize = 200f;
	float obstRockSize = 200f;
	float ringDist = 320f;
	float ringRadius= 700f;
	int numberPerRing = 20;
	float obstSpace = 350f; // Vertical/horizontal space between asteroid objects
	float noiseThresh = 0.55f; // Perlin noise value must be greater than this for an asteroid to be placed at that point

	float currentStart;
	float wallLength;

	// Use this for initialization
	void Start () {
		avatarScript = GameObject.Find ("Avatar").GetComponent<AvatarScript> ();
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

	public void EndGame() {
		destroyAll();
	}

	void genAllRings (float z){
		currentStart = z;
		for (int i=0; i<rings.Length; i++) {
			makeAstroidRing (0f, 0f, currentStart+ringDist*i, ringRadius, numberPerRing, rings[i]);
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
		makeAstroidRing (0f, 0f, currentStart+ringDist*(len-1), ringRadius, numberPerRing, rings[len-1]);
	}

	void makeAstroidRing(float x, float y, float z, float radius, int num, List<GameObject> list){
//		Vector3 center = new Vector3 (x, y, z);
//		for (float theta = 0f; theta<2*Mathf.PI; theta+= Mathf.PI*2/num) {
//			Vector3 myCenter = center;
//			myCenter.x+= Mathf.Sin(theta)*radius;
//			myCenter.y+= Mathf.Cos(theta)*radius;
//			//GameObject a = Instantiate (strongPrefab1, myCenter, Quaternion.identity) as GameObject;
//			//a.transform.localScale = new Vector3(boundRockSize,boundRockSize,boundRockSize);
//			//list.Add(a);
//		}
		if (z <= gameLogic.courseLength) {
				makeAsteroidField (x, y, z, radius, list);
		}
	}

	void makeAsteroidField(float x, float y, float z, float radius, List<GameObject> list) {
		for (float hor = x - radius; hor < x + radius; hor += obstSpace) {
			for (float ver = y - radius; ver < y + radius; ver += obstSpace) {
				float randSeed = Random.Range (-1.0f, 1.0f); // So that we use a different spot on the perlin noise plane for each ring
				float noise = Mathf.PerlinNoise((hor + radius)/(radius) + randSeed, (ver + radius)/(radius) + randSeed);
				float offset = Random.Range (-1.0f*obstSpace, obstSpace); // So that asteroids don't line up like a grid
				//Debug.Log (noise);
				if (noise > noiseThresh) {
					GameObject a = Instantiate (a1, new Vector3(hor + offset, ver + offset, z), Quaternion.identity) as GameObject;
					a.transform.localScale = new Vector3(obstRockSize, obstRockSize, obstRockSize);
					list.Add(a);
				}
			}
		}
	}

	void destroyAll() {
		for (int i=0; i<rings.Length; i++) {
			destroyList(rings[i]);
		}
	}

	void destroyList(List<GameObject> list) {
		foreach (GameObject obj in list) {
			Destroy(obj);
		}
	}

	// Update is called once per frame
	void Update () {
		updateMyVariables ();
		if (avatarZPos > currentStart + ringDist) {
			genNextField();
		}
	}

	void updateMyVariables () {
		avatarZPos = avatarScript.getZPos ();
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
}
