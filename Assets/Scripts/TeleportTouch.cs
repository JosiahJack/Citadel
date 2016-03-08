using UnityEngine;
using System.Collections;

public class TeleportTouch : MonoBehaviour {
	public Transform targetDestination; // assign in the editor
	public bool playSound;
	public AudioClip SoundFX;
	private AudioSource SoundFXSource;

	void Awake () {
		SoundFXSource = GetComponent<AudioSource>();
	}

	void  OnTriggerEnter ( Collider col  ){
		if ((col.gameObject.tag == "Player") & (col.gameObject.GetComponent<PlayerHealth>().health > 0f)) {
			col.transform.position = targetDestination.position;
			if (playSound)
				SoundFXSource.PlayOneShot(SoundFX);
		}
	}
}