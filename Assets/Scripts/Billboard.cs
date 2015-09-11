using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	void  Update (){
		if (Camera.main != null) {
			Camera mainCamera = Camera.main;
			if (mainCamera.enabled = true) {
				Vector3 dir = mainCamera.transform.forward;
				transform.rotation = Quaternion.LookRotation(-dir);
			}
		}
	}

	public void DestroySprite() {
		Destroy(gameObject);
	}
}