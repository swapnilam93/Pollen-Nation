using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: DELETE THIS FILE
public class HatObject : MonoBehaviour {

	public AudioClip hitSound;
	public AudioClip[] punchedClip;
	bool hitBee = false;


	// Use this for initialization
	void Start () {
		if (hitSound == null) {
			hitSound = punchedClip[(int)Random.Range(0f, punchedClip.Length - 0.1f)];
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		BeeController beeController = other.gameObject.GetComponent<BeeController>();
		if (beeController != null) {
			// If object is being thrown and hasn't hit bee, hit bee
			if (!gameObject.GetComponent<InteractableItem>().isPickedUp && !hitBee) {
				hitBee = true;
			}
		}
	}
}
