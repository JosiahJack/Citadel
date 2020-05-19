using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIState : MonoBehaviour {
	[SerializeField]
	public bool isBlocking = false;
	public static GUIState a;
	public enum ButtonType {Generic,GeneralInv,Patch,Grenade,Weapon,Search,None,PGrid,PWire};
	public ButtonType overButtonType = ButtonType.None;
	public bool overButton;
	[HideInInspector] public GameObject currentButton;

	void Awake() {
		a = this;
		a.currentButton = null;
		a.overButton = false;
		a.overButtonType = ButtonType.None;
		a.isBlocking = false;
	}

	public void PtrHandler (bool block, bool overState, ButtonType overType,GameObject button) {
		isBlocking = block;
		overButton = overState;
		overButtonType = overType;
		currentButton = button;
	}
}
