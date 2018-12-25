using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenusFlyTrapSeed : MonoBehaviour {

	public GameObject venusFlyTrap;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "Venus Flytrap Plane") {
			Debug.Log("plane entered");
			//TODO: animate flytrap grow
			//gameObject.GetComponent<Rigidbody>().useGravity = false;
			//gameObject.GetComponent<Rigidbody>().isKinematic = true;
			Vector3 position = other.ClosestPointOnBounds(transform.position);
			Quaternion rotation = Quaternion.identity;
			Instantiate(venusFlyTrap, position, rotation);
			Destroy(gameObject);
		} else if (other.gameObject.name == "Venus Flytrap Destroyer") {
			Destroy(gameObject);
		}
	}
}
