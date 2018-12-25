using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomer : MonoBehaviour {

	public GameObject[] gameObjects;

	private Vector3 originalPos;
	private Vector3 lastPos;
	private float maxZoomOut;

	// Use this for initialization
	void Start () {
		maxZoomOut = gameObject.GetComponent<Camera>().orthographicSize;
		originalPos = gameObject.GetComponent<Camera>().transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float xMax = -10, zMax = -10, xMin = 10, zMin = 10;
		foreach (GameObject p in gameObjects) {
			if (p.GetComponentInChildren<Flower>() != null && p.GetComponentInChildren<Flower>().pollinationComplete)
				continue;
			if (xMax < p.transform.position.x) {
				xMax = p.transform.position.x;
			}
			if (zMax < p.transform.position.z) {
				zMax = p.transform.position.z;
			}
			if (xMin > p.transform.position.x) {
				xMin = p.transform.position.x;
			}
			if (zMin > p.transform.position.z) {
				zMin = p.transform.position.z;
			}
		}
		float z = (zMax+zMin)/2f;

		transform.localPosition = Vector3.Lerp(lastPos, new Vector3((xMax+xMin/2f), originalPos.y, z*1.5f-1f), Time.deltaTime * 10);
		lastPos = transform.localPosition;
		gameObject.GetComponent<Camera>().orthographicSize = (Mathf.Sqrt(Mathf.Pow((xMax - xMin), 2f) + Mathf.Pow((zMax - zMin), 2f)*2.2f)+3f)/13f*4f;
		StartCoroutine(Shake());
	}

	public IEnumerator Shake () {
		Vector3 originalPos = transform.localPosition;
		//float elapsed = 0.0f;
		//while (elapsed < duration) {
		float magnitude = 0.0f;
		for (int i = 0; i < 3; i++) {
			if (gameObjects[i].CompareTag("Stinger") && gameObjects[i].GetComponent<BeeController>().trapped) {
				magnitude += 0.04f;
			}
		}
			float x = Random.Range(-1f, 1f) * magnitude;
			float y = Random.Range(-1f, 1f) * magnitude;

			transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
			//elapsed += Time.deltaTime;
			yield return null;
		//}

		//transform.localPosition = originalPos;
	}
}
