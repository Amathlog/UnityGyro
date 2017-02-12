using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereBehaviour : MonoBehaviour {
    [SerializeField] private GameObject plane;

    private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.layer == 9) {
			GameSceneManager.instance.AddPoints (10);
			EnemyMovementBehaviour temp = collision.gameObject.GetComponent<EnemyMovementBehaviour> ();
			temp.StopAllCoroutines ();
			temp.enabled = false;
			GameSceneManager.instance.myEnemiesComp.aliveEnemies.Remove (temp);
			if (GameSceneManager.instance.myEnemiesComp.aliveEnemies.Count == 0) {
				GameSceneManager.instance.myEnemiesComp.aliveEnemiesReadyForPattern = false;
			}
			Destroy (collision.gameObject,0.1f);
        }
		GameObject.Destroy(this.gameObject);
    }

}
