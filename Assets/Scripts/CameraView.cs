using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour {
	private Camera cam;
	private float tick;
	private float tickFinished;
	//private bool frameOn;

	void Awake () {
		cam = GetComponent<Camera>();
		cam.enabled = false;
		cam.Render();
		//tick = 10f;
		//tickFinished = Time.time + tick + (Random.value * tick);
		//frameOn = false;
	}

	// Update is called once per frame
	//void Update () {
		//if (PauseScript.a.paused || PauseScript.a.mainMenu.activeSelf == true) {
		//	cam.enabled = false;
		//} else {
		//	cam.enabled = true;
		//}
    //    if (PauseScript.a.mainMenu.activeSelf == true) {
	//		return; // don't render, the menu is up
	//	}

	//	if (tickFinished < Time.time) {
			//cam.enabled = true;
			//frameOn = true;
	//		tickFinished = Time.time + tick;
	//		cam.Render();
	//	}
	//}

	//void LateUpdate() {
	//	if (frameOn) cam.enabled = false; // Render to texture for 1 frame only!
	//}
}
