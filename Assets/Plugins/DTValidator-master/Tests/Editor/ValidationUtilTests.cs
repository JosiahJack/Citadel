using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace DTValidator.Internal {
	public static class ValidationUtilTests {
		[Test]
		public static void ValidateAllGameObjectsInScenes_WorksAsExpected() {
			Scene scene = EditorSceneManager.OpenScene(GetPathToScene("TestMissingOutletsScene"));
			IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInScenes(new Scene[] { scene });
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.GreaterThan(1));
		}

		[Test]
		public static void ValidateAllGameObjectsInScenes_EarlyExit_WorksAsExpected() {
			Scene scene = EditorSceneManager.OpenScene(GetPathToScene("TestMissingOutletsScene"));
			IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInScenes(new Scene[] { scene }, earlyExitOnError: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		[Test]
		public static void ValidateAllScriptableObjects_WorksAsExpected() {
			OutletScriptableObject objA = ScriptableObject.CreateInstance<OutletScriptableObject>();
			objA.Outlet = null;
			OutletScriptableObject objB = ScriptableObject.CreateInstance<OutletScriptableObject>();
			objB.Outlet = null;
			OutletScriptableObject objC = ScriptableObject.CreateInstance<OutletScriptableObject>();
			objC.Outlet = null;

			var scriptableObjects = new ScriptableObject[] { objA, objB, objC };

			IList<IValidationError> errors = ValidationUtil.ValidateAllScriptableObjects(scriptableObjects);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.GreaterThan(1));
		}

		[Test]
		public static void ValidateAllScriptableObjects_EarlyExit_WorksAsExpected() {
			OutletScriptableObject objA = ScriptableObject.CreateInstance<OutletScriptableObject>();
			objA.Outlet = null;
			OutletScriptableObject objB = ScriptableObject.CreateInstance<OutletScriptableObject>();
			objB.Outlet = null;
			OutletScriptableObject objC = ScriptableObject.CreateInstance<OutletScriptableObject>();
			objC.Outlet = null;

			var scriptableObjects = new ScriptableObject[] { objA, objB, objC };

			IList<IValidationError> errors = ValidationUtil.ValidateAllScriptableObjects(scriptableObjects, earlyExitOnError: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}


		private static string GetPathToScene(string sceneName) {
			string[] guids = AssetDatabase.FindAssets("t:Scene");
			foreach (string guid in guids) {
				string path = AssetDatabase.GUIDToAssetPath(guid);
				if (path.Contains(sceneName)) {
					return path;
				}
			}

			return null;
		}
	}
}
