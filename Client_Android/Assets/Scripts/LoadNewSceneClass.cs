using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewSceneClass : MonoBehaviour {

	public void LoadSceneFunc() {
        SceneManager.LoadScene(1);
    }
}
