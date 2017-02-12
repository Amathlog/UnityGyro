using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsManager : MonoBehaviour {
	// Use this for initialization
	public float force = 1f,getBackValue = 60f;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Transform t in transform) {
			if (t.position.z > -15f) {
				t.Translate (Vector3.back *force * Time.deltaTime, Space.World);
			} else {
				t.position += Vector3.forward * getBackValue;
			}
		}	
	}
}
