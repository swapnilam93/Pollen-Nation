using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffectParticles : MonoBehaviour {

	public ParticleSystem[] particleSystems;

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Stinger")) {
			foreach (var particleSystem in particleSystems)
			{
				particleSystem.Play();	
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag("Stinger")) {
			foreach (var particleSystem in particleSystems)
			{
				particleSystem.Stop();	
			}
		}
	}

}
