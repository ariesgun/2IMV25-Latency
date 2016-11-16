using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneNameDisplay : MonoBehaviour {

	public string name;
	public int trial;
	private Text sceneNameText;

	// Use this for initialization
	void Start () {
		sceneNameText = GetComponent<Text> ();
	}

	// Update is called once per frame
	void Update () {
		sceneNameText.text = "Scene name: " + name + " (" + trial.ToString() + ")";
	}
}
