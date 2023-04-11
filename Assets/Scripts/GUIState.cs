using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Prevents shooting through the UI when using the UI on the HUD.
// Handles data about what is under the cursor for different button
// interactions such as right clicking as parsed by MouseLookScript.
public class GUIState : MonoBehaviour {
	[SerializeField] public bool isBlocking = false;
	public static GUIState a;
	public ButtonType overButtonType = ButtonType.None;
	public bool overButton;
	[HideInInspector] public GameObject currentButton;

	void Awake() {
		a = this;
		a.ClearOverButton();
	}

	public void PtrHandler(bool block, bool overState, ButtonType overType,
						   GameObject button) {
		isBlocking = block;
		overButton = overState;
		overButtonType = overType;
		currentButton = button;
	}

	public void ClearOverButton() {
		currentButton = null;
		overButton = false;
		overButtonType = ButtonType.None;
		isBlocking = false;
	}

	public static string Save(GameObject go) {
		GUIState guis = go.GetComponent<GUIState>();
		if (guis == null) {
			Debug.Log("GUIState missing!  GameObject.name: " + go.name);
			return "0|0";
		}

		string line = System.String.Empty;
		line = Utils.UintToString(Utils.ButtonTypeToInt(guis.overButtonType));
		line += Utils.splitChar + Utils.BoolToString(guis.overButton);
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		GUIState guis = go.GetComponent<GUIState>();
		if (guis == null) {
			Debug.Log("GUIState.Load failure, guis == null");
			return index + 2;
		}

		if (index < 0) {
			Debug.Log("GUIState.Load failure, index < 0");
			return index + 2;
		}

		if (entries == null) {
			Debug.Log("GUIState.Load failure, entries == null");
			return index + 2;
		}

		int type = Utils.GetIntFromString(entries[index]); index++;
		guis.overButtonType = Utils.IntToButtonType(type);
		guis.overButton = Utils.GetBoolFromString(entries[index]); index++;
		if (guis.overButton) guis.isBlocking = true;
		return index;
	}
}
