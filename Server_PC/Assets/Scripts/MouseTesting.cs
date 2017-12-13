using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTesting : MonoBehaviour {

    public Vector3 position;
    public bool fire = false;
	
	// Update is called once per frame
	void Update () {
        // Getting mouse position
        position = Input.mousePosition;

        // Restraint the position into the screen view
        position.x = Mathf.Clamp(position.x, 0, Screen.width);
        position.y = Mathf.Clamp(position.y, 0, Screen.height);
        position.z = 3.0f;

        //// Get a value between -1 and 1
        //position.x = ((position.x / Screen.width) - 0.5f) * 2.0f;
        //position.y = ((position.y / Screen.height) - 0.5f) * 2.0f;

        if(Input.GetButtonDown("Fire1"))
            fire = true;
    }
}
