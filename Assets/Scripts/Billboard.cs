using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	public Camera mainCamera;
	private Vector3 tempDir;

	void Awake () {
		mainCamera = GetComponent<Camera> ();
		if (mainCamera == null)
			transform.gameObject.SetActive (false);
	}

	void Update(){
		if (mainCamera.enabled == true) {
			tempDir = mainCamera.transform.forward;
			transform.rotation = Quaternion.LookRotation(-tempDir);
		}
	}

	public void DestroySprite() {
		Destroy(gameObject);
	}
}