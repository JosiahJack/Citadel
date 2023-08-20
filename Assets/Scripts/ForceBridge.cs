using System.Collections;
using System.Collections.Generic;
using System.Text;
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

	public float activatedScaleX; // save
	public float activatedScaleY; // save
	public float activatedScaleZ; // save
	private MeshRenderer mr;
	private BoxCollider bCol;
	private AudioSource SFX;
	private static float tickTime = 0.05f;
	private bool initialized = false;

	public void Start() {
		if (!initialized) {
			tickFinished = PauseScript.a.relativeTime + tickTime + Random.value;
			lerping = true;
		}

		mr = GetComponent<MeshRenderer>();
		bCol = GetComponent<BoxCollider>();
		SFX = GetComponent<AudioSource>();
		SetColorMaterial();
	}

	public void InitializeFromLoad() {
		mr = GetComponent<MeshRenderer>();
		bCol = GetComponent<BoxCollider>();
		SFX = GetComponent<AudioSource>();
		SetColorMaterial();
		initialized = true;
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
			Utils.EnableMeshRenderer(mr);
			Utils.EnableBoxCollider(bCol);
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
					Utils.DisableMeshRenderer(mr);
					Utils.DisableBoxCollider(bCol);
					lerping = false;
				}
			}
		}
    }

	public void Activate(bool isSilent) {
		if (activated) return; // already there

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

	public void Deactivate(bool isSilent) {
		if (!activated) return; // already there

		if (!isSilent) Utils.PlayOneShotSavable(SFX,SFXBridgeChange);
		activated = false;
		lerping = true;
	}

	public void Toggle() {
		if (activated) {
			Deactivate(false);
		} else {
			Activate(false);
		}
	}

	public static string Save(GameObject go) {
		ForceBridge fb = go.GetComponent<ForceBridge>();
		if (fb == null) {
			Debug.Log("ForceBridge missing on savetype of ForceBridge!"
					  + "  GameObject.name: " + go.name);
			return "1|0|0000.00000";
		}

		fb.Start();
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.BoolToString(fb.activated,"ForceBridge.activated"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(fb.lerping,"lerping"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(fb.tickFinished,
													 "tickFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(fb.x,"x"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(fb.y,"y"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(fb.z,"z"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(fb.activatedScaleX));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(fb.activatedScaleY));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(fb.activatedScaleZ));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(Utils.ForceFieldColorToInt(fb.fieldColor),
															   "fieldColor"));
		return s1.ToString();
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

		fb.activated = Utils.GetBoolFromString(entries[index],"ForceBridge.activated");
		index++;

		fb.lerping = Utils.GetBoolFromString(entries[index],"lerping");
		index++;

		fb.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index],
															 "tickFinished");
		index++;

		fb.x = Utils.GetBoolFromString(entries[index],"x"); index++;
		fb.y = Utils.GetBoolFromString(entries[index],"y"); index++;
		fb.z = Utils.GetBoolFromString(entries[index],"z"); index++;

		fb.activatedScaleX = Utils.GetFloatFromString(entries[index]); index++;
		fb.activatedScaleY = Utils.GetFloatFromString(entries[index]); index++;
		fb.activatedScaleZ = Utils.GetFloatFromString(entries[index]); index++;

		if (fb.activated) {
			fb.transform.localScale = new Vector3(fb.activatedScaleX,
												fb.activatedScaleY,
												fb.activatedScaleZ);
		}

		fb.fieldColor = Utils.GetForceFieldColorFromInt(
						  Utils.GetIntFromString(entries[index],"fieldColor"));
		index++;

		fb.InitializeFromLoad();
		return index;
	}
}
