using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tests))]
public class TestsEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		Tests testScript = (Tests)target;
		if (GUILayout.Button("Run Setup Tests")) {
			testScript.Run();
		}
		if (GUILayout.Button("Run Unit Tests")) {
			testScript.RunUnits();
		}
	}
}
