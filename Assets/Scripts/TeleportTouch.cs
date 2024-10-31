using UnityEngine;
using System.Collections;

public class TeleportTouch : MonoBehaviour {
	public Transform targetDestination; // assign in the editor
	public bool playSound;
	public AudioClip SoundFX;
	public GameObject teleportFX;
	public float justUsed = 0f; // save
	private AudioSource SoundFXSource;

	void Awake () {
		SoundFXSource = GetComponent<AudioSource>();
		if (SoundFXSource != null) SoundFXSource.playOnAwake = false;
	}

	void  OnTriggerEnter ( Collider col  ) {
		if (targetDestination == null || col == null) return;
		if (col.gameObject == null) return;

		if (col.gameObject.CompareTag("Player")) {
			HealthManager hm = Utils.GetMainHealthManager(col.gameObject);
			if (hm != null) {
				if (hm.health > 0f && justUsed < PauseScript.a.relativeTime) {
					if (teleportFX != null) teleportFX.SetActive(true);
					col.transform.position = targetDestination.position;
					TeleportTouch tt = targetDestination.transform.gameObject.GetComponent<TeleportTouch>();
					if (tt != null) tt.justUsed = PauseScript.a.relativeTime + 1.0f;
					if (playSound) Utils.PlayOneShotSavable(SoundFXSource,SoundFX);
				}
			}
		}
	}

	public static string Save(GameObject go) {
		TeleportTouch tt = go.GetComponent<TeleportTouch>();
		return Utils.SaveRelativeTimeDifferential(tt.justUsed,"justUsed");
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		TeleportTouch tt = go.GetComponent<TeleportTouch>();
		tt.justUsed = Utils.LoadRelativeTimeDifferential(entries[index],"justUsed"); index++;
		return index;
	}
}
