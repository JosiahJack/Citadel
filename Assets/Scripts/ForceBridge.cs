﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceBridge : MonoBehaviour {
    public bool x;
	public bool y;
	public bool z;
	private float activatedScaleX;
	private float activatedScaleY;
	private float activatedScaleZ;
	private MeshRenderer mr;
	private BoxCollider bCol;
	public bool activated; // save
	public AudioClip SFXBridgeChange;
	private AudioSource SFX;
	[HideInInspector]
	public bool lerping; // save
	[HideInInspector]
	public float tickFinished; // save
	public float tickTime = 0.05f;

	void Start() {
		activatedScaleX = transform.localScale.x;
		activatedScaleY = transform.localScale.y;
		activatedScaleZ = transform.localScale.z;
		mr = GetComponent<MeshRenderer>();
		bCol = GetComponent<BoxCollider>();
		SFX = GetComponent<AudioSource>();
		lerping = false;
		tickFinished = PauseScript.a.relativeTime + tickTime + Random.value;
		if (activated) Activate(true,true);
		else Deactivate(true, true);
	}

	void Update() {
		if (!gameObject.activeSelf) return;
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (tickFinished < PauseScript.a.relativeTime) {
				if (activated) {
					if (mr.enabled == false) mr.enabled = true;
					if (bCol.enabled == false) bCol.enabled = true;
					if (lerping) {
						float sx = transform.localScale.x;
						float sy = transform.localScale.y;
						float sz = transform.localScale.z;
						if (x) sx = Mathf.Lerp(transform.localScale.x,activatedScaleX,tickTime*2);
						if (y) sy = Mathf.Lerp(transform.localScale.y,activatedScaleY,tickTime*2);
						if (z) sz = Mathf.Lerp(transform.localScale.z,activatedScaleZ,tickTime*2);
						transform.localScale = new Vector3(sx,sy,sz);
						if ((activatedScaleX-transform.localScale.x) < 0.08f && (activatedScaleY-transform.localScale.y) < 0.08f && (activatedScaleZ-transform.localScale.z) < 0.08f) {
							transform.localScale = new Vector3(activatedScaleX,activatedScaleY,activatedScaleZ);
							lerping = false;
						}
					}
				} else {
					if (lerping) {
						// lerp scale down on deactivate
						float sx = transform.localScale.x;
						float sy = transform.localScale.y;
						float sz = transform.localScale.z;
						if (x) sx = Mathf.Lerp(transform.localScale.x,0f,tickTime*2);
						if (y) sy = Mathf.Lerp(transform.localScale.y,0f,tickTime*2);
						if (z) sz = Mathf.Lerp(transform.localScale.z,0f,tickTime*2);
						transform.localScale = new Vector3(sx,sy,sz);
						if (transform.localScale.x < 0.08f || transform.localScale.y < 0.08f || transform.localScale.z < 0.08f) {
							if (mr.enabled) mr.enabled = false;
							if (bCol.enabled) bCol.enabled = false;
							lerping = false;
						}
					}
				}
				tickFinished = PauseScript.a.relativeTime + tickTime;
			}
		}
    }

	public void Activate(bool forceIt, bool isSilent) {
		if (activated && !forceIt) return; // already there

		if (SFX != null && SFXBridgeChange != null && !isSilent) SFX.PlayOneShot(SFXBridgeChange);
		activated = true;
		lerping = true;
		float sx = activatedScaleX;
		float sy = activatedScaleY;
		float sz = activatedScaleZ;
		if (x) sx = 0.1f;
		if (y) sy = 0.1f;
		if (z) sz = 0.1f;
		transform.localScale = new Vector3(sx,sy,sz);
	}

	public void Deactivate(bool forceIt, bool isSilent) {
		if (!activated && !forceIt) return; // already there

		if (!isSilent && SFX != null && SFXBridgeChange != null) SFX.PlayOneShot(SFXBridgeChange);
		activated = false;
		lerping = true;
	}

	public void Toggle() {
		if (activated) {
			Deactivate(false, false);
		} else {
			Activate(false,false);
		}
	}
}