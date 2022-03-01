using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tests))]
public class TestsEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		Tests testScript = (Tests)target;
		//if (GUILayout.Button("Run Unit Tests")) {
		//	testScript.RunUnits();
		//}
		if (GUILayout.Button(testScript.buttonLabel)) {
			testScript.Run();
		}
	}
}
