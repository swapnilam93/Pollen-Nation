using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class Stinger : MonoBehaviour {

    private AudioSource audioSource;

    public AudioClip[] stingLandedClip;

	[SerializeField] GameObject damageBlock;
    
    private void Start() {
        audioSource = GetComponent<AudioSource>();
        // TODO add objects to this list
		//damageBlock.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    // If stung by another bee, ignore it
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Stinger") {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroySting() {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}