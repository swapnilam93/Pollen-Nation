using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenusFlyTrap : MonoBehaviour {

	public GameObject mouth;
	public GameObject mouthPivot;
	public GameObject body;
	public ParticleSystem bloodParticles;
	public GameObject aoeParticles;
	public float force;
	public AudioSource mouthAudioSource;
	public AudioClip damageClip;
	public AudioClip deathClip;
	private AudioSource audioSource;
	public GameObject beeTrapped;
	private Vector3 forceMag;
	bool lerpToBee;
	bool lerpBack;
	float trappedDuration;
	int health;
	float timeBeeIsTrapped = 5f;
	float timeLeft;
	float venusFlyTrapLifespan = 12f;
	public bool canTrap;

	// Use this for initialization
	void Start () {
		timeLeft = venusFlyTrapLifespan;
		lerpToBee = false;
		lerpBack = false;
		beeTrapped = null;
		trappedDuration = 0f;
		health = 1;
		audioSource = GetComponent<AudioSource>();
		canTrap = true;
		//mouth.SetActive(false);
		body.GetComponent<Animator>().SetTrigger("Grow");
		//StartCoroutine(ActivateMouth());
	}
	bool destroyStarted = false;
	// Update is called once per frame
	void Update () {
		// Destroy venus fly trap after venusFlyTrapLifespan seconds
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0 && !destroyStarted && beeTrapped == null) {
			StartCoroutine(DestroyVenusFlytrap());
		}

		//lerp venus flytrap mouth towards or away from bee
		if (lerpToBee) {
			mouth.transform.position = Vector3.Lerp(mouth.transform.position, 
										beeTrapped.transform.position,
										Time.deltaTime * timeBeeIsTrapped);
			mouth.transform.rotation = Quaternion.Slerp(mouth.transform.rotation, 
										Quaternion.LookRotation(beeTrapped.transform.position - mouthPivot.transform.position), 
										Time.deltaTime * timeBeeIsTrapped);
			//release bee after some time if not released by other bee
			trappedDuration += Time.deltaTime;
			if (trappedDuration >= timeBeeIsTrapped) {
				ReleaseBee();
			}
		} else if (lerpBack) {
			mouth.transform.position = Vector3.Lerp(mouth.transform.position,
										mouthPivot.transform.position, 
										Time.deltaTime * timeBeeIsTrapped);
			mouth.transform.rotation = Quaternion.Slerp(mouth.transform.rotation,
										mouthPivot.transform.rotation,
										Time.deltaTime * timeBeeIsTrapped);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "AOE") {
        	return; 
		}
		if (other.gameObject.tag == "Stinger" && beeTrapped == null) {
			//trap the entered bee until released by another bee
			beeTrapped = other.gameObject;
			//only trap untrapped bees
			if (!beeTrapped.GetComponent<BeeController>().trapped && canTrap) {
				trappedDuration = 0f;
				lerpBack = false;
				lerpToBee = true;
				beeTrapped.GetComponent<BeeController>().VenusFlyTrapped();
				mouth.GetComponent<Animator>().SetBool("Grab", true);
				mouthAudioSource.Play();
				bloodParticles.Play();
			}
		} else if (other.gameObject.tag == "Stinger" && beeTrapped != null) {
			//release the trapped bee by another bee
			ReleaseBee();
			if (!destroyStarted)
				StartCoroutine(DestroyVenusFlytrap());
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Stinger" && beeTrapped != null) {
			Debug.Log("released by collider exit");
			ReleaseBee();
		}
	}

	public void ReleaseBee() {
		//TODO: audio for release
		trappedDuration = 0f;
		mouthAudioSource.Stop();
		beeTrapped.GetComponent<BeeController>().VenusFlyReleased();
		lerpToBee = false;
		beeTrapped = null;
		lerpBack = true;
		mouth.GetComponent<Animator>().SetBool("Grab", false);
		bloodParticles.Stop();
	}

	IEnumerator ActivateMouth() {
		yield return new WaitForSeconds(0.2f);
		mouth.SetActive(true);
		//canTrap = true;
	}

	public void StartDestroy() {
		StartCoroutine(DestroyVenusFlytrap());
	}

	IEnumerator DestroyVenusFlytrap() {
		destroyStarted = true;
		canTrap = false;	
		bloodParticles.Play();
		mouth.GetComponent<Animator>().SetTrigger("Destroy");
		audioSource.PlayOneShot(deathClip);
		yield return new WaitForSeconds(0.6f);
		mouth.SetActive(false);
		body.GetComponent<Animator>().SetTrigger("Die");
		if (beeTrapped != null) {
			ReleaseBee();
		}
		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}

}
