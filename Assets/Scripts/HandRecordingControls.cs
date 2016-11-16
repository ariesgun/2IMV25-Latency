using UnityEngine;
using System.Collections;
using System.IO;

public class HandRecordingControls : MonoBehaviour {
	
	[SerializeField]
	private GameObject cursor;
	[SerializeField]
	private ScenesController sceneCont;

	private SphereInteraction sphereInteraction;

	void Start() {
		sphereInteraction = cursor.GetComponent<SphereInteraction> ();

		// Register Event Listener
		if (sphereInteraction != null) {
			sphereInteraction.OnStartLocking += StartRecoding;
			sphereInteraction.OnEndLocking   += EndRecording;
		}
	}

	void StartRecoding() {
		//UnityEngine.Debug.Log ("Begin Recording");
		HandController.Main.ResetRecording();
		HandController.Main.Record();
	}

	void EndRecording() {
		//UnityEngine.Debug.Log ("End Recording");
		HandController.Main.StopRecording ();

		LeapRecorder leapRec = HandController.Main.GetLeapRecorder ();

		if (!Directory.Exists ("./Recording/" + sceneCont.curLatency.ToString () +
		    "/" + sceneCont.curScene.sceneName)) {
			Directory.CreateDirectory ("./Recording/" + sceneCont.curLatency.ToString () +
				"/" + sceneCont.curScene.sceneName);
		}

		string savePath = leapRec.SaveToNewFile ("./Recording/" + sceneCont.curLatency.ToString() +
			"/" + sceneCont.curScene.sceneName + "/" + "Recording_" + sceneCont.curScene.curTrial + "_" +
			System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".bytes");

		//UnityEngine.Debug.Log ("Recording saved to:\n" + savePath );
	}
		
}
