using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using Leap;
//using Leap.Unity;

public class SphereInteraction : MonoBehaviour {

	//public delegate void startTimer ();
	//public static event startTimer OnStartTimer;
	public event Action OnStartLocking;
	public event Action OnEndLocking;
	public event Action OnResetTimer;

	private bool locked;
    private bool mouse;
	private bool inCursor;
    private Vector3 movement;
	private Frame curFrame;
	public int nbOvershoot;

	private bool insideTarget;
	private bool insideTarget2;

		//LeapProvider provider;
	Controller controller;
	HandModel hand_model;

	//[SerializeField]
	//private Controller controller;

	void Start() {
		//provider = FindObjectOfType<LeapProvider> () as LeapProvider;
		hand_model =  FindObjectOfType<SkeletalHand> ();

		controller = new Controller();
		locked = false;
        mouse = false;
	}

    void Update()
    {
		if (inCursor && Input.GetMouseButtonDown(0))
        {
			mouse = true;
			if (inCursor) {
				
				OnStartLocking ();
			}
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouse = false;
            //DebugConsole.Log("Released right click.");
			if (locked) {
				OnEndLocking ();

			}
        }
    }

	void LateUpdate() {

		if (locked) {
			hand_model =  FindObjectOfType<SkeletalHand> ();
	
			//DebugConsole.Log (hand_model.name, "warning");
			//DebugConsole.Log ("Step1", "warning");
			//DebugConsole.Log(curFrame.ToString(), "warning");	
			//DebugConsole.Log("Step2", "warning");

			if (hand_model!= null) {
				Hand leap_hand = hand_model.GetLeapHand ();
				if (leap_hand.IsRight) {
					FingerModel fingerIndexModel = hand_model.fingers [1];

					if (fingerIndexModel != null) {
						transform.position = fingerIndexModel.GetTipPosition ();
					}
				}
			}
			//DebugConsole.Log(fingerIndexModel.GetTipPosition ().ToString(), "warning");






			/*
			// curFrame = provider.CurrentFrame;
			Frame frame = controller.Frame();
			//DebugConsole.Log(curFrame.ToString(), "warning");	
			/*
			/*
			foreach (Hand hand in frame.Hands) {
				if (hand.IsRight) {
					foreach (Finger finger in hand.Fingers) {
						if (finger.Type == Finger.FingerType.TYPE_INDEX) {
							transform.position = new Vector3 (finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);
							//DebugConsole.Log(finger.TipPosition.ToString() , "warning");
						}
					}
				}
			}
			*/
		}

	}

	void OnTriggerExit(Collider other) {
        Renderer rend = this.gameObject.GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.white);
		inCursor = false;
		//if (other.gameObject.name.Equals ("Target")) {
			if (insideTarget) {
				insideTarget = false;
				nbOvershoot++;
				UnityEngine.Debug.Log (nbOvershoot.ToString ());
			}
		//}
    }

    void OnTriggerStay(Collider other) {
		//DebugConsole.Log("Triggering Sphered", "warning");	
		inCursor = true;
		Renderer rend = this.gameObject.GetComponent<Renderer>();
             
		if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.name.Equals("index")) {
			if (mouse) {     
				rend.material.SetColor ("_Color", Color.red);
				locked = true;
			} else {
				rend.material.SetColor ("_Color", Color.green);
				locked = false;
			}
		} 

		if (other.gameObject.name.Equals ("Target")) {
			if (IsInsideTarget (other.gameObject)) {
				insideTarget = true;
				insideTarget2 = true;
				for (int i = 0; i < other.gameObject.transform.childCount; i++) {
					var temp = other.gameObject.transform.GetChild (i);
					if (!temp.name.Equals ("Front")) {
						Renderer render = temp.GetComponent<Renderer> ();
						render.material.SetColor ("_Color", Color.blue);
					}
				}
			} else {
				insideTarget2 = false;
				for (int i = 0; i < other.gameObject.transform.childCount; i++) {
					var temp = other.gameObject.transform.GetChild (i);
					if (!temp.name.Equals ("Front")) {
						Renderer render = temp.GetComponent<Renderer> ();
						render.material.SetColor ("_Color", Color.yellow);
					}
				}
			}
		} 
	}

	public bool IsHit() {
		UnityEngine.Debug.Log (insideTarget & insideTarget2);
		return (insideTarget & insideTarget2);
	}

	public bool IsInsideTarget(GameObject cube) {

		double x1 = this.transform.position.x;
		double y1 = this.transform.position.y;
		double z1 = this.transform.position.z;
		double sizeX1 = 0.6 * this.transform.localScale.x / 2;
		double sizeY1 = 0.6 * this.transform.localScale.y / 2;
		double sizeZ1 = 0.6 * this.transform.localScale.z / 2;

		double x2 = cube.transform.position.x;
		double y2 = cube.transform.position.y;
		double z2 = cube.transform.position.z;
		double sizeX2 = cube.transform.localScale.x / 2;
		double sizeY2 = cube.transform.localScale.y / 2;
		double sizeZ2 = cube.transform.localScale.z / 2;

		//DebugConsole.Log (sizeX2.ToString (), "Warning");

		return ((x1 - sizeX1) >= (x2 - sizeX2)) &&
			((x1 - sizeX1) <= (x2 + sizeX2)) &&
			((x1 + sizeX1) <= (x2 + sizeX2)) &&
			((x1 + sizeX1) >= (x2 - sizeX2)) &&

			((y1 - sizeY1) >= (y2 - sizeY2)) &&
			((y1 - sizeY1) <= (y2 + sizeY2)) &&
			((y1 + sizeY1) <= (y2 + sizeY2)) &&
			((y1 + sizeY1) >= (y2 - sizeY2)) &&

			((z1 - sizeZ1) >= (z2 - sizeZ2)) &&
			((z1 - sizeZ1) <= (z2 + sizeZ2)) &&
			((z1 + sizeZ1) <= (z2 + sizeZ2)) &&
			((z1 + sizeZ1) >= (z2 - sizeZ2));


	}

}
