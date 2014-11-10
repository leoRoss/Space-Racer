using UnityEngine;
using System.Collections;

public class GenerateEnvironment : MonoBehaviour {

	public GameObject astPrefab;
	// Use this for initialization
	void Start () {
		makeAstroidLineForLeo ();
	}

	void makeAstroidLineForLeo(){
		for (int z = 0; z<10000; z+=200) {
			GameObject a = Instantiate (astPrefab, new Vector3(20f,5f,z*1f), Quaternion.identity) as GameObject;
			GameObject b = Instantiate (astPrefab, new Vector3(-40f,5f,z*1f), Quaternion.identity) as GameObject;
			a.transform.localScale = new Vector3(10f,10f,10f);
			b.transform.localScale = new Vector3(20f,20f,20f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
