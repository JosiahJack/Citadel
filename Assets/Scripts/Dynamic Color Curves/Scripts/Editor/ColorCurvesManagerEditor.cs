using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(ColorCurvesManager))]
class ColorCurvesManagerEditor : Editor
{
	SerializedObject serObj;
	
	SerializedProperty Factor;

	SerializedProperty RedA;
	SerializedProperty GreenA;
	SerializedProperty BlueA;

	SerializedProperty RedB;
	SerializedProperty GreenB;
	SerializedProperty BlueB;

	SerializedProperty DepthRedChannelA;
	SerializedProperty DepthGreenChannelA;
	SerializedProperty DepthBlueChannelA;
	SerializedProperty ZCurveChannelA;

	SerializedProperty DepthRedChannelB;
	SerializedProperty DepthGreenChannelB;
	SerializedProperty DepthBlueChannelB;
	SerializedProperty ZCurveChannelB;

	SerializedProperty SaturationA;
	SerializedProperty SaturationB;

	SerializedProperty SelectiveFromColorA;
	SerializedProperty SelectiveToColorA;
	SerializedProperty SelectiveFromColorB;
	SerializedProperty SelectiveToColorB;
	
	void OnEnable ()
	{
		serObj = new SerializedObject (target);
		
		Factor = serObj.FindProperty ("Factor");

		SaturationA = serObj.FindProperty ("SaturationA");
		RedA = serObj.FindProperty ("RedA");
		GreenA = serObj.FindProperty ("GreenA");
		BlueA = serObj.FindProperty ("BlueA");

		SaturationB = serObj.FindProperty ("SaturationB");
		RedB = serObj.FindProperty ("RedB");
		GreenB = serObj.FindProperty ("GreenB");
		BlueB = serObj.FindProperty ("BlueB");

		DepthRedChannelA = serObj.FindProperty ("RedADepth");
		DepthGreenChannelA = serObj.FindProperty ("GreenADepth");
		DepthBlueChannelA = serObj.FindProperty ("BlueADepth");
		ZCurveChannelA = serObj.FindProperty ("ZCurveA");

		DepthRedChannelB = serObj.FindProperty ("RedBDepth");
		DepthGreenChannelB = serObj.FindProperty ("GreenBDepth");
		DepthBlueChannelB = serObj.FindProperty ("BlueBDepth");
		ZCurveChannelB = serObj.FindProperty ("ZCurveB");

		SelectiveFromColorA = serObj.FindProperty ("SelectiveFromColorA");
		SelectiveToColorA = serObj.FindProperty ("SelectiveToColorA");
		SelectiveFromColorB = serObj.FindProperty ("SelectiveFromColorB");
		SelectiveToColorB = serObj.FindProperty ("SelectiveToColorB");

		serObj.ApplyModifiedProperties ();

	}
	
	void CurveGui ( string name, SerializedProperty animationCurve, Color color)
	{
		// @NOTE: EditorGUILayout.CurveField is buggy and flickers, using PropertyField for now
		//animationCurve.animationCurveValue = EditorGUILayout.CurveField (GUIContent (name), animationCurve.animationCurveValue, color, Rect (0.0f,0.0f,1.0f,1.0f));
		EditorGUILayout.PropertyField (animationCurve, new GUIContent (name));
	}

	public override void OnInspectorGUI ()
	{
		serObj.Update ();
		
		GUILayout.Label ("Dynamicaly interpolate between configuration A and B", EditorStyles.miniBoldLabel);
		Factor.floatValue = EditorGUILayout.Slider( "Factor", Factor.floatValue, 0.0f, 1.0f);
		EditorGUILayout.Separator ();
		
		//	BeginCurves ();
		GUILayout.Label ("A configuration", EditorStyles.miniBoldLabel);
		SaturationA.floatValue = EditorGUILayout.Slider( " Saturation", SaturationA.floatValue, 0.0f, 5.0f);
		CurveGui (" Red", RedA, Color.red);
		CurveGui (" Green", GreenA, Color.green);
		CurveGui (" Blue", BlueA, Color.blue);

		if (((ColorCurvesManager)target).ScriptAdvancedMode())
		{
			EditorGUILayout.Separator();
			CurveGui (" Red (depth)", DepthRedChannelA, Color.red);
			CurveGui (" Green (depth)", DepthGreenChannelA, Color.green);
			CurveGui (" Blue (depth)", DepthBlueChannelA, Color.blue);
			EditorGUILayout.Separator();
			CurveGui (" Blend Curve", ZCurveChannelA, Color.grey);
		}

		if (((ColorCurvesManager)target).ScriptSelective())
		{
			EditorGUILayout.Separator ();
			EditorGUILayout.PropertyField (SelectiveFromColorA, new GUIContent (" Key"));
			EditorGUILayout.PropertyField (SelectiveToColorA, new GUIContent (" Target"));
		}

		GUILayout.Label ("B configuration", EditorStyles.miniBoldLabel);
		SaturationB.floatValue = EditorGUILayout.Slider( " Saturation", SaturationB.floatValue, 0.0f, 5.0f);
		CurveGui (" Red", RedB, Color.red);
		CurveGui (" Green", GreenB, Color.green);
		CurveGui (" Blue", BlueB, Color.blue);

		if (((ColorCurvesManager)target).ScriptAdvancedMode())
		{
			EditorGUILayout.Separator();
			CurveGui (" Red (depth)", DepthRedChannelB, Color.red);
			CurveGui (" Green (depth)", DepthGreenChannelB, Color.green);
			CurveGui (" Blue (depth)", DepthBlueChannelB, Color.blue);
			EditorGUILayout.Separator();
			CurveGui (" Blend Curve", ZCurveChannelB, Color.grey);
		}

		if (((ColorCurvesManager)target).ScriptSelective())
		{
			EditorGUILayout.Separator ();
			EditorGUILayout.PropertyField (SelectiveFromColorB, new GUIContent (" Key"));
			EditorGUILayout.PropertyField (SelectiveToColorB, new GUIContent (" Target"));
		}


		EditorGUILayout.Separator ();

		if (GUI.changed)
		{
			serObj.ApplyModifiedProperties ();
			(serObj.targetObject as ColorCurvesManager).gameObject.SendMessage ("EditorHasChanged");
		}
		else
			serObj.ApplyModifiedProperties ();
	}
}