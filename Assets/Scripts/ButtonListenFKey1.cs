﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonListenFKey1 : MonoBehaviour {
	private Button button;
	public KeyCode Fkey;

	void Awake () {
		button = GetComponent<Button>();
	}
	
	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if(Input.GetKeyDown(Fkey))
				button.onClick.Invoke();
		}
	}
}
