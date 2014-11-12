using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateEnvironment : MonoBehaviour {
	
	public GameObject strongPrefab1;
	public GameObject weakPrefab1;
	AvatarScript avatarScript;
	public float avatarZPos;

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
	float rockSize = 200f;
	float ringDist = 320f;
	float ringRadius= 700f;
	int numberPerRing = 20;

	float currentStart;

	// Use this for initialization
	void Start () {
		avatarScript = GameObject.Find ("Avatar").GetComponent<AvatarScript> ();
	}

	public void StartGame () {
		makeAstroidLine (); //just for now
		//updateMyVariables ();
		//the above must happen b4 anything else
		setUpRings ();
		genAllRings (0f);

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
		Vector3 center = new Vector3 (x, y, z);
		for (float theta = 0f; theta<2*Mathf.PI; theta+= Mathf.PI*2/num) {
			Vector3 myCenter = center;
			myCenter.x+= Mathf.Sin(theta)*radius;
			myCenter.y+= Mathf.Cos(theta)*radius;
			GameObject a = Instantiate (strongPrefab1, myCenter, Quaternion.identity) as GameObject;
			a.transform.localScale = new Vector3(rockSize,rockSize,rockSize);
			list.Add(a);
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
	void makeAstroidLine(){
		for (int z = 0; z<10000; z+=50) {
			GameObject a = Instantiate (weakPrefab1, new Vector3(0f,30f,z), Quaternion.identity) as GameObject;
			a.transform.localScale = new Vector3(10f,10f,10f);
		}
	}
}
