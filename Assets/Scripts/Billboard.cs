using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	void  Update (){
		Vector3 dir = Camera.main.transform.forward;
		transform.rotation = Quaternion.LookRotation(-dir);
	}

	public void DestroySprite() {
		Destroy(gameObject);
	}
}