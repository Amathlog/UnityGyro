using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenLookAt : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Transform t in transform) {
			t.LookAt (Camera.main.transform, Camera.main.transform.up);
			Vector3 rot = t.rotation.eulerAngles;
			rot.x = 90;
			rot.z = 0;
			t.rotation = Quaternion.Euler (rot);
		}
	}
}
