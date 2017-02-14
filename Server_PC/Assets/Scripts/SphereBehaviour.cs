﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereBehaviour : MonoBehaviour {
    [SerializeField] private GameObject plane;

    private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.layer == 9) {
			GameSceneManager.instance.AddPoints (200*(2-GameSceneManager.instance.speedMultiplier));
			EnemyMovementBehaviour temp = collision.gameObject.GetComponent<EnemyMovementBehaviour> ();
			temp.StopAllCoroutines ();
			temp.enabled = false;
			GameSceneManager.instance.myEnemiesComp.aliveEnemies.Remove (temp);
			if (GameSceneManager.instance.myEnemiesComp.aliveEnemies.Count == 0) {
				GameSceneManager.instance.myEnemiesComp.aliveEnemiesReadyForPattern = false;
			}
            GameSceneManager.instance.Explode(temp.transform.position);
			Destroy (collision.gameObject);
        }
		GameObject.Destroy(this.gameObject);
    }

}
