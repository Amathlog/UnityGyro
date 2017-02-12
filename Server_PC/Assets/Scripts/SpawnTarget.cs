using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTarget : MonoBehaviour {

    [SerializeField] private GameObject sphereSpawn;
	private Rigidbody spawnedSphere;

    [SerializeField] private float forwardForce = 200.0f;
    [SerializeField] private float planeForce = 100.0f;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float ratio;
    private float invRatio;

	void Update(){
		if (spawnedSphere != null) {
		}
	}

	// Use this for initialization
	void Start () {
		Vector3 topLeft, bottomRight;
		topLeft = Camera.main.ScreenToWorldPoint (new Vector3 (0f, Camera.main.pixelHeight, 10f));
		bottomRight = Camera.main.ScreenToWorldPoint (new Vector3 (Camera.main.pixelWidth, 0f, 10f));
		minX = topLeft.x;
		maxX = bottomRight.x;
		minY = bottomRight.y;
		maxY = topLeft.y;
        ratio = (float)Screen.width / (float)Screen.height;
        invRatio = 2.0f / ratio;
	}
	
	public void Spawn(Vector3 position, Color color) {
		GameObject obj = Instantiate(sphereSpawn,Camera.main.transform.position + Vector3.down, Quaternion.identity);
        float xValue = ((position.x - minX) / (maxX - minX) - 0.5f) * 2.0f;
        float yValue = ((position.y - minY) / (maxY - minY) - 0.5f) * 2.0f;

        //obj.transform.position = position;
        obj.GetComponent<Rigidbody>().AddForce(Vector3.forward * forwardForce +  (Vector3.right * xValue * ratio + Vector3.up * yValue * invRatio) * planeForce);
        obj.GetComponent<Renderer>().material.color = color;
		spawnedSphere = obj.GetComponent<Rigidbody> ();
    }
}
