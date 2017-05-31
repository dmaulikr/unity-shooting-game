using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class Button : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void click()
    {
        SceneManager.LoadScene("_Complete-Game");

//		// thread create test
//		Thread thread = new Thread (() => {
//			for( var i=0; i<3; i++ ) {
//				Thread.Sleep(1000);
//				Debug.Log((i + 1) + " seconds elapsed!");
//			}
//		});
//		thread.Start ();

    }
}
