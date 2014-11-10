using UnityEngine;
using System.Collections;

public class GenerateEnvironment : MonoBehaviour {

	public GameObject astPrefab;
	// Use this for initialization
	void Start () {
		makeAstroidTunnel ();
	}

	void makeAstroidTunnel(){
		for (int z = 0; z<10000; z+=200) {
			makeAstroidRing(0f, 0f, z, 300f, 30);
		}
	}

	void makeAstroidRing(float x, float y, float z, float radius, int num){
		Vector3 center = new Vector3 (x, y, z);
		for (float theta = 0f; theta<2*Mathf.PI; theta+= Mathf.PI*2/num) {
			Vector3 myCenter = center;
			myCenter.x+= Mathf.Sin(theta)*radius;
			myCenter.y+= Mathf.Cos(theta)*radius;
			GameObject a = Instantiate (astPrefab, myCenter, Quaternion.identity) as GameObject;
			a.transform.localScale = new Vector3(30f,30f,30f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
