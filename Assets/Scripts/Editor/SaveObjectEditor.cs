using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveObject))]
public class SaveObjectEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		SaveObject soScript = (SaveObject)target;
		if (GUILayout.Button("SetSaveID")) {
			soScript.SetSaveID();
		}
	}
}
