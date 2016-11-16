using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Leap;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class ScenesController : MonoBehaviour {

	[SerializeField]
	private HandController handController;
	[SerializeField]
	private GameObject cursor;
	[SerializeField]
	private FrameDelayDisplay frameDelayText;
	[SerializeField]
	private TimeResultHeaderDisplay timeResultHeaderText;
	[SerializeField]
	private TimeResultDisplay timeResultText;
	[SerializeField]
	private SceneNameDisplay sceneNameText;

	[SerializeField]
	private int numberOfTrials = 5;

	[Header("Key Mappings")]
	[SerializeField]
	private KeyCode startExperimentKey = KeyCode.S;
	[SerializeField]
	private KeyCode reloadSceneKey = KeyCode.Space;

	int[] delay = { 0, 20, 40, 59 };
	private int idFrameDelay;

	// Scene States
	private bool reloading = false;
	private bool trialMode;
	//private bool locked = false;
	private bool nextScene = false;
	// current Trial number

	public SceneRandomizer.SceneTrial curScene;
	public int curLatency;

	// LeapProvider provider
	private SphereInteraction sphereInteraction;
	Stopwatch sw = new Stopwatch();

	private float reloadDelay = .8f;
	private float reloadTimer;

	private int lastLatency;
	private bool nextTrial;

	// Use this for initialization
	void Start () {
		LoadData ();

		curScene = SceneRandomizer.Instance.GetCurScene ();
		if (!trialMode) {
			curLatency = SceneRandomizer.Instance.GetCurLatency ();
		}

		if (curScene.sceneName == "" || curScene.sceneName == null) {
			curScene.sceneName = SceneManager.GetActiveScene ().name;
		}

		if (curScene.sceneName == SceneRandomizer.Instance.GetFinalScene ()) {
			UnityEngine.Debug.Log ("Exiting");
			return;
		}

		frameDelayText.delay = curLatency; //delay[idFrameDelay];
		sceneNameText.name = curScene.sceneName;
		sceneNameText.trial = curScene.curTrial;


		sphereInteraction = cursor.GetComponent<SphereInteraction> ();
		//sceneRnd = FindObjectOfType<SceneRandomizer> ();


		if (sphereInteraction != null) {
			sphereInteraction.OnStartLocking += StartTimer;
			sphereInteraction.OnEndLocking   += EndTimer;
			sphereInteraction.OnResetTimer += ResetTimer;
		}

		// Set Trial Mode
		if (trialMode)
			timeResultHeaderText.SetText ("Trial Mode!");
		else
			timeResultHeaderText.SetText ("Time Results");

		// provider = FindObjectOfType<LeapProvider> () as LeapProvider;
		// provider.SetFrameDelay (delay[idFrameDelay]);
		handController.SetFrameDelay(curLatency);

	}

	// Update is called once per frame
	public void LoadData() {
		idFrameDelay = MutableData.Instance.latencyIdx;
		trialMode = MutableData.Instance.trialMode;
		// MutableData.Instance.meanTime.CopyTo (timeResultText.result, 0);
	}

	public void SaveData() {
		MutableData.Instance.latencyIdx = idFrameDelay;
		MutableData.Instance.trialMode = trialMode;
		// timeResultText.result.CopyTo (MutableData.Instance.meanTime, 0);
	}

	// Update is called once per frame
	void Update () {
		//UnityEngine.Debug.Log ("Time: " + Time.deltaTime);
		if ((!reloading && Input.GetKeyUp (reloadSceneKey)) || nextTrial) {
			// Go to the next State
			nextTrial = false;
			NextState ();
			// SceneRandomizer.SceneTrial temp = sceneRnd.GetNextSceneFromList ();
		} else if (!reloading && trialMode && Input.GetKeyDown (startExperimentKey)) {
			DebugConsole.Log ("Starting Experiment");

			MutableData.Instance.PrepareOutputFile ();

			SceneRandomizer.Instance.ResetScenesList ();

			curScene = SceneRandomizer.Instance.GetNextSceneFromList (!trialMode);
			curLatency = SceneRandomizer.Instance.GetNextLatencyFromList (!trialMode);

			trialMode = false;
			reloading = true;
			MutableData.Instance.trialMode = trialMode;
		} 

		if (reloading) {
			// .. add a timer for delay before loading the next scene.
			//DebugConsole.Log ("Reloading", "warning");
			reloadTimer = reloadTimer + Time.deltaTime;

			if (reloadTimer >= reloadDelay) {
				reloadTimer = 0;
				reloading = false;
				if (nextScene) {
					// load the next scene
					MutableData.Instance.WriteDataIntoFile (lastLatency);
					//SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
					//SceneManager.LoadScene("scene3");
					nextScene = false;
				} 
				SceneManager.LoadScene (curScene.sceneName);
			}
		} 
	}

	void NextState() {
		if (trialMode) {
			DebugConsole.Log ("Resetting Scene");
			curScene = SceneRandomizer.Instance.GetNextSceneFromList (trialMode);

			reloading = true;
		} else {
			DebugConsole.Log (curScene.sceneName + " " + curScene.curTrial.ToString(), "warning");
			MutableData.Instance.StoreData (curScene.sceneName, curScene.curTrial, sw.ElapsedMilliseconds, sphereInteraction.IsHit(), sphereInteraction.nbOvershoot);

			reloading = true;
			curScene = SceneRandomizer.Instance.GetNextSceneFromList (trialMode);
	
			if (curScene.sceneName == "") {
				nextScene = true;
				lastLatency = curLatency;
				curLatency = SceneRandomizer.Instance.GetNextLatencyFromList (trialMode);
				if (curLatency == -1) {
					UnityEngine.Debug.Log ("End Of Experiment");
					curScene.sceneName = SceneRandomizer.Instance.GetFinalScene ();
				} else {
					curScene = SceneRandomizer.Instance.GetNextSceneFromList (trialMode);
				}
			}
		}
	}

	void StartTimer() {
		//DebugConsole.Log ("Timer Started", "warning");
		sw.Start ();
	}

	void EndTimer() {
		//DebugConsole.Log ("Timer stopped", "warning");
		sw.Stop ();
		timeResultText.setMeanTime (idFrameDelay, sw.ElapsedMilliseconds);
		UnityEngine.Debug.Log (curScene.sceneName + "(" + curScene.curTrial.ToString() + "): " + sw.ElapsedMilliseconds);

		if (!trialMode)
			Application.CaptureScreenshot("./Screenshot/Screenshot_" + curLatency.ToString() + "_" + 
				curScene.sceneName + "_" + curScene.curTrial.ToString() + ".png");

		nextTrial = true;
		// DisplayElapsedTime ();
	}

	void ResetTimer() {
		//DebugConsole.Log ("Timer resetted", "warning");
		sw.Reset ();
	}

	/* For Debugging */
	void DisplayElapsedTime() {
		DebugConsole.Log ("Elapsed: " + sw.ElapsedMilliseconds.ToString());
	}

}
