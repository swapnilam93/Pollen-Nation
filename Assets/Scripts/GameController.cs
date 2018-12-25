using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

// The game controller will direct where in the experience the guest is
public class GameController : MonoBehaviour {
	bool kAssigned;
	bool j1Assigned;
	bool j2Assigned;
	bool startPressed;

	/* State Key:
	 * 0 - Introduction and tutorial
	 * 1 - Game play
	 * 2 - Game over - Mantis won
	 * 3 - Game over - Bees won
	 */
	int state = 0;
	public float timeLeft;

	// Objects in the scene
	// The main flower which the mantis must protect and the bees must pollinate
	[SerializeField] List<Flower> flowers = new List<Flower>();
	[SerializeField] public Text timerText;
	[SerializeField] public Text beeScoreText;
	private int beeScore;
	[SerializeField] public Text mantisScoreText;
	private int mantisScore;
	// The virtual reality player
	[SerializeField] MantisController mantis;
	bool won;

	public BeeController[] beeControllers;

	private AudioSource audioSource;
	public AudioClip winClip;
	public int numSeconds = 60;
	public GameObject[] VRUIs;
	public GameObject[] xboxUIs;
	public GameObject gameName;

	// Use this for initialization
	void Start () {
		// Time left = 2 minutes, or 120 seconds
		timeLeft = numSeconds;
		timerText.text = timeLeft.ToString();
		beeScoreText.text = "Bees Score: 0";
		beeScore = 0;
		mantisScoreText.text = "Mantis Score: 0";
		mantisScore = 0;
		kAssigned = false;
		j1Assigned = false;
		j2Assigned = false;
		won = false;
		startPressed = false;

		// Initialize pop up texts
		PopUpTextController.Initialize();
		audioSource = GetComponent<AudioSource>();
	}

	[SerializeField] AudioClip startClip;
	[SerializeField] AudioSource crowdAnnouncerSource;
	[SerializeField] AudioSource announcerSource;
	[SerializeField] AudioClip beesWinClip;
	[SerializeField] AudioClip mantisWinsClip;
	[SerializeField] AudioSource bgm;
	[SerializeField] public AudioClip[] roundClips;
	IEnumerator StartGame() {
		gameName.SetActive(false);
		int round = beeScore + mantisScore;
		announcerSource.PlayOneShot(roundClips[round]);
		StartCoroutine(PlaySound(roundClips[round].length, crowdAnnouncerSource, startClip));
		VRUIs[mantisScore + beeScore].SetActive(false);
		xboxUIs[mantisScore + beeScore].SetActive(false);
		StartCoroutine(VR321Go());
		//crowdAnnouncerSource.PlayOneShot(startClip); // Start clip is 12 seconds long
		yield return new WaitForSeconds(startClip.length - 8 + roundClips[round].length);
		// Start music
		bgm.volume = 1f;
		bgm.Play();
		yield return new WaitForSeconds(2);
		// Timer starts here
		if (state == 0) {
			state++;
		}
		foreach (GameObject vrui in VRUIs) {
			vrui.SetActive(false);
		}
		foreach (GameObject xboxui in xboxUIs) {
			xboxui.SetActive(false);
		}
	}
	[SerializeField] TimerBar timerBar;
	IEnumerator RestartCoroutine() {
		// Fade out BGM
		while (bgm.volume > 0) {
            bgm.volume -= 1f * Time.deltaTime / .5f;
            yield return null;
        }
		timeLeft = numSeconds;
		timerText.text = timeLeft.ToString();
		won = false;

		// Restart all flowers
		for (int i = 0; i < flowers.Count; i++) {
			flowers[i].Restart();
		}

		// Get rid of all venus fly traps currently in the scene
		GameObject[] vfts = GameObject.FindGameObjectsWithTag("VFT");
		for (int i = 0; i < vfts.Length; i++) {
			VenusFlyTrap vft = vfts[i].GetComponent<VenusFlyTrap>();
			if (vft != null) {
				if (vft.beeTrapped != null) {
					vft.ReleaseBee();
				}
				vft.StartDestroy();
			}
		}

		// Put bees in original positions
		for (int i = 0; i < beeControllers.Length; i++) {
			beeControllers[i].Restart();
		}

		timerBar.Restart();
		winningStuffPlays = false;
		playedHalfWay = false;
		startedCountDownClip = false;
		state = 0;
		foreach (GameObject vrui in VRUIs) {
			vrui.SetActive(false);
		}
		foreach (GameObject xboxui in xboxUIs) {
			xboxui.SetActive(false);
		}
		VRUIs[mantisScore + beeScore].SetActive(true);
		xboxUIs[mantisScore + beeScore].SetActive(true);
	}
	/*void Restart() {
		timeLeft = numSeconds;
		timerText.text = timeLeft.ToString();
		won = false;

		// Restart all flowers
		for (int i = 0; i < flowers.Count; i++) {
			flowers[i].Restart();
		}

		// Get rid of all venus fly traps currently in the scene
		GameObject[] vfts = GameObject.FindGameObjectsWithTag("VFT");
		for (int i = 0; i < vfts.Length; i++) {
			VenusFlyTrap vft = vfts[i].GetComponent<VenusFlyTrap>();
			if (vft != null) {
				if (vft.beeTrapped != null) {
					vft.ReleaseBee();
				}
				vft.StartDestroy();
			}
		}

		// Put bees in original positions
		for (int i = 0; i < beeControllers.Length; i++) {
			beeControllers[i].Restart();
		}

		timerBar.Restart();
		winningStuffPlays = false;
		playedHalfWay = false;
		startedCountDownClip = false;
		state = 0;
	}*/
	[SerializeField] AudioClip halfWayClip;
	[SerializeField] AudioClip countDownClip;
	bool playedHalfWay = false;
	bool winningStuffPlays = false;
	bool startedCountDownClip = false;
	bool ultimateClipPlayed = false;
	// Update is called once per frame
	void Update () {
		if (Mathf.RoundToInt(timeLeft) == 33 && !playedHalfWay) {
			playedHalfWay = true;
			crowdAnnouncerSource.PlayOneShot(halfWayClip);
		}
		if (Mathf.RoundToInt(timeLeft) == 12 && !startedCountDownClip) {
			startedCountDownClip = true;
			crowdAnnouncerSource.PlayOneShot(countDownClip);
		}
		/*if (!kAssigned) {
			beeControllers[0].controller = "K";
			beeControllers[0].playerIndex = PlayerIndex.Three;
			kAssigned = true;
		} /*else if (!j1Assigned && Input.GetButtonDown("J1Submit")) {
			Debug.Log("J1 assigned");
			beeControllers[1].controller = "J1";
			j1Assigned = true;
		} else if (!j2Assigned && Input.GetButtonDown("J2Submit")) {
			Debug.Log("J2 assigned");
			beeControllers[2].controller = "J2";
			j2Assigned = true;
		}*/

		if (!j1Assigned || !j2Assigned || !kAssigned)
        {
            for (int i = 0; i < 3; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    if (!j1Assigned) {
						//beeControllers[1].controller = "J1";
						beeControllers[1].playerIndex = PlayerIndex.One;
						j1Assigned = true;
					} else if (!j2Assigned) {
						//beeControllers[2].controller = "J2";
						beeControllers[2].playerIndex = PlayerIndex.Two;
						j2Assigned = true;
					} else if (!kAssigned) {
						beeControllers[0].playerIndex = PlayerIndex.Three;
						kAssigned = true;
					}
                }
            }
        }

		timerText.text = Mathf.RoundToInt(timeLeft).ToString();

		if (state == 0) {
			if (Input.GetKeyDown(KeyCode.P) || (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed && !startPressed)) {
				startPressed = true;
				StartCoroutine(StartGame());
			}
			// Tutorial setup, announcer describing situation, players playing
			// TODO: Play welcome and introduction announcement
		} else if (state == 1) {
			// Game play, enable all actions
			// Start decrementing the timer
			timeLeft -= Time.deltaTime;
			// If the flower is pollinated fully, the bees win
			if (AllFlowersComplete()) {
				state += 1;
			}
			// If time is up, the mantis wins
			if (timeLeft <= 0) {
				state += 2;
			}
		} else if (state == 2) {
			// Play game ending state, bees won
			crowdAnnouncerSource.Stop();
			if (!won) {
				for (int i = 0; i < flowers.Count; i++) {
					//flowers[i].animator.SetTrigger("Restart");
				}
				audioSource.PlayOneShot(winClip);
				won = true;
				beeScore++;
				beeScoreText.text = "Bees Score: " + beeScore;
				VRUIs[3].SetActive(true);
				//xboxUIs[3].SetActive(true);
			}
			if (!winningStuffPlays) {
				winningStuffPlays = true;
				StartCoroutine(PlaySound(3.5f, announcerSource, beesWinClip));
			}
			
			PopUpTextController.CreatePopUpText("The bees win!", mantis.gameObject.transform);
		} else if (state == 3) {
			if (!winningStuffPlays) {
				// Play game ending state, mantis won
				winningStuffPlays = true;
				StartCoroutine(PlaySound(3.5f, announcerSource, mantisWinsClip));
			}
			PopUpTextController.CreatePopUpText("The mantis wins!", mantis.gameObject.transform);
			if (!won) {
				for (int i = 0; i < flowers.Count; i++) {
					flowers[i].animator.SetTrigger("Fade");
                    flowers[i].animator.SetBool("Smile", false);
                    flowers[i].pollinationCompleteParticles.Stop();
                }
				audioSource.PlayOneShot(winClip);
				won = true;
				mantisScore++;
				mantisScoreText.text = "Mantis Score: " + mantisScore;
				VRUIs[4].SetActive(true);
				//xboxUIs[4].SetActive(true);
			}
		}
		if (state == 4) {
			PopUpTextController.CreatePopUpText("Mantis: " + mantisScore + " Bee: " + beeScore, mantis.gameObject.transform);
		}
		// Play again
		if (state == 2 || state == 3) {
			if (!ultimateClipPlayed) {
				if (mantisScore + beeScore >= 3) {
				// TODO: Declare final winner
				// Wait for everything to finish
				// Present final scores, play audio
				StartCoroutine(Finishing(mantisScore, beeScore));
			} else {
				// Restart
				if (Input.GetKeyDown(KeyCode.N) || (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed && startPressed)) {
					//Restart();
					startPressed = false;
					StartCoroutine(RestartCoroutine());
				}
			}
			}
		}
		if (Input.GetKeyDown(KeyCode.N)) {
			//Restart();
			startPressed = false;
			gameName.SetActive(false);
			StartCoroutine(RestartCoroutine());
		}
	}
	[SerializeField] AudioClip mantisUltimateClip;
	[SerializeField] AudioClip beeUltimateClip;
	IEnumerator Finishing(int mantisScore, int beeScore) {
		ultimateClipPlayed = true;
		yield return new WaitForSeconds(7);
		bgm.volume = 0.2f;
		// Mantis won
		if (mantisScore > beeScore) {
			state=4;
			VRUIs[4].SetActive(true);
			announcerSource.PlayOneShot(mantisUltimateClip);
			// TODO Add UI
		} else {
			// Bees won
			state=4;
			VRUIs[3].SetActive(true);
			announcerSource.PlayOneShot(beeUltimateClip);
			// TODO Add UI
		}

	}

	bool AllFlowersComplete() {
		int num = 0;
		for (int i = 0; i < flowers.Count; i++) {
			if (flowers[i].pollinationComplete)
				num++;
		}
		if (num >= 3)
			return true;
		else
			return false;
	}

	IEnumerator PlaySound(float duration, AudioSource audioSource, AudioClip audioClip) {
		yield return new WaitForSeconds(duration);
		audioSource.PlayOneShot(audioClip);
	}

	IEnumerator VR321Go() {
		yield return new WaitForSeconds(2f);
		VRUIs[5].SetActive(true);
		xboxUIs[5].SetActive(true);
		yield return new WaitForSeconds(1f);
		VRUIs[5].SetActive(false);
		VRUIs[6].SetActive(true);
		xboxUIs[5].SetActive(false);
		xboxUIs[6].SetActive(true);
		yield return new WaitForSeconds(1f);
		VRUIs[6].SetActive(false);
		VRUIs[7].SetActive(true);
		xboxUIs[6].SetActive(false);
		xboxUIs[7].SetActive(true);
		yield return new WaitForSeconds(1f);
		VRUIs[7].SetActive(false);
		VRUIs[8].SetActive(true);
		xboxUIs[7].SetActive(false);
		xboxUIs[8].SetActive(true);
		yield return new WaitForSeconds(1f);
		VRUIs[8].SetActive(false);
		xboxUIs[8].SetActive(false);
	}
}

