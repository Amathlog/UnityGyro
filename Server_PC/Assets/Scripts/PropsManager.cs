using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsManager : MonoBehaviour {
	// Use this for initialization
	public float force = 60f;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Transform t in transform) {
			if (t.position.z > -15f) {
				t.Translate (Vector3.back * 0.1f, Space.World);
			} else {
				t.position += Vector3.forward * force;
			}
		}	
	}
}
