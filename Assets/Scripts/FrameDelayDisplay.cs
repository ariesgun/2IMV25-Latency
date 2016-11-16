using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FrameDelayDisplay : MonoBehaviour {

	public int delay;
	private Text frameDelayText;

	// Use this for initialization
	void Start () {
		frameDelayText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		frameDelayText.text = "Frame Delay: " + delay.ToString();
	}
}
