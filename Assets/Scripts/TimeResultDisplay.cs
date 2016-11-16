using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeResultDisplay : MonoBehaviour {

	public long[] result = new long[4];
	private Text resultText;

	// Use this for initialization
	void Start () {
		resultText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		resultText.text = "";
		for (int i = 0; i < result.Length; i++) {
			if (result [i] > 0) {
				resultText.text += (result [i]/1000).ToString () + "." + (result[i] % 1000).ToString() + " ms\n";
			}
		}
	}

	public void setMeanTime( int idx, long newValue) {
		result [idx] = newValue;
	}

}
