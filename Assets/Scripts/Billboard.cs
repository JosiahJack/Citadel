using UnityEngine;
using System.Collections;

// Used by the TargetID instances to face the 3D text towards the player's camera.
public class Billboard : MonoBehaviour {
	// External references, required
	public Camera mainCamera;

	// External references, optional
	public bool flip = false;

	// Internal references
	private Vector3 tempDir;

	void Awake () {
		if (mainCamera == null) mainCamera = GetComponent<Camera> ();
		if (mainCamera == null) { 
			Debug.Log("BUG: Billboard missing manually assigned reference "
					  + "for mainCamera");
			gameObject.SetActive(false);
		}
	}

	void Update(){
		if (mainCamera.enabled == true) {
			tempDir = mainCamera.transform.forward;
			if (flip) tempDir = tempDir * -1f;
			transform.rotation = Quaternion.LookRotation(-tempDir);
		}
	}

	public void DestroySprite() {
		Utils.SafeDestroy(gameObject);
	}
}