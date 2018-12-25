using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerState : MonoBehaviour {
	protected SteamVR_TrackedObject trackedObj;
	[SerializeField] GameObject glove;
	[SerializeField] GameObject hand;
	public bool isFist;
	public SteamVR_Controller.Device device {
		get {
			return SteamVR_Controller.Input((int) trackedObj.index);
		}
	}

	// Use this for initialization
	void Awake () {
		isFist = false;
	}

	void Start () {
		glove.SetActive(false);
		hand.SetActive(true);
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger)) {
			// TODO: Activate fist
			Debug.Log("Button pressed");
			isFist = true;
			glove.SetActive(true);
			hand.SetActive(false);
		}
		if (device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger)) {
			Debug.Log("Button released");
			// Activate hand
			isFist = false;
			glove.SetActive(false);
			hand.SetActive(true);
		}
	}

	public void StingFeedback() {
		Debug.Log("HAPTIC FEEDBACK");
		device.TriggerHapticPulse(3000);
	}
}
