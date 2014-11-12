using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateEnvironment : MonoBehaviour {
	
	public GameObject astPrefab;
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

	List<GameObject> currentField;
	List<GameObject> nextField;
	List<GameObject> nextNextField;
	float currentStart;
	float fieldDist = 1500f;
	float ringDensity = 200f;
	float ringRadius= 600f;
	int numberPerRing = 25;

	// Use this for initialization
	void Start () {
		avatarScript = GameObject.Find ("Avatar").GetComponent<AvatarScript> ();
	}

	public void StartGame () {
		makeAstroidLine (); //just for now
		//updateMyVariables ();
		//the above must happen b4 anything else
		currentField = new List<GameObject> ();
		nextField = new List<GameObject> ();
		nextNextField = new List<GameObject> ();
		genAllFields (0f);

	}

	public void EndGame() {
		destroyAll();
	}

	void genAllFields (float z){
		currentStart = z;
		makeAstroidTunnel (z, z+fieldDist, ringDensity, currentField);
		makeAstroidTunnel (z+fieldDist, z+fieldDist*2, ringDensity, nextField);
		makeAstroidTunnel (z+fieldDist*2, z+fieldDist*3, ringDensity, nextNextField);
	}

	void genNextField () {
		currentStart += fieldDist;
		destroyList (currentField);
		currentField = nextField;
		nextField = nextNextField;
		nextNextField = new List<GameObject> ();
		makeAstroidTunnel (currentStart+fieldDist*2, currentStart+fieldDist*3, ringDensity, nextNextField);
	}

	void makeAstroidTunnel(float zstart, float zend, float rDensity, List<GameObject> list){
		for (float z = zstart; z<zend; z+=rDensity) {
			makeAstroidRing(0f, 0f, z, ringRadius, numberPerRing, list);
		}
	}

	void makeAstroidRing(float x, float y, float z, float radius, int num, List<GameObject> list){
		Vector3 center = new Vector3 (x, y, z);
		for (float theta = 0f; theta<2*Mathf.PI; theta+= Mathf.PI*2/num) {
			Vector3 myCenter = center;
			myCenter.x+= Mathf.Sin(theta)*radius;
			myCenter.y+= Mathf.Cos(theta)*radius;
			GameObject a = Instantiate (astPrefab, myCenter, Quaternion.identity) as GameObject;
			a.transform.localScale = new Vector3(90f,90f,90f);
			list.Add(a);
		}
	}

	void destroyAll() {
		destroyList(nextField);
		destroyList(nextNextField);
		destroyList(currentField);
	}

	void destroyList(List<GameObject> list) {
		foreach (GameObject obj in list) {
			Destroy(obj);
		}
	}

	// Update is called once per frame
	void Update () {
		updateMyVariables ();
		if (avatarZPos > currentStart + fieldDist) {
			genNextField();
		}
	}

	void updateMyVariables () {
		avatarZPos = avatarScript.getZPos ();
	}

	// TEMP
	void makeAstroidLine(){
		for (int z = 0; z<10000; z+=50) {
			GameObject a = Instantiate (astPrefab, new Vector3(0f,30f,z), Quaternion.identity) as GameObject;
			a.transform.localScale = new Vector3(10f,10f,10f);
		}
	}
}
