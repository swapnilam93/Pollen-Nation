using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MantisController : MonoBehaviour {
	public List<ControllerInput> controllers = new List<ControllerInput>();
	[SerializeField] ControllerInput controller1;
	[SerializeField] ControllerInput controller2;

	public GameObject vrCamera;

	public Transform offset;

	// Use this for initialization
	void Start () {
		controllers.Add(controller1);
		controllers.Add(controller2);
	}
	
	// Update is called once per frame
	void Update () {
		if (offset != null)
			transform.position = vrCamera.transform.position - offset.position - offset.position;
		else
			transform.position = vrCamera.transform.position;
		transform.rotation = Quaternion.Euler(0f, vrCamera.transform.rotation.eulerAngles.y, 0f);
	}

	private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Stinger") {
            Destroy(gameObject);
        }
    }

}
