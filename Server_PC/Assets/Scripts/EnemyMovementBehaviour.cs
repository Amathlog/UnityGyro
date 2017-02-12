using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementBehaviour : MonoBehaviour {
	private Vector3 initialPos, myPositiveLimit, myNegativeLimit;
	public float translationTime = 3, translation, translationTimer = 0;
	private int sign = 1;
	public AnimationCurve xAnimCurve, yAnimCurve; 


	public void ResetAnimation(Vector3 positiveLimit, Vector3 negativeLimit, float t){
		initialPos = transform.position;
		translationTimer = 0;
		translationTime = t;
		myPositiveLimit = initialPos + positiveLimit;
		myNegativeLimit = initialPos + negativeLimit;
		myPositiveLimit.z = transform.position.z;
		myNegativeLimit.z = transform.position.z;
	}

	public void AnimateLoop(){
		translationTimer = Mathf.Clamp(translationTimer + Time.deltaTime*sign, 0, translationTime);
		Vector3 v3 = transform.position;
		v3.x = Mathf.Lerp(myPositiveLimit.x, myNegativeLimit.x, xAnimCurve.Evaluate(translationTimer / translationTime));
		v3.y = Mathf.Lerp (myPositiveLimit.y, myNegativeLimit.y, yAnimCurve.Evaluate (translationTimer / translationTime));
		transform.position = v3;
		if ((translationTimer >= translationTime)||(translationTimer <= 0)) {
			sign *= -1;
		}
	}

	public IEnumerator AnimateCoroutine(){
		float animationIterator = 0;
		while(animationIterator <= 1){
			animationIterator += 0.02f/translationTime;

		}
		yield break;
	}
}
