using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class ColliderPushBack : MonoBehaviour {

	public float force;
	private Vector3 forceMag;
	public AudioSource audioSource;
	public AudioClip pushBackClip;
	public AudioClip[] punchedClip;
	public AudioClip stunClip;
	public Animator animator;
	public GameObject star;
	public bool onFlower = false;
	public ParticleSystem knockbackParticles;
	private void Awake() {

	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.name.Equals("InnerCollider")) {
			if (!other.gameObject.GetComponent<Flower>().pollinationComplete)
				onFlower = true;
		}
		if (other.gameObject.tag == "Hat Object" || other.gameObject.tag == "VRPlayer") {
			//Debug.Log(other.gameObject.tag + " entered");
			knockbackParticles.Play();
			animator.SetTrigger("Knockback");
			if (other.gameObject.tag == "Hat Object") {
				//audioSource.PlayOneShot(other.gameObject.GetComponent<HatObject>().hitSound);
				audioSource.PlayOneShot(punchedClip[(int)Random.Range(0f, punchedClip.Length - 0.1f)]);
			}
			//Debug.Log("entered");
            Vector3 pushDirection = (other.transform.position - transform.position);
			//Debug.Log("start " + pushDirection);
			pushDirection =- pushDirection.normalized;
			//Debug.Log(pushDirection);
			forceMag = pushDirection * force;
    		GetComponent<Rigidbody>().velocity = forceMag;
			StartCoroutine(Stun());
		}
		if (other.gameObject.tag == "Bullet") {
			animator.SetTrigger("Knockback");
			//audioSource.PlayOneShot(punchedClip[(int)Random.Range(0f, punchedClip.Length - 0.1f)]);
			//Debug.Log("entered");
            Vector3 pushDirection = (other.transform.position - transform.position);
			//Debug.Log("start " + pushDirection);
			pushDirection =- pushDirection.normalized;
			//Debug.Log(pushDirection);
			forceMag = pushDirection * force/4;
    		GetComponent<Rigidbody>().velocity = forceMag;
			StartCoroutine(Stun());
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.gameObject.name.Equals("InnerCollider")) {
			onFlower = false;
			GamePad.SetVibration(gameObject.GetComponent<BeeController>().playerIndex, 0, 0);
		} 
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag == "Bullet") {
			animator.SetTrigger("Knockback");
			//audioSource.PlayOneShot(punchedClip[(int)Random.Range(0f, punchedClip.Length - 0.1f)]);
			//Debug.Log("entered");
            Vector3 pushDirection = (other.transform.position - transform.position);
			//Debug.Log("start " + pushDirection);
			pushDirection =- pushDirection.normalized;
			//Debug.Log(pushDirection);
			forceMag = pushDirection * force/4;
			Debug.Log("ssting push added");
    		GetComponent<Rigidbody>().velocity = forceMag;
			StartCoroutine(Stun());
		}
	}

	IEnumerator NullifyForce(Vector3 forceMag) {
		yield return new WaitForSeconds(0.2f);
		GetComponent<Rigidbody>().AddForce(-forceMag);
	}

	IEnumerator Stun() {
		animator.SetBool("Stun", true);
		star.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<BeeController>().canMove = false;
		audioSource.PlayOneShot(stunClip);
		//add stun animation (rotating)
		yield return new WaitForSeconds(1.8f);
		animator.SetBool("Stun", false);
		star.SetActive(false);
		//dont release trapped bee after stun
		if (!GetComponent<BeeController>().trapped)
			GetComponent<BeeController>().canMove = true;
	}

}
