using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	void  Update (){
		Vector3 dir = Camera.main.transform.forward;
		//dir.y = 0.0f;
		transform.rotation = Quaternion.LookRotation(-dir);
	}

	public void DestroySprite() {
		Destroy(gameObject);
	}
}