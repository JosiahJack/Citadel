using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	public Camera mainCamera;
	private Vector3 tempDir;
	public bool flip = false;

	void Awake () {
		if (mainCamera == null) mainCamera = GetComponent<Camera> ();
		if (mainCamera == null) transform.gameObject.SetActive (false);
	}

	void Update(){
		if (mainCamera.enabled == true) {
			tempDir = mainCamera.transform.forward;
			if (flip) tempDir = tempDir * -1f;
			transform.rotation = Quaternion.LookRotation(-tempDir);
		}
	}

	public void DestroySprite() {
		Destroy(gameObject);
	}
}