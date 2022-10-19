using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Helper script to put a button in the inspector for me to click and run tests.
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

		if (GUILayout.Button("Output All Lights to File")) {
			testScript.GenerateLightsDataFile();
		}
		if (GUILayout.Button("Load Lights for\nSelected Level")) {
			testScript.LoadLevelLights();
		}
		if (GUILayout.Button("Unload Lights for\nSelected Level")) {
			testScript.UnloadLevelLights();
		}

		if (GUILayout.Button("Save Object")) {
			testScript.SaveObject();
		}

		if (GUILayout.Button("Load Object")) {
			testScript.LoadObject();
		}
	}
}
