using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour {

	void Start() {
		DebugConsole.Log ("Triggered", "warning");	
	}

	void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject)
		DebugConsole.Log("Triggered", "warning");	
		if (other.gameObject.CompareTag("Cursor")) {
			Renderer rend = other.gameObject.GetComponent<Renderer>();
			rend.material.SetColor ("_Color", Color.red);
		}
	}
}
