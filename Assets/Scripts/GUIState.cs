using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

// Prevents shooting through the UI when using the UI on the HUD.
// Handles data about what is under the cursor for different button
// interactions such as right clicking as parsed by MouseLookScript.
public class GUIState : MonoBehaviour {
	[SerializeField] public bool isBlocking = false;
	public static GUIState a;
	public ButtonType overButtonType = ButtonType.None;
	public bool overButton;
	[HideInInspector] public GameObject currentButton;
	private static StringBuilder s1 = new StringBuilder();

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
		EventSystem.current.SetSelectedGameObject(null); // ENTER SHOULD NOT INTERACT WITH THE HUD!!!
	}

	public void ClearOverButton() {
		currentButton = null;
		overButton = false;
		overButtonType = ButtonType.None;
		isBlocking = false;
		EventSystem.current.SetSelectedGameObject(null);
	}

	public static string Save(GameObject go) {
		GUIState guis = go.GetComponent<GUIState>();
		if (guis == null) {
			Debug.Log("GUIState missing!  GameObject.name: " + go.name);
			return "0|0";
		}

		s1.Clear();
		s1.Append(Utils.UintToString(Utils.ButtonTypeToInt(guis.overButtonType),"overButtonType"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(guis.overButton,"overButton"));
		return s1.ToString();
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

		int type = Utils.GetIntFromString(entries[index],"overButtonType"); index++;
		guis.overButtonType = Utils.IntToButtonType(type);
		guis.overButton = Utils.GetBoolFromString(entries[index],"overButton"); index++;
		if (guis.overButton) guis.isBlocking = true;
		return index;
	}
}
