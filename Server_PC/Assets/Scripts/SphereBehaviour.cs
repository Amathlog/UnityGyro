using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereBehaviour : MonoBehaviour {
    [SerializeField] private GameObject plane;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name == plane.name) {
            GameObject.Destroy(this.gameObject);
        }
    }

}
