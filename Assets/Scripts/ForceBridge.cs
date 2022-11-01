using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceBridge : MonoBehaviour {
    public bool x;
	public bool y;
	public bool z;
	public bool activated; // save
	public AudioClip SFXBridgeChange;
	public float tickTime = 0.05f;

	[HideInInspector] public bool lerping; // save
	[HideInInspector] public float tickFinished; // save

	private float activatedScaleX;
	private float activatedScaleY;
	private float activatedScaleZ;
	private MeshRenderer mr;
	private BoxCollider bCol;
	private AudioSource SFX;

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

		if (!isSilent) Utils.PlayOneShotSavable(SFX,SFXBridgeChange);
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

		if (!isSilent) Utils.PlayOneShotSavable(SFX,SFXBridgeChange);
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

	public static string Save(GameObject go) {
		ForceBridge fb = go.GetComponent<ForceBridge>();
		if (fb == null) {
			Debug.Log("ForceBridge missing on savetype of ForceBridge!  GameObject.name: " + go.name);
			return "1|0|0000.00000";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(fb.activated); // bool - is the bridge on?
		line += Utils.splitChar + Utils.BoolToString(fb.lerping); // bool - are we currently lerping one way or tother
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(fb.tickFinished); // float - time before firing targets
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		ForceBridge fb = go.GetComponent<ForceBridge>(); // fjb
		if (fb == null || index < 0 || entries == null) return index + 3;

		fb.activated = Utils.GetBoolFromString(entries[index]); index++; // bool - is the bridge on?
		fb.lerping = Utils.GetBoolFromString(entries[index]); index++; // bool - are we currently lerping one way or tother
		fb.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before thinking
		return index;
	}
}
