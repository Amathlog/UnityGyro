﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {
	EnemyMovementBehaviour myMovementComp;
	public Vector3 positiveLimit, negativeLimit;
	public float time;
	// Use this for initialization
	void Start () {
		myMovementComp = GetComponent<EnemyMovementBehaviour> ();
		myMovementComp.ResetAnimation (positiveLimit, negativeLimit, time);
	}
	
	// Update is called once per frame
	void Update () {
		myMovementComp.AnimateLoop ();
	}
}
