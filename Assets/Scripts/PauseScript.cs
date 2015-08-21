using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseScript : MonoBehaviour {
	public Text pauseText;
	public bool paused = false;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			PauseToggle();
		}
	}

	public void PauseToggle() {
		if (paused == true)
			PauseDisable();
		else
			PauseEnable();
	}

	void PauseEnable() {
		paused = true;
		pauseText.enabled = true;
	}

	void PauseDisable() {
		paused = false;
		pauseText.enabled = false;
	}
}
