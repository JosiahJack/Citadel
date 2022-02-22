using UnityEngine;
using System.Collections;

public class CyberDecoy : MonoBehaviour {
	void OnEnable() {
		Const.a.decoyActive = true;
	}

	void OnDisable() {
		Const.a.decoyActive = false;
	}
}