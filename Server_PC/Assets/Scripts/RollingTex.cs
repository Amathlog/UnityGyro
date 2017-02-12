using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingTex : MonoBehaviour {
	Material[] myMats;

	void Start () {
		myMats = GetComponent<MeshRenderer> ().materials;
	}
	// Update is called once per frame
	void Update () {
		myMats [0].SetTextureOffset ("_MainTex", myMats [0].GetTextureOffset ("_MainTex") + Vector2.up * -0.01f);
	}
}
