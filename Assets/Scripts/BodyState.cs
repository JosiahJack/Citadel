using UnityEngine;
using System.Collections;

public class BodyState : MonoBehaviour {
	public float floatAbove = 0.08f;
	public Transform playerCapsuleTransform;
	//[HideInInspector]
	public bool collisionDetected = false;
	public bool collisionDebugAll = false;
	//[HideInInspector]
	public float colliderHeight;
	
	void  Start (){
		colliderHeight = playerCapsuleTransform.GetComponent<CapsuleCollider>().height;
	}

	public void LocalPositionSetY(Transform t, float s ) {
		Vector3 v = t.position;
		v.y  = s;
		t.position = v;
	}

	void  FixedUpdate (){
		//transform.position.x = playerCapsuleTransform.position.x;
		//transform.position.y = playerCapsuleTransform.position.y + ((colliderHeight * playerCapsuleTransform.localScale.y)/2) + (transform.localScale.y/2) + floatAbove;
		LocalPositionSetY(transform, (playerCapsuleTransform.position.y + ((colliderHeight * playerCapsuleTransform.localScale.y)/2) + (transform.localScale.y/2) + floatAbove));
		//transform.position.z = playerCapsuleTransform.position.z;
	}
	
	void  OnCollisionStay ( Collision collisionInfo  ){
		collisionDebugAll = true;
		if (collisionInfo.gameObject.CompareTag("Geometry")) {
			collisionDetected = true;
		}
	}
	
	void  OnCollisionExit (){
		collisionDetected = false;
	}
}