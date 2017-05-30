using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lantern : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
	}

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log(arg0.name + " this scene has been loaded!");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
