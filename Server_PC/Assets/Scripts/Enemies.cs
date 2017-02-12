using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour {
	public GameObject enemyPref;
	public List<EnemyMovementBehaviour> aliveEnemies = new List<EnemyMovementBehaviour>();
	public bool aliveEnemiesReadyForPattern = false;
	private Transform bossTransform;
	public EnemyType[] EnemiesMoveToPositionList, EnemiesPatternsList;

	[System.Serializable]
	public class EnemyType{
		public bool oneByOne = false;
		public Vector3 positiveLimit;
		public Vector3 negativeLimit;
		public float t;
		public AnimationCurve acx;
		public AnimationCurve acy;
		public AnimationCurve acz;
	}

	void Start(){
		bossTransform = GameObject.Find ("Boss").transform;
	}

	void Update(){
		if (aliveEnemiesReadyForPattern) {
			foreach (EnemyMovementBehaviour e in aliveEnemies) {
				e.AnimateLoop ();
			}
		}
	}

	//MAKE IT A POOL PATTERN FOROPT
	public void SpawnEnemies(int nmbrOfEnemies, int enemyTypeIndex){
		for (int i = 0; i < nmbrOfEnemies; i++) {
			GameObject go = Instantiate (enemyPref, transform.position + Vector3.forward, Quaternion.identity);
			go.transform.parent = transform;
			aliveEnemies.Add (go.GetComponent<EnemyMovementBehaviour>());
		}
		StartCoroutine (MoveToPositionAndStartPattern (enemyTypeIndex));
	}

	IEnumerator MoveToPositionAndStartPattern(int enemyTypeIndex)
	{
		EnemyType eT = EnemiesMoveToPositionList [enemyTypeIndex];
		for (int i = 0; i < aliveEnemies.Count; i++) {
			Vector3 v3 = Vector3.Lerp (eT.positiveLimit, eT.negativeLimit, i / (aliveEnemies.Count - 1f));
			Debug.Log (v3);
			aliveEnemies [i].ResetAnimation (Vector3.zero, v3 - aliveEnemies [i].transform.position, eT.t*GameSceneManager.instance.speedMultiplier, eT.acx, eT.acy, eT.acz);
			StartCoroutine(aliveEnemies[i].AnimateCoroutine());
		}
		yield return new WaitForSeconds(eT.t*2);
		EnemyType ePattern = EnemiesPatternsList [enemyTypeIndex];
		for (int i = 0; i < aliveEnemies.Count; i++) {
			if (ePattern.oneByOne) {
				if (i % 2 == 0) {
					aliveEnemies [i].ResetAnimation (ePattern.positiveLimit, ePattern.negativeLimit, ePattern.t*GameSceneManager.instance.speedMultiplier, ePattern.acx, ePattern.acy, ePattern.acz);
				} else {
					aliveEnemies [i].ResetAnimation (ePattern.negativeLimit, ePattern.positiveLimit, ePattern.t*GameSceneManager.instance.speedMultiplier, ePattern.acx, ePattern.acy, ePattern.acz);
				}
			} else {
				aliveEnemies [i].ResetAnimation (ePattern.positiveLimit, ePattern.negativeLimit, ePattern.t*GameSceneManager.instance.speedMultiplier, ePattern.acx, ePattern.acy, ePattern.acz);
			}
		}
		aliveEnemiesReadyForPattern = true;
		yield break;
	}
}
