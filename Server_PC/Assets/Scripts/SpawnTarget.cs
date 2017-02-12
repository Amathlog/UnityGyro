using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTarget : MonoBehaviour {

    [SerializeField] private Transform targetUpLeft;
    [SerializeField] private Transform targetBotRight;

    [SerializeField] private GameObject sphereSpawn;

    [SerializeField] private float forwardForce = 200.0f;
    [SerializeField] private float planeForce = 100.0f;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float ratio;
    private float invRatio;

    

	// Use this for initialization
	void Start () {
        minX = targetUpLeft.position.x;
        maxX = targetBotRight.position.x;
        minY = targetBotRight.position.y;
        maxY = targetUpLeft.position.y;
        ratio = (float)Screen.width / (float)Screen.height;
        invRatio = 2.0f / ratio;
	}
	
	public void Spawn(Vector3 position, Color color) {
        GameObject obj = Instantiate(sphereSpawn);
        float xValue = ((position.x - minX) / (maxX - minX) - 0.5f) * 2.0f;
        float yValue = ((position.y - minY) / (maxY - minY) - 0.5f) * 2.0f;
        //obj.transform.position = position;
        obj.GetComponent<Rigidbody>().AddForce(Vector3.forward * forwardForce +  (Vector3.right * xValue * ratio + Vector3.up * yValue * invRatio) * planeForce);
        obj.GetComponent<Renderer>().material.color = color;
    }
}
