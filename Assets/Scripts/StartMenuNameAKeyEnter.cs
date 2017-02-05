using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartMenuNameAKeyEnter : MonoBehaviour {
	public InputField ip;

	void ClickViaKeyboard () {
		if (ip != null) {
			EventSystem.current.SetSelectedGameObject(ip.gameObject,null);
			ip.ActivateInputField();
		}
	}

	void InputFieldFocus () {
		ClickViaKeyboard();
	}

	void InputFieldCancelFocus () {
		if (ip != null) {
			ip.DeactivateInputField();
		}
	}
}
