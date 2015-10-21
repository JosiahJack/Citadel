using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPointerMask : MonoBehaviour {
	public void PtrEnter () {
		GUIState.isBlocking = true;
	}
	
	public void PtrExit () {
		GUIState.isBlocking = false;
	}
}
