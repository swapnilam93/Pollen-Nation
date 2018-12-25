using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpTextController : MonoBehaviour {

	private static PopUpText popUpText;
	private static PopUpText damageText;
	private static GameObject canvas;
	private static Camera camera;

	public static void Initialize() {
		canvas = GameObject.Find("Canvas");
		popUpText = Resources.Load<PopUpText>("PopUpText/PopUpTextParent");
		damageText = Resources.Load<PopUpText>("PopUpText/DamageTextParent");
		foreach (Camera c in Camera.allCameras) {
			if (c.gameObject.name == "Bee Camera")
				camera = c;
		}
	}

	public static void CreatePopUpText (string text, Transform location) {
		PopUpText instance = Instantiate(popUpText);
		Vector2 screenPosition = camera.WorldToScreenPoint(location.position);
		instance.transform.SetParent(canvas.transform, false);
		instance.transform.position = screenPosition;
		instance.SetText(text);
	}

	public static void CreateDamageText (string text, Transform location) {
		PopUpText instance = Instantiate(damageText);
		Vector2 screenPosition = camera.WorldToScreenPoint(location.position);
		instance.transform.SetParent(canvas.transform, false);
		instance.transform.position = screenPosition;
		instance.SetText(text);
	}
}
