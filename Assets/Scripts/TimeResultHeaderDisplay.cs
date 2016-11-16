using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeResultHeaderDisplay : MonoBehaviour {

	private Text textHeader;

	// Use this for initialization
	void Start () {
		textHeader = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetText(string newText) {
		if (textHeader == null)
			textHeader = GetComponent<Text> ();
		
		textHeader.text = newText;
	}

}
