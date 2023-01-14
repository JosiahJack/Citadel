using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceBridge : MonoBehaviour {
    public bool x; // save
	public bool y; // save
	public bool z; // save
	public bool activated; // save
	public AudioClip SFXBridgeChange;
	public ForceFieldColor fieldColor;

	[HideInInspector] public bool lerping; // save
	[HideInInspector] public float tickFinished; // save

	[HideInInspector] public float activatedScaleX; // save
	[HideInInspector] public float activatedScaleY; // save
	[HideInInspector] public float activatedScaleZ; // save
	private MeshRenderer mr;
	private BoxCollider bCol;
	private AudioSource SFX;
	private float tickTime = 0.05f;

	void Start() {
		activatedScaleX = transform.localScale.x;
		activatedScaleY = transform.localScale.y;
		activatedScaleZ = transform.localScale.z;
		mr = GetComponent<MeshRenderer>();
		bCol = GetComponent<BoxCollider>();
		SFX = GetComponent<AudioSource>();
		lerping = false;
		tickTime = 0.05f;
		tickFinished = PauseScript.a.relativeTime + tickTime + Random.value;
		if (activated) Activate(true,true);
		else Deactivate(true, true);

		SetColorMaterial();
	}

	public void SetColorMaterial() {
		switch (fieldColor) {
			case ForceFieldColor.Red:      mr.material = Const.a.genericMaterials[5];  break;
			case ForceFieldColor.Green:    mr.material = Const.a.genericMaterials[9];  break;
			case ForceFieldColor.Blue:     mr.material = Const.a.genericMaterials[8];  break;
			case ForceFieldColor.Purple:   mr.material = Const.a.genericMaterials[12]; break;
			case ForceFieldColor.RedFaint: mr.material = Const.a.genericMaterials[6];  break;
		}
	}

	void FixedUpdate() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;
		if (tickFinished >= PauseScript.a.relativeTime) return;

		tickFinished = PauseScript.a.relativeTime + tickTime;
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
		line += Utils.splitChar + Utils.BoolToString(fb.x); // bool - direction(s) to expand in
		line += Utils.splitChar + Utils.BoolToString(fb.y);
		line += Utils.splitChar + Utils.BoolToString(fb.z);
		line += Utils.splitChar + Utils.FloatToString(fb.activatedScaleX); // Current lerped value for expansion
		line += Utils.splitChar + Utils.FloatToString(fb.activatedScaleY);
		line += Utils.splitChar + Utils.FloatToString(fb.activatedScaleZ);
		line += Utils.splitChar + Utils.IntToString(Utils.ForceFieldColorToInt(fb.fieldColor));
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		ForceBridge fb = go.GetComponent<ForceBridge>(); // fjb
		if (fb == null) {
			Debug.Log("ForceBridge.Load failure, fb == null");
			return index + 3;
		}

		if (index < 0) {
			Debug.Log("ForceBridge.Load failure, index < 0");
			return index + 3;
		}

		if (entries == null) {
			Debug.Log("ForceBridge.Load failure, entries == null");
			return index + 3;
		}

		fb.activated = Utils.GetBoolFromString(entries[index]); index++; // bool - is the bridge on?
		fb.lerping = Utils.GetBoolFromString(entries[index]); index++; // bool - are we currently lerping one way or tother
		fb.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before thinking
		fb.x = Utils.GetBoolFromString(entries[index]); index++; // bool - direction(s) to expand in
		fb.y = Utils.GetBoolFromString(entries[index]); index++;
		fb.z = Utils.GetBoolFromString(entries[index]); index++;
		fb.activatedScaleX = Utils.GetFloatFromString(entries[index]); index++;
		fb.activatedScaleY = Utils.GetFloatFromString(entries[index]); index++;
		fb.activatedScaleZ = Utils.GetFloatFromString(entries[index]); index++;
		fb.fieldColor = Utils.GetForceFieldColorFromInt(Utils.GetIntFromString(entries[index])); index++;
		fb.Start();
		return index;
	}
}
