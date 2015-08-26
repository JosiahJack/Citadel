using UnityEngine;
using System.Collections;

public class AutomapPlayerIcon : MonoBehaviour {	
	public GameObject cameraObject;
	[HideInInspector]
	public MouseLookScript mlookScript;
	
	void  Start (){
		mlookScript = cameraObject.GetComponent<MouseLookScript>(); // Get the mouselookscript to reference rotation
	}
	
	void  FixedUpdate (){
		transform.rotation = Quaternion.Euler(0,0,(mlookScript.yRotation*(-1) + 180));  // Rotation adjusted for player view and direction vs UI space
	}
}