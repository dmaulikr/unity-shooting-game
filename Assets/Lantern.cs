using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;

public class Lantern : MonoBehaviour {

	Thread th = new Thread (() => {
		UnityEngine.Debug.Log("Profiler.GetTotalAllocatedMemoryLong : " + Profiler.GetTotalAllocatedMemoryLong());
		UnityEngine.Debug.Log("Profiler.GetTotalReservedMemoryLong : " + Profiler.GetTotalReservedMemoryLong());
		UnityEngine.Debug.Log("Profiler.GetTotalUnusedReservedMemoryLong : " + Profiler.GetTotalUnusedReservedMemoryLong());
//		UnityEngine.Debug.Log("Profiler.GetRuntimeMemorySizeLong : " + Profiler.GetRuntimeMemorySizeLong());
		UnityEngine.Debug.Log("Profiler.GetMonoHeapSizeLong : " + Profiler.GetMonoHeapSizeLong());
		UnityEngine.Debug.Log("Profiler.GetMonoUsedSizeLong : " + Profiler.GetMonoUsedSizeLong());
		UnityEngine.Debug.Log("StackTraceUtility.ExtractStackTrace : " + StackTraceUtility.ExtractStackTrace());
//		Thread.Sleep(1000);
	});

	// Use this for initialization
	void Start () {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
//		th.Start ();
		UnityEngine.Debug.Log("Profiler.GetTotalAllocatedMemoryLong : " + Profiler.GetTotalAllocatedMemoryLong());
		UnityEngine.Debug.Log("Profiler.GetTotalReservedMemoryLong : " + Profiler.GetTotalReservedMemoryLong());
		UnityEngine.Debug.Log("Profiler.GetTotalUnusedReservedMemoryLong : " + Profiler.GetTotalUnusedReservedMemoryLong());
		//		UnityEngine.Debug.Log("Profiler.GetRuntimeMemorySizeLong : " + Profiler.GetRuntimeMemorySizeLong());
		UnityEngine.Debug.Log("Profiler.GetMonoHeapSizeLong : " + Profiler.GetMonoHeapSizeLong());
		UnityEngine.Debug.Log("Profiler.GetMonoUsedSizeLong : " + Profiler.GetMonoUsedSizeLong());
		UnityEngine.Debug.Log("StackTraceUtility.ExtractStackTrace : " + StackTraceUtility.ExtractStackTrace());
		PerformanceCounter cpuCounter = new PerformanceCounter ("Processor", "% Processor Time", "_Total");
		UnityEngine.Debug.Log ("" + cpuCounter.NextValue ());
		log( "thread count : " + Process.GetCurrentProcess().Threads.Count);
		log ("Lantern Agent for Unity has been loaded successfully");
	}

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        log(arg0.name + " this scene has been loaded!");
    }

    // Update is called once per frame
    void Update () {
		
	}

	private void log(object obj) {
		UnityEngine.Debug.Log (obj.ToString ());
	}
}