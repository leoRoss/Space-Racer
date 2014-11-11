﻿using UnityEngine;
using System.Collections;

public class DuckFollow : MonoBehaviour {
	public GameObject player;
	Vector3 relativeCamPos = new Vector3(0f,1f,-2f);
	Vector3 lookAtVector = new Vector3(0f,0f,0f);
	Vector3 [] playerPos;
	Vector3 [] camPos;
	
	float cameraCatchUpFactor = 0.8f;
	float playerCatchUpFactor = 0.8f;
	// Use this for initialization
	void Start () {
		playerPos = new Vector3[10];
		camPos = new Vector3[10];
		resetArrays ();
		transform.position = camPos [9];
		transform.LookAt (playerPos [9]);
	}
	
	// Update is called once per frame
	void Update () {
		updateArray (playerPos, player.transform.TransformPoint (lookAtVector), playerCatchUpFactor);
		updateArray (camPos, player.transform.TransformPoint (relativeCamPos), cameraCatchUpFactor);
		transform.position = camPos [9];
		transform.LookAt (playerPos [9]);
	}
	
	void resetArrays() {
		fillArray(playerPos, player.transform.TransformPoint (lookAtVector));
		fillArray(camPos, player.transform.TransformPoint (relativeCamPos));
	}
	
	void fillArray(Vector3 [] a, Vector3 pos) {
		for (int i=0; i<a.Length; i++) {
			a[i] = pos;
		}
	}
	
	void updateArray (Vector3 [] a, Vector3 motherGoose, float adjustmentFactor) {
		a [0] = halfWayPoint (a [0], motherGoose, adjustmentFactor);
		for (int i=1; i<a.Length; i++) {
			a[i]=halfWayPoint(a[i], a[i-1], adjustmentFactor);
		}
	}
	
	Vector3 halfWayPoint (Vector3 a, Vector3 b, float adjustmentFactor){
		return a + (b - a) * adjustmentFactor;
	}
}
