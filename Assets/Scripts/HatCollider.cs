using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class HatCollider : MonoBehaviour {
    [SerializeField] List<GameObject> hatObjects = new List<GameObject>();
	AudioSource audioSource;
	public AudioClip takeOutClip;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other) {
		ControllerInput hand = other.gameObject.GetComponent<ControllerInput>();
		// If collider belongs to a hand
		if (hand != null) {
			// If hand is not holding anything
			if (hand.heldObjects.Count == 0 && hand.device.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger)) {
				// Assign new object to a random value inside hatObjects
				GameObject newObject = Instantiate(hatObjects[Random.Range(0, hatObjects.Count)]);
				newObject.transform.position = hand.transform.position;
				// Pick up object
				newObject.GetComponent<InteractableItem>().Pickup(hand);
				hand.GetComponent<ControllerInput>().heldObjects.Add(newObject.GetComponent<InteractableItem>());
				audioSource.PlayOneShot(takeOutClip);
			}
		}
	}
}
