using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour {
	// Use this for initialization
	[SerializeField] public Animator animator;

	public GameObject[] petals;
	[SerializeField] AudioClip[] petalClips;

	public GameObject pollinatingParticles;
	public ParticleSystem pollinationSuccessParticles;
	public ParticleSystem pollinationCompleteParticles;
	[SerializeField] AudioSource flowerSource;
	[SerializeField] AudioClip flowerCompleteClip;
	// Number of pollinations done so far in the game
	public int numPollinations = 0;
	public bool pollinationComplete = false;
	public int maxPollinations;
	private int petalNumberHolder;
	void Start () {
		maxPollinations = petals.Length;
		petalNumberHolder = -1;
		foreach (GameObject petal in petals) {
			petal.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	public void Restart() {
		numPollinations = 0;
		petalNumberHolder = -1;
		foreach (GameObject petal in petals) {
			petal.GetComponent<MeshRenderer>().enabled = false;
		}
		Idle();
		pollinationCompleteParticles.Stop();
		pollinationComplete = false;
		finishedPlayed = false;
		animator.SetBool("Smile", false);
		animator.SetTrigger("Restart");
	}
	
	bool finishedPlayed = false;
	// Update is called once per frame
	void Update () {
		if (petalNumberHolder != (numPollinations - 1))
			PollinatePetal();
		if (numPollinations == maxPollinations && !finishedPlayed) {
			finishedPlayed = true;
			pollinationComplete = true;
			Smile();
			pollinationCompleteParticles.Play();
			flowerSource.PlayOneShot(flowerCompleteClip);
		}
	}

	public void Idle() {
		if (!pollinationComplete)
			animator.SetBool("Smile", false);
	}

	public void Smile() {
		animator.SetBool("Smile", true);
	}

	public void PollinatePetal () {
		if (!pollinationComplete) {
			pollinationSuccessParticles.Play();
			petalNumberHolder = numPollinations - 1;
			if (petalNumberHolder < numPollinations) {
				if (petalNumberHolder >= maxPollinations - 1)
					petalNumberHolder = maxPollinations - 1;
				petals[petalNumberHolder].GetComponent<MeshRenderer>().enabled = true;
				flowerSource.PlayOneShot(petalClips[petalNumberHolder]);
			}
		}
	}
}
