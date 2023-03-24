using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFlash : MonoBehaviour {
	public Material normalMat;
	public Material alternateMat;
	public bool startFlashing = false;
	public bool startNormal = true;
	public bool stopReturnsToNormal = true;
	public float timeBetweenFlashes = 0.35f;
	public Light lit;
	private MeshRenderer meshR;
	private bool isFlashing = false;
	private float flashFinished; // Visual only, using Time.time
	private bool changeDone = false;
	private bool normal = true;

	void Start () {
		meshR = GetComponent<MeshRenderer>();
		if (normalMat == null) Debug.Log("BUG: MaterialFlash.cs has a null normal material!  Assign your materials!");
		if (alternateMat == null) Debug.Log("BUG: MaterialFlash.cs has a null alternate material!  Assign your materials!");

		if (startFlashing) isFlashing = true;
		flashFinished = Time.time;
		changeDone = false;
		normal = true;
		if (!startNormal) {
			if (lit != null) lit.enabled = true;
			meshR.material = alternateMat;
			normal = false;
		}
	}

    void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (Const.a.questData.SelfDestructActivated) isFlashing = true;

			if (isFlashing) {
				if (flashFinished < Time.time) {
					flashFinished = Time.time + timeBetweenFlashes;
					if (normal) {
						if (lit != null) lit.enabled = true;
						meshR.material = alternateMat;
						normal = !normal;
					} else {
						if (lit != null) lit.enabled = false;
						meshR.material = normalMat;
						normal = !normal;
					}
				}
			} else {
				if (stopReturnsToNormal && !changeDone) {
					if (lit != null) lit.enabled = false;
					changeDone = true;
					meshR.material = normalMat;
				}
			}
		}
    }

	public void StartFlashing() {
		isFlashing = true;
	}

	public void StopFlashing() {
		isFlashing = false;
	}
}
