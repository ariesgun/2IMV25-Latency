using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class MutableData : MonoBehaviour {

	public int latencyIdx;
	public long[] meanTime = new long[4];
	public bool trialMode = true;

	public long[][] measuredTime;
	public bool[][] errorLog;
	public int[][] nbOvershoot;

	public static MutableData Instance;

	void Awake() {
		if (Instance == null) {
			DontDestroyOnLoad (gameObject);
			Instance = this;

		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	void Start() {
		int nbOfScene = SceneRandomizer.Instance.GetNumberOfScenes ();
		int nbOfTrial = SceneRandomizer.Instance.GetNumberOfTrials ();

		measuredTime = new long[nbOfScene][];
		for (int i = 0; i < nbOfScene; i++) {
			measuredTime [i] = new long[nbOfTrial];
		}
		errorLog = new bool[nbOfScene][];
		for (int i = 0; i < nbOfScene; i++) {
			errorLog [i] = new bool[nbOfTrial];
		}
		nbOvershoot = new int[nbOfScene][];
		for (int i = 0; i < nbOfScene; i++) {
			nbOvershoot [i] = new int[nbOfTrial];
		}
		//UnityEngine.Debug.Log (measuredTime.Length);
		//UnityEngine.Debug.Log  (measuredTime[0].Length);

	}

	void ExportDataToFile() {
		StringBuilder csv = new StringBuilder ();
		for (int i = 0; i < meanTime.Length; i++) {
			csv.Append (meanTime[i].ToString() + ";");
		}
		csv.Append ("\n");
		File.AppendAllText("./timeMeasurement.csv", csv.ToString());
	}

	void AppendNextScene(string sceneId) {
		File.AppendAllText("./timeMeasurement.csv", "Scene;" + sceneId + "\n");
	}

	public void SaveData(string sceneName) {
		AppendNextScene(sceneName);
		ExportDataToFile();
	}

	public void PrepareOutputFile() {
		StringBuilder csv = new StringBuilder ();

		csv.Append ("Start Of Experiment at " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\n");

		File.AppendAllText("./timeMeasurement.csv", csv.ToString());
	}

	public void WriteDataIntoFile(int lastLatency) {
		StringBuilder csv = new StringBuilder ();
		//UnityEngine.Debug.Log ("0 - " + measuredTime.GetLength (0) + "; 1 - " + measuredTime[0].GetLength (0));
		for (int i = 0; i < measuredTime.GetLength(0); i++) {
			// Append Scene Name
			csv.Append (SceneRandomizer.Instance.GetSceneNameBasedOnIndex(i) + ";");
			// Append Lag Info
			csv.Append (lastLatency + ";");
			// Append Measured Time according to the number of trials
			for (int j = 0; j < measuredTime[i].GetLength (0); j++) {
				csv.Append (measuredTime [i] [j].ToString () + ";");
			}
			// Append Error Rate
			csv.Append (";;;");
			for (int j = 0; j < errorLog[i].GetLength (0); j++) {
				csv.Append (errorLog [i] [j].ToString () + ";");
			}

			// Append Overshoot
			csv.Append (";;;");
			for (int j = 0; j < nbOvershoot[i].GetLength (0); j++) {
				csv.Append (nbOvershoot [i] [j].ToString () + ";");
			}

			csv.Append ("\n");
		}
		File.AppendAllText("./timeMeasurement.csv", csv.ToString());
	}

	public void StoreData(string sceneName, int idxTrial, long val, bool hit, int overshoot) {
		//UnityEngine.Debug.Log ("saved to " + sceneName + " " + idxTrial.ToString() + " hit: " + hit + "; val: " + val);
		measuredTime [SceneRandomizer.Instance.GetIndexOfScene(sceneName)] [idxTrial-1] = val;
		errorLog [SceneRandomizer.Instance.GetIndexOfScene(sceneName)] [idxTrial-1]     = hit;
		nbOvershoot [SceneRandomizer.Instance.GetIndexOfScene(sceneName)] [idxTrial-1]     = overshoot;
	}

}
