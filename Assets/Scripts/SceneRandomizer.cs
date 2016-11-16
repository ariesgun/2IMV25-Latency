using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class SceneRandomizer : MonoBehaviour {

	public struct SceneTrial 
	{
		public string sceneName;
		public int curTrial;
	}

	[SerializeField]
	private int NbOfTrials = 3;

	[SerializeField]
	private string finalScene;

	[SerializeField]
	private string[] scenes = {
		"scene1",
		"scene2",
		"scene3",
		"scene4"
	};

	[SerializeField]
	private int[] latency = { 
		0, 20, 40, 59 
	};

	public List<SceneTrial> scenesList;
	public List<int> latencyList;
	public int curLatency;

	private SceneTrial curScene;

	public static SceneRandomizer Instance;

	void Awake() {
		if (Instance == null) {
			DontDestroyOnLoad (gameObject);
			Instance = this;

		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {

		InitializeScenesList ();
		RandomizeScenesList ();
		InitializeLatencyList ();
		RandomizeLatencyList ();
	}

	public int GetNumberOfScenes() {
		return scenes.Length;
	}

	public int GetNumberOfTrials() {
		return NbOfTrials;
	}

	public int GetIndexOfScene(string sceneName) {
		return Array.IndexOf (scenes, sceneName);
	}

	public string GetSceneNameBasedOnIndex (int idx) {
		return scenes [idx];
	}

	// Initialize Scenes into the list
	void InitializeScenesList() {
		if (scenesList != null && scenesList.Count > 0)
			return;
		else {
			if (scenesList == null) {
				scenesList = new List<SceneTrial> ();
			}
			for (int i = 0; i < scenes.Length; i++) {
				SceneTrial temp;
				temp.sceneName = scenes [i];
				temp.curTrial = 0;
				scenesList.Add (temp);
			}
		}
	}

	void InitializeLatencyList() {
		if (latencyList != null && latencyList.Count > 0)
			return;
		else {
			if (latencyList == null) {
				latencyList = new List<int> ();
			}
			for (int i = 0; i < latency.Length; i++) {
				int temp = latency [i];
				latencyList.Add (temp);
			}
		}
	}

	public void ResetScenesList() {
		for (int i = 0; i < scenesList.Count; i++) {
			SceneTrial temp = scenesList [i];
			temp.curTrial = 0;
			scenesList [i] = temp;
		}
	}

	/* For Debugging Only
	void Update() {
		for (int i = 0; i < scenesList.Count; i++) {
			DebugConsole.Log (scenesList [i].sceneName, "warning");
		}
		DebugConsole.Log ("End", "warning");

		if (Input.GetKeyUp (KeyCode.A)) {
			RandomizeList ();
		}
	}
	*/

	// Randomize List of scenes
	void RandomizeScenesList() {
		if (scenesList == null)
			InitializeScenesList ();

		// Randomize
		int n = scenesList.Count;
		while (n > 1) {
			int k = (UnityEngine.Random.Range (0, n));
			n--;
			SceneTrial value = scenesList [k];
			scenesList [k] = scenesList [n];
			scenesList [n] = value;
		}
	}

	void RandomizeLatencyList() {
		if (latencyList == null)
			InitializeLatencyList ();

		// Randomize
		int n = latencyList.Count;
		while (n > 1) {
			int k = (UnityEngine.Random.Range (0, n));
			n--;
			int value = latencyList [k];
			latencyList [k] = latencyList [n];
			latencyList [n] = value;
		}
	}

	// Get Next Scenes
	public SceneTrial GetNextSceneFromList(bool trialMode) {
		if (scenesList.Count > 0) {
			int k = (UnityEngine.Random.Range (0, scenesList.Count));

			SceneTrial temp = scenesList [k];

			//string name = temp.sceneName;
			temp.curTrial++;
			scenesList [k] = temp;

			if (!trialMode && temp.curTrial > NbOfTrials - 1) {
				//UnityEngine.Debug.Log ("Removing scene " + temp.sceneName);
				scenesList.Remove (temp);
			}
				
			//UnityEngine.Debug.Log ("Latency " + temp.sceneName + "; Trial " + temp.curTrial);
			curScene = temp;

			return temp;
		} else {
			SceneTrial temp;
			temp.sceneName = "";
			temp.curTrial = 0;

			return temp;
		}
	}

	public int GetNextLatencyFromList(bool trialMode) {
		if (scenesList.Count == 0) {
			InitializeScenesList ();
			RandomizeScenesList ();
		}

		if (latencyList.Count > 0) {
			int k = (UnityEngine.Random.Range (0, latencyList.Count));
			curLatency = latencyList [k];

			if (!trialMode) {
				latencyList.Remove (curLatency);
				//UnityEngine.Debug.Log (latencyList.Count + " removing latency");

			}

			return curLatency;
		} else {
			curScene.sceneName = finalScene;
			return -1;
		}
	}

	public string GetFinalScene() {
		return finalScene;
	}

	public SceneTrial GetCurScene() {
		return curScene;
	}

	public int GetCurLatency() {
		return curLatency;
	}
}
