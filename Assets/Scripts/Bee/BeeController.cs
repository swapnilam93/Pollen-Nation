using UnityEngine;
using System.Collections;
using UnityEditor;
using XInputDotNetPure;

public class BeeController : MonoBehaviour {
	// Bee existence and movement variables
	/* Bee gravestone is a game object on the bee that will be set inactive at the start of the game.
	 * It must have a collider and a rigidbody with gravity. Once the bee dies, everything about the bee
	 * will be deactivated, but the model will be replaced by this, which will be activated. The gravestone
	 * will then fall on the floor and not be controllable by the bee
	 */
	public Transform birdTransform;
	public Vector2 maxDistance;
	//public Vector2 minDistance;
	Vector3 _followOffset;
	public GameObject beeBody;

	private Vector3 curLoc;
	private Vector3 prevLoc;
	private Quaternion curRot;
	private Quaternion prevRot;

	Flower flower;
	public string controller;
	public PlayerIndex playerIndex;
	public AudioSource audioSource;
	public AudioClip pollinatingClip;
	public AudioClip pollinationSuccessfulClip;
	public AudioSource pollinationAudioSource;
	public bool canMove;
	public bool trapped;
	public Vector3 startLocation;
	public Quaternion startRotation;

	void Start() {
        // Cache the initial offset at time of load/spawn:
        _followOffset = transform.position;// - birdTransform.position;
		controller = null;
		curLoc = transform.position;
		curRot = transform.rotation;
		canMove = true;
		trapped = false;
		startLocation = gameObject.transform.position;
		startRotation = gameObject.transform.rotation;
    }

	bool startPollinating = false;
	float pollination = 0f;

	public void Restart() {
		gameObject.transform.position = startLocation;
		gameObject.transform.rotation = startRotation;
	}
    void Update() {
		if (canMove) {
			//keyboard or xbox
			/*if (Input.GetButtonDown(controller + "Fire1")) {
				Sting();
			}*/
			// Detect whether bee is pollinating
			if ((controller != null && (Input.GetButton(controller + "Fire1") || Input.GetButton(controller + "Fire2"))) || 
				(GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.B == ButtonState.Pressed)) {
				if (this.GetComponent<ColliderPushBack>().onFlower) {
					// If pollinating, increment pollination status
					if (pollination >= 0 && startPollinating) {
						pollination += 1f * Time.deltaTime;
					// Start pollinating if not pollinating and hasn't started
					} else if (pollination <= 0f && !startPollinating && !pollinationAudioSource.isPlaying && !flower.pollinationComplete) {
						gameObject.GetComponentInChildren<ParticleSystem>().Play();
						beeBody.GetComponent<Animator>().SetBool("Pollinate", true);
						startPollinating = true;
						pollinationAudioSource.PlayOneShot(pollinatingClip);
						if (flower != null) {
							flower.pollinatingParticles.SetActive(true);
							flower.Smile();
						}
						StartCoroutine(Pollinate());
					}
				}
			}
			// If pollinating has stopped, determine whether it was enough
			if (!startPollinating && pollination > 0f || !this.GetComponent<ColliderPushBack>().onFlower) {
				pollinationAudioSource.Pause();
				gameObject.GetComponentInChildren<ParticleSystem>().Stop();
				beeBody.GetComponent<Animator>().SetBool("Pollinate", false);
				if (pollination >= 1.25f) {
					// TODO: Haptic feedback, success
					flower.numPollinations++;
					PopUpTextController.CreatePopUpText("+1", transform);
					pollinationAudioSource.Stop();
					//pollinationAudioSource.PlayOneShot(pollinationSuccessfulClip);
				}
				pollination = 0f;
				if (flower != null) {
					flower.pollinatingParticles.SetActive(false);
					flower.Idle();
				}
			}

			//to maintain previous and current position and rotation
			prevLoc = curLoc;
			curLoc = transform.position;
			prevRot = curRot;
			curRot = transform.rotation;

			//get displacements in x and z

			float xD;
			float zD;

			if (controller == "K") {
				//for keyboard
				xD = Input.GetAxis(controller + "Horizontal");
				zD = Input.GetAxis(controller + "Vertical");
			}
			else {
				//for xbox controllers
				xD = GamePad.GetState(playerIndex).ThumbSticks.Left.X;
				zD = GamePad.GetState(playerIndex).ThumbSticks.Left.Y;
			}

			var x = xD * Time.deltaTime * 3.0f;
			var z = zD * Time.deltaTime * 3.0f;

			//change position wrt last
			curLoc += new Vector3(x, 0, z);
			transform.position = curLoc;

			//change rotation wrt last
			curRot = Quaternion.Slerp (curRot,  (curLoc - prevLoc == Vector3.zero ? curRot : Quaternion.LookRotation(curLoc - prevLoc)), Time.fixedDeltaTime * 30);
			transform.rotation = curRot;
		}

			//clamp position to a restricted space
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, birdTransform.position.x-maxDistance.x, birdTransform.position.x+maxDistance.x),
							_followOffset.y,
							Mathf.Clamp(transform.position.z, birdTransform.position.z-maxDistance.y, birdTransform.position.z+maxDistance.y));
	}

	private void FixedUpdate() {
		if (this.GetComponent<ColliderPushBack>().onFlower && controller != "K") {
			//Debug.Log("vibrate" + playerIndex);
			GamePad.SetVibration(playerIndex, pollination/1.25f, pollination/1.25f);
		}
	}

	IEnumerator Pollinate() {
		yield return new WaitForSeconds(1.25f);
		startPollinating = false;
	}

	public void VenusFlyTrapped() {
		//TODO: audioclip for trapped
		if (!trapped) {
			canMove = false;
			trapped = true;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			Animator animator = beeBody.GetComponent<Animator>();
			animator.SetBool("Trapped", true);
			PopUpTextController.CreateDamageText("Trap", transform);
		}
	}

	public void VenusFlyReleased() {
		if (trapped) {
			Animator animator = beeBody.GetComponent<Animator>();
			animator.SetBool("Trapped", false);
			canMove = true;
			trapped = false;
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.GetComponent<Flower>() != null) {
			flower = other.gameObject.GetComponent<Flower>();
		}
	}

	private void OnApplicationQuit() {
		GamePad.SetVibration(playerIndex, 0, 0);
	}

}