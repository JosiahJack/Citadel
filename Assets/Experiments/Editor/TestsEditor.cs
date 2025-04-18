﻿#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Helper script to put a button in the inspector for me to click and run tests.
[CustomEditor(typeof(CitadelTests))]
public class TestsEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		CitadelTests testScript = (CitadelTests)target;
		if (GUILayout.Button("Save Reflection Probe Image")) testScript.SaveReflectionProbeImage();
		//if (GUILayout.Button(testScript.buttonLabel)) {
		//	testScript.Run();
		//}

		//EditorGUILayout.BeginVertical();
        //GUILayout.Space(8f);
        //EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false,1f), new Color(0f, 0f, 0f, 0.8f));
        //GUILayout.Space(8f);
		//EditorGUILayout.EndVertical();
		
		// Already output, hiding to not accidentally overwrite!!
// 		if (GUILayout.Button("Output All Geometry to File")) {
// 			testScript.GenerateGeometryDataFile();
// 		}
// 		if (GUILayout.Button("TEMP: Correct light data files")) {
// 			testScript.CorrectLightDataFiles();
// 		}
// 		if (GUILayout.Button("TEMP: Correct dynamics data files")) {
// 			testScript.CorrectDynamicDataFiles();
// 		}
// 		if (GUILayout.Button("Check all UseHandler's to see if they use their bools at all")) {
// 			testScript.CheckUseHandlers();
// 		}
		if (GUILayout.Button("Output All Static Objects Immutable to File")) {
			testScript.GenerateStaticObjectsImmutableDataFile();
		}
		
		// Already output, hiding to not accidentally overwrite!!	
// 		if (GUILayout.Button("Output All Lights to File")) {
// 			testScript.GenerateLightsDataFile();
// 		}

// 		if (GUILayout.Button("Load Lights for\nSelected Level")) {
// 			testScript.LoadLevelLights();
// 		}
// 		if (GUILayout.Button("Unload Lights for\nSelected Level")) {
// 			testScript.UnloadLevelLights();
// 		}

// 		EditorGUILayout.BeginVertical();
//         GUILayout.Space(8f);
//         EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false,1f), new Color(0f, 0f, 0f, 0.8f));
//         GUILayout.Space(8f);
// 		EditorGUILayout.EndVertical();
// 
// 		if (GUILayout.Button("Save Object")) {
// 			testScript.SaveSelectedObject();
// 		}
// 
// 		if (GUILayout.Button("Load Object")) {
// 			testScript.LoadSelectedObject();
// 		}

// 		EditorGUILayout.BeginVertical();
//         GUILayout.Space(8f);
//         EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false,1f), new Color(0f, 0f, 0f, 0.8f));
//         GUILayout.Space(8f);
// 		EditorGUILayout.EndVertical();
// 
// 		if (GUILayout.Button("Output All Dynamic Objects to File")) {
// 			testScript.GenerateDynamicObjectsDataFile();
// 		}

// 		if (GUILayout.Button("Load Dynamic Objects for\nSelected Level")) {
// 			testScript.LoadLevelLights();
// 		}
// 
// 		if (GUILayout.Button("Unload Dynamic Objects for\nSelected Level")) {
// 			testScript.UnloadLevelLights();
// 		}

// 		EditorGUILayout.BeginVertical();
//         GUILayout.Space(8f);
//         EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false,1f), new Color(0f, 0f, 0f, 0.8f));
//         GUILayout.Space(8f);
// 		EditorGUILayout.EndVertical();

// 		if (GUILayout.Button("Set ChunkIDs array for all func_wall's")) {
// 			testScript.TEMP_SetFunc_WallChunkIDs();
// 		}

// 		EditorGUILayout.BeginVertical();
//         GUILayout.Space(8f);
//         EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false,1f), new Color(0f, 0f, 0f, 0.8f));
//         GUILayout.Space(8f);
// 		EditorGUILayout.EndVertical();
// 
// 		if (GUILayout.Button("Reset SaveIDs on Static Saveables")) {
// 			testScript.SetStaticSaveableIDs();
// 		}
// 
// 		if (GUILayout.Button("Report Max SaveIDs on Static Saveables")) {
// 			testScript.ReportMaxSaveableID();
// 		}

		//EditorGUILayout.BeginVertical();
        //GUILayout.Space(8f);
        //EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false,1f), new Color(0f, 0f, 0f, 0.8f));
        //GUILayout.Space(8f);
		//EditorGUILayout.EndVertical();

		//if (GUILayout.Button("Revert All m_CastShadows")) {
		//	testScript.RevertAll_m_CastShadows();
		//}
	}
}

#endif
