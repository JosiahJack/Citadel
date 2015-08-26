using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	public GameObject cameraObject;
	public Vector3 lookPos;
	public float damping = 1;
	
	void  Update (){
		lookPos = transform.position - cameraObject.transform.position;
		
		Quaternion rotationCorrection = Quaternion.LookRotation(lookPos);
		rotationCorrection *= Quaternion.Euler(0, -90, 90);
		//transform.rotation = Quaternion.Slerp(transform.rotation, rotationCorrection, Time.deltaTime * damping);
		transform.rotation = rotationCorrection;
	}
}