using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour {
	private Camera cam;
	private float tick = 0.1f;
	private float tickFinished; // Visual only, Time.time controlled
	public Transform screenPoint;
	private float maxDistVisible = 5.8f;
	private Transform playerT;
	private bool renderedOnce = false;
	private MeshRenderer mR;
	private bool mRpresent = false;

	void Start () {
		cam = GetComponent<Camera>();
		cam.enabled = false;
		renderedOnce = false;
		tickFinished = Time.time + tick;
		playerT = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		mRpresent = false;
		mR = GetComponent<MeshRenderer>();
		if (mR != null) mRpresent = true;
	}

	void Update () {
		if (!PauseScript.a.paused && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (mRpresent) {
				if (!mR.isVisible) return;
			}
			if (screenPoint != null) {
				if (Vector3.Distance(playerT.position,screenPoint.position) > maxDistVisible) {
					if (!renderedOnce) return;
				}
			}
			if (tickFinished < Time.time) {
				tickFinished = Time.time + tick;
				cam.Render();
				if (!renderedOnce) renderedOnce = true; // rendering once in Update to reduce load at start, should happen 0.1f seconds after game start
			}
		}
	}
}
