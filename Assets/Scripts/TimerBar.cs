using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour {
	[SerializeField] Image pollinationBar;
	[SerializeField] GameController gameController;
	public Text ratioText;
	private float textSize;
	Vector3 startVector;

	// Use this for initialization
	void Start () {
		startVector = pollinationBar.rectTransform.localScale;
		textSize = pollinationBar.rectTransform.localScale.x;
	}

	public void Restart() {
		pollinationBar.rectTransform.localScale = startVector;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateTimerBar();	
	}

	void UpdateTimerBar() {
		float ratio = gameController.timeLeft / gameController.numSeconds * textSize;
		pollinationBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
		ratioText.text = gameController.timerText.text;
	}
}
