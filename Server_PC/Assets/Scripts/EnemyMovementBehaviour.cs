using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementBehaviour : MonoBehaviour {
	private Vector3 initialPos, source, destination;
	public float translationTime = 3, translation, translationTimer = 0, goToSourceTimer;
	private AnimationCurve xAnimCurve, yAnimCurve, zAnimCurve;
	private bool goToSource;


	public void ResetAnimation(Vector3 positiveLimit, Vector3 negativeLimit, float t, AnimationCurve acx, AnimationCurve acy, AnimationCurve acz){
		initialPos = transform.position;
		translationTime = t;
		source = initialPos + positiveLimit;
		destination = initialPos + negativeLimit;
		translationTimer = 0f;
		xAnimCurve = acx;
		yAnimCurve = acy;
		zAnimCurve = acz;
		if (initialPos != positiveLimit) {
			goToSource = true;
			goToSourceTimer = 0;
		}
	}

	public void AnimateLoop(){
		if (goToSource) {
			goToSourceTimer = Mathf.Clamp (goToSourceTimer + Time.deltaTime, 0, translationTime / 2);
			transform.position = Vector3.Lerp (initialPos, source, goToSourceTimer / (translationTime / 2));
			if (goToSourceTimer >= translationTime / 2) {
				goToSource = false;
			}
		} 
		else {
			if ((translationTimer >= translationTime)) {
				translationTimer = 0;
				Vector3 temp = source;
				source = destination;
				destination = temp;
			}

			translationTimer = Mathf.Clamp (translationTimer + Time.deltaTime, 0, translationTime);
			Vector3 v3 = Vector3.zero;
			v3.x = Mathf.Lerp (source.x, destination.x, xAnimCurve.Evaluate (translationTimer / translationTime));
			v3.y = Mathf.Lerp (source.y, destination.y, yAnimCurve.Evaluate (translationTimer / translationTime));
			v3.z = Mathf.Lerp (source.z, destination.z, zAnimCurve.Evaluate (translationTimer / translationTime));
			transform.position = v3;
		}
	}

	public IEnumerator AnimateCoroutine(){
		float animationIterator = 0;
		while(animationIterator <= 1){
			animationIterator += 0.02f/translationTime;
			Vector3 v3 = Vector3.zero;
			v3.x = Mathf.LerpUnclamped (source.x, destination.x, xAnimCurve.Evaluate (animationIterator));
			v3.y = Mathf.LerpUnclamped (source.y, destination.y, yAnimCurve.Evaluate (animationIterator));
			v3.z = Mathf.LerpUnclamped (source.z, destination.z, zAnimCurve.Evaluate (animationIterator));
			if (this != null) {
				transform.position = v3;
			}
			yield return new WaitForSeconds (0.02f);
		}
		yield break;
	}
}
