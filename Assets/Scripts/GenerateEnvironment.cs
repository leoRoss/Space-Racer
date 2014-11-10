using UnityEngine;
using System.Collections;

public class GenerateEnvironment : MonoBehaviour {

	public GameObject astPrefab;
	// Use this for initialization
	void Start () {
		makeAstroidTunnel ();
		makeAstroidLine ();
	}

	void makeAstroidTunnel(){
		for (int z = 0; z<10000; z+=200) {
			makeAstroidRing(0f, 0f, z, 600f, 25);
		}
	}

	void makeAstroidLine(){
		for (int z = 0; z<10000; z+=50) {
			GameObject a = Instantiate (astPrefab, new Vector3(0f,10f,z), Quaternion.identity) as GameObject;
			a.transform.localScale = new Vector3(10f,10f,10f);
		}
	}

	void makeAstroidRing(float x, float y, float z, float radius, int num){
		Vector3 center = new Vector3 (x, y, z);
		for (float theta = 0f; theta<2*Mathf.PI; theta+= Mathf.PI*2/num) {
			Vector3 myCenter = center;
			myCenter.x+= Mathf.Sin(theta)*radius;
			myCenter.y+= Mathf.Cos(theta)*radius;
			GameObject a = Instantiate (astPrefab, myCenter, Quaternion.identity) as GameObject;
			a.transform.localScale = new Vector3(90f,90f,90f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
