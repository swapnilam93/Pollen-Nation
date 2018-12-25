using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotator : MonoBehaviour {
    public Vector3 rotateSpeed;
    //public Vector3 translateSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotateSpeed * Time.deltaTime);
        //transform.Translate(translateSpeed * Time.deltaTime);
	}
}
