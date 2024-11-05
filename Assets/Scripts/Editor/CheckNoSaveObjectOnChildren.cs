#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Trioptimum {
	public class CheckNoSaveObjectOnChildren : EditorWindow {
		[MenuItem("Tools/Report SaveObjects On Children of Selected")]
		private static void CheckSaveObjectScriptsOnChildrenOfSelected() {
			int numFound = 0;
			int numInstantiated = 0;
			int numStatic = 0;
			Transform[] allParents = Selection.transforms;
			for (int i=0;i<allParents.Length;i++) {
				// Find all SaveObject components, including inactive, hence
				// the true as final argument.
				GameObject go = allParents[i].gameObject;
				Component[] compArray = go.GetComponentsInChildren(
											 typeof(SaveObject),true);
				
				for (int k=0;k<compArray.Length;k++) {
					numFound++;
					GameObject foundGo = compArray[k].gameObject;
					SaveObject so = SaveLoad.GetPrefabSaveObject(foundGo);
					if (so.instantiated) numInstantiated++;
					if (foundGo.isStatic) { // EDITOR ONLY!!!!!!!!!!!!!!!!!!!!
						numStatic++;
						//Debug.Log("Static: " + foundGo.name);
					}
				}

				Debug.Log("First Order Children = "
						  + allParents[i].transform.childCount.ToString());
			}

			Debug.Log(numFound.ToString() + " SaveObject.cs scripts found "
					  + "attached to selected GameObjects and children of "
					  + "each. (Instantiated = " + numInstantiated.ToString()
					  + ") (Static = " + numStatic.ToString() + ")");
		}
	}
}

#endif
