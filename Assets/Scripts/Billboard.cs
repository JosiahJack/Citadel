using UnityEngine;
using System.Collections;

// Used by the TargetID instances to face the 3D text towards the player's camera.
public class Billboard : MonoBehaviour {
	// External references, optional
	public bool flip = false;

	// Internal references
	private Vector3 tempDir;

	void Update(){
		if (MouseLookScript.a.playerCamera.enabled == true) {
			tempDir = MouseLookScript.a.playerCamera.transform.forward;
			if (flip) tempDir = tempDir * -1f;
			transform.rotation = Quaternion.LookRotation(-tempDir);
		}
	}

	public void DestroySprite() {
		Utils.SafeDestroy(gameObject);
	}
}
