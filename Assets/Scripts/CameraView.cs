using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour {
	private Camera cam;
	private float tick = 0.1f;
	private float tickFinished;
	public Transform screenPoint;
	private float maxDistVisible = 5.8f;
	private Transform playerT;
	private bool renderedOnce = false;

	void Start () {
		cam = GetComponent<Camera>();
		cam.enabled = false;
		renderedOnce = false;
		tickFinished = PauseScript.a.relativeTime + tick;
		playerT = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
	}

	void Update () {
		if (!PauseScript.a.paused && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (screenPoint != null) {
				if (Vector3.Distance(playerT.position,screenPoint.position) > maxDistVisible) {
					if (!renderedOnce) return;
				}
			}
			if (tickFinished < PauseScript.a.relativeTime) {
				tickFinished = PauseScript.a.relativeTime + tick;
				cam.Render();
				if (!renderedOnce) renderedOnce = true; // rendering once in Update to reduce load at start, should happen 0.1f seconds after game start
			}
		}
	}
}
