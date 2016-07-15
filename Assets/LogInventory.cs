using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogInventory : MonoBehaviour {
	public bool[] hasLog;
	public bool[] readLog;
	public KeyCode uKey;
	public static LogInventory a;
	public int lastAddedIndex = -1;
	private AudioSource SFXSource;
	private AudioClip SFXClip;

	void Awake() {
		SFXSource = GetComponent<AudioSource>();
		a = this;
	}

	void Update () {
		if(Input.GetKeyDown(uKey)) {
			if (lastAddedIndex != -1) {
				SFXClip = Const.a.audioLogs[lastAddedIndex];
				SFXSource.PlayOneShot(SFXClip);
				lastAddedIndex = -1;
			}
		}
	}
}
