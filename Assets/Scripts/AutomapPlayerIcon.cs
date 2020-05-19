using UnityEngine;
using System.Collections;

public class AutomapPlayerIcon : MonoBehaviour {	
	public Transform pCapTransform;
	private float threshang = 0.5f;
	
	void  Update() {
		float zAdjusted = (pCapTransform.eulerAngles.y * (-1) + 90);
		if (Mathf.Abs(transform.localRotation.z-zAdjusted) > threshang) transform.localRotation = Quaternion.Euler(0,0,zAdjusted);  // Rotation adjusted for player view and direction vs UI space
	}
}