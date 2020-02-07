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
	private MeshRenderer meshR;
	private bool isFlashing = false;
	private float flashFinished;
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
			meshR.material = alternateMat;
			normal = false;
		}
	}

    void Update() {
        if (isFlashing) {
			if (flashFinished < Time.time) {
				flashFinished = Time.time + timeBetweenFlashes;
				if (normal) {
					meshR.material = alternateMat;
					normal = !normal;
				} else {
					meshR.material = normalMat;
					normal = !normal;
				}
			}
		} else {
			if (stopReturnsToNormal && !changeDone) {
				changeDone = true;
				meshR.material = normalMat;
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
