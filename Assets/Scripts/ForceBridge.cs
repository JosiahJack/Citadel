using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ForceBridge : MonoBehaviour {
    public bool x; // save
	public bool y; // save
	public bool z; // save
	public bool activated; // save
	public ForceFieldColor fieldColor;

	public bool lerping; // save
	[HideInInspector] public float tickFinished; // save

	public float activatedScaleX; // save
	public float activatedScaleY; // save
	public float activatedScaleZ; // save
	public MeshRenderer mr;
	[HideInInspector] public BoxCollider bCol;
	private AudioSource SFX;
	private static float tickTime = 0.05f;
	private bool initialized = false;
	[HideInInspector] public GameObject segiEmitter;

	public void Start() {
		#if UNITY_EDITOR
			if (!Application.isPlaying) return;
		#endif
		
		if (!initialized) {
			if (PauseScript.a == null) tickFinished = tickTime + Random.value;
			else tickFinished = PauseScript.a.relativeTime + tickTime + Random.value;
			
			lerping = true;
		}

		if (activatedScaleX <= 0.02f) activatedScaleX = 2.56f;
		if (activatedScaleY <= 0.02f) activatedScaleY = 0.08f;
		if (activatedScaleZ <= 0.02f) activatedScaleZ = 2.56f;
		InitializeFromLoad();
	}

	public void InitializeFromLoad() {
		mr = GetComponent<MeshRenderer>();
		bCol = GetComponent<BoxCollider>();
		SFX = GetComponent<AudioSource>();
		if (!activated) {
			Utils.DisableMeshRenderer(mr);
			Utils.DisableBoxCollider(bCol);
			Utils.Deactivate(segiEmitter);
		}
		
		SetColorMaterial();
		initialized = true;
		if (segiEmitter == null) segiEmitter = CreateSEGIEmitterCube();
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
			Utils.Activate(segiEmitter);
			if (lerping) {
				float sx = transform.localScale.x;
				float sy = transform.localScale.y;
				float sz = transform.localScale.z;
				if (x) sx = Mathf.Lerp(transform.localScale.x,activatedScaleX,tickTime*2);
				if (y) sy = Mathf.Lerp(transform.localScale.y,activatedScaleY,tickTime*2);
				if (z) sz = Mathf.Lerp(transform.localScale.z,activatedScaleZ,tickTime*2);
				transform.localScale = new Vector3(sx,sy,sz);
				if (   (activatedScaleX - sx) < 0.08f
					&& (activatedScaleY - sy) < 0.08f
					&& (activatedScaleZ - sz) < 0.08f) {

					transform.localScale = new Vector3(activatedScaleX,activatedScaleY,activatedScaleZ);
					lerping = false;
				}
			} else {
				if (transform.localScale.x != activatedScaleX) transform.localScale = new Vector3(activatedScaleX,activatedScaleY,activatedScaleZ);
				if (transform.localScale.y != activatedScaleY) transform.localScale = new Vector3(activatedScaleX,activatedScaleY,activatedScaleZ);
				if (transform.localScale.z != activatedScaleZ) transform.localScale = new Vector3(activatedScaleX,activatedScaleY,activatedScaleZ);
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
				if ((sx < 0.08f || sy < 0.08f || sz < 0.08f)) {
					Utils.DisableMeshRenderer(mr);
					Utils.DisableBoxCollider(bCol);
					Utils.Deactivate(segiEmitter);
					lerping = false;
				}
			}
		}
    }

	public void Activate(bool isSilent) {
		if (activated) return; // already there

		if (!isSilent) Utils.PlayOneShotSavable(SFX,Const.a.sounds[102]);
		Utils.EnableMeshRenderer(mr);
		Utils.EnableBoxCollider(bCol);
		Utils.Activate(segiEmitter);
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

		if (!isSilent) Utils.PlayOneShotSavable(SFX,Const.a.sounds[102]);
		if (segiEmitter != null) segiEmitter.SetActive(false);
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
	
	private GameObject CreateSEGIEmitterCube() {
		GameObject segiEmitter = new GameObject("ForceBridgeSEGIEmitter_"  + LevelManager.a.currentLevel.ToString() + "." + gameObject.name);
        segiEmitter.transform.parent = transform;
        segiEmitter.transform.localPosition = new Vector3(0f,0f,0f);
		segiEmitter.transform.localScale = new Vector3(1f,1f,1f); // Parent scales it
        MeshFilter mf = segiEmitter.AddComponent<MeshFilter>();
        mf.sharedMesh = Const.a.cubeMesh;
        MeshRenderer mR = segiEmitter.AddComponent<MeshRenderer>();
        mR.material = Const.a.segiEmitterMaterial1;
		switch (fieldColor) {
			case ForceFieldColor.Red:      mR.sharedMaterial = Const.a.segiEmitterMaterialRed;  break;
			case ForceFieldColor.Green:    mR.sharedMaterial = Const.a.segiEmitterMaterialGreen;  break;
			case ForceFieldColor.Blue:     mR.sharedMaterial = Const.a.segiEmitterMaterialBlue;  break;
			case ForceFieldColor.Purple:   mR.sharedMaterial = Const.a.segiEmitterMaterialPurple; break;
			case ForceFieldColor.RedFaint: mR.sharedMaterial = Const.a.segiEmitterMaterialRedFaint;  break;
		}

        segiEmitter.layer = 2; // IgnoreRaycast
        return segiEmitter;
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
		s1.Append(Utils.SaveRelativeTimeDifferential(fb.tickFinished,"tickFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(fb.x,"x"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(fb.y,"y"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(fb.z,"z"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(fb.activatedScaleX,"activatedScaleX"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(fb.activatedScaleY,"activatedScaleY"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(fb.activatedScaleZ,"activatedScaleZ"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(Utils.ForceFieldColorToInt(fb.fieldColor),"fieldColor"));
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
		if (fb.activated) {
			Utils.EnableMeshRenderer(fb.mr);
			Utils.EnableBoxCollider(fb.bCol);
			fb.transform.localScale = new Vector3(fb.activatedScaleX,fb.activatedScaleY,fb.activatedScaleZ);
		} else {
			if (fb.transform.localScale.x < 0.08f
				|| fb.transform.localScale.y < 0.08f
				|| fb.transform.localScale.z < 0.08f) {

				Utils.DisableMeshRenderer(fb.mr);
				Utils.DisableBoxCollider(fb.bCol);
			}
		}
		
		index++;
		fb.lerping = Utils.GetBoolFromString(entries[index],"lerping"); index++;
		fb.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index],"tickFinished"); index++;
		fb.x = Utils.GetBoolFromString(entries[index],"x"); index++;
		fb.y = Utils.GetBoolFromString(entries[index],"y"); index++;
		fb.z = Utils.GetBoolFromString(entries[index],"z"); index++;
		fb.activatedScaleX = Utils.GetFloatFromString(entries[index],"activatedScaleX"); index++;
		fb.activatedScaleY = Utils.GetFloatFromString(entries[index],"activatedScaleY"); index++;
		fb.activatedScaleZ = Utils.GetFloatFromString(entries[index],"activatedScaleZ"); index++;
		if (fb.activated) {
			fb.transform.localScale = new Vector3(fb.activatedScaleX,
												  fb.activatedScaleY,
												  fb.activatedScaleZ);
			fb.Activate(true);
		}

		fb.fieldColor = Utils.GetForceFieldColorFromInt(Utils.GetIntFromString(entries[index],"fieldColor"));
		index++;
		fb.InitializeFromLoad();
		return index;
	}
}
