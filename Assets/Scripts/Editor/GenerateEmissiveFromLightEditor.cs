using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightGeneration))]
public class GenerateEmissiveFromLightEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		LightGeneration testScript = (LightGeneration)target;
		if (GUILayout.Button("Reset Selected")) {
			testScript.Reset_Selected();
		}
		// if (GUILayout.Button("Generate Lighting For Selected")) {
			// testScript.GenerateLighting_Selected();
		// }

		if (GUILayout.Button("Generate Lighting For All Selected")) {
			testScript.GenerateLighting_MultipleSelectionHandler();
		}
	}
}
