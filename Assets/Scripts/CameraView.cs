using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour {
	// External reference, required
	public Transform screenPoint;

	// External reference, optional
	// Added both of these for the bridge camera, only instance of having more
	// than one screen for the same camera.
	/*[DTValidator.Optional] */public Transform screenPoint2;
	/*[DTValidator.Optional] */public Transform screenPoint3;

	// Internal references
	private Camera cam;
	private const float tick = 0.1f;
	private float tickFinished; // Visual only, Time.time controlled
	private MeshRenderer mR; // These are the screens showing the feed
	private MeshRenderer mR2;
	private MeshRenderer mR3;

	void Start () {
		cam = GetComponent<Camera>();
		if (cam == null) Debug.Log("BUG: CameraView missing component for cam");
		else Utils.DisableCamera(cam);

		tickFinished = Time.time + tick;
		if (screenPoint == null) {
			Debug.Log("BUG: CameraView missing manually assigned reference for"
					  + " screenPoint");
		} else {
			mR = screenPoint.gameObject.GetComponent<MeshRenderer>();
		}

		if (mR == null) {
			Debug.Log("BUG: CameraView missing component for "
					  + "screenPoint.gameObject to assign to mR");
		}

		if (screenPoint2 != null) {
			mR2 = screenPoint2.gameObject.GetComponent<MeshRenderer>();
		}

		if (screenPoint3 != null) {
			mR3 = screenPoint3.gameObject.GetComponent<MeshRenderer>();
		}

		if (mR2 == null) mR2 = mR;
		if (mR3 == null) mR3 = mR;
	}

	void OnEnable() {
		DynamicCulling.AddCameraPosition(this);
		if (cam != null) cam.Render();
	}
	
	void OnDisable() {
		DynamicCulling.RemoveCameraPosition(this);
		
	}
	
	public bool IsVisible() {
		Vector2Int cellPos = DynamicCulling.a.PosToCellCoords(transform.position);
		if (DynamicCulling.a.XYPairInBounds(cellPos.x,cellPos.y)) {
			if (DynamicCulling.a.cullEnabled) {
				if (!DynamicCulling.a.GetPlayerCell().visible) return false;
			}
		}
		if (mR == null) return false;
		return mR.isVisible || mR2.isVisible || mR3.isVisible;
	}

	void Update() {
		if (!PauseScript.a.paused && !PauseScript.a.MenuActive()) {
			if (!IsVisible()) return;

			if (tickFinished < Time.time) {
				tickFinished = Time.time + tick;
				cam.Render();
			}
		}
	}
}
