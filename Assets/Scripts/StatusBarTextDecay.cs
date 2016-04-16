using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusBarTextDecay : MonoBehaviour {
	public float defaultDecayTime = 3.0f;
	private float decayFinished;
	private float flashTime;
	private Text statusText;
	private string tempString;
	private string oldString;
	private bool shouldFlash;

	void Awake () {
		decayFinished = Time.time;
		statusText = gameObject.GetComponent<Text>();
		oldString = "";
		shouldFlash = false;
	}

	public void SendText(string inputString) {
		if (tempString == "")
			return; // no text sent

		shouldFlash = false;
		decayFinished = Time.time + defaultDecayTime; // text stays on screen for next n seconds
		tempString = inputString;
		if (tempString == oldString) {
			statusText.text = "";
			flashTime = Time.time + 0.1f; // disable text for this time to indicate change to player
			shouldFlash = true;
		} else {
			statusText.text = tempString;
			oldString = tempString;
		}
	}

	void Update () {
		if (shouldFlash && (flashTime < Time.time))
			statusText.text = tempString; // re-enable text

		if (decayFinished < Time.time) {
			oldString = statusText.text;
			statusText.text = "";
		}
	}
}
