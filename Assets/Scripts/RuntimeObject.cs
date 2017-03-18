using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeObject : MonoBehaviour {
	public bool isRuntimeObject = false;
	void Awake () {
		isRuntimeObject = true;  // Lets us know if this object is indeed not the prefab but rather an instance of a prefab
	}
}
