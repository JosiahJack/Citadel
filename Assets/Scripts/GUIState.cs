using UnityEngine;
using System.Collections;

public class GUIState : MonoBehaviour {
	[SerializeField]
	public bool isBlocking = false;
	public static GUIState a;

	void Awake() {a = this; }
}
