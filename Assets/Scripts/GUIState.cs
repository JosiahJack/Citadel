using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GUIState : MonoBehaviour {
	[SerializeField] public bool isBlocking = false;
	public static GUIState a;
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

	public static string Save(GameObject go) {
		GUIState guis = go.GetComponent<GUIState>();
		if (guis == null) {
			Debug.Log("GUIState missing!  GameObject.name: " + go.name);
			return "0|0";
		}

		string line = System.String.Empty;
		line = Utils.UintToString(Utils.ButtonTypeToInt(GUIState.a.overButtonType)); // int
		line += Utils.splitChar + Utils.BoolToString(GUIState.a.overButton); // bool
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		GUIState guis = go.GetComponent<GUIState>();
		if (guis == null || index < 0 || entries == null) return index + 2;

		int type = Utils.GetIntFromString(entries[index]); index++;
		guis.overButtonType = Utils.IntToButtonType(type);
		guis.overButton = Utils.GetBoolFromString(entries[index]); index++;
		return index;
	}
}
