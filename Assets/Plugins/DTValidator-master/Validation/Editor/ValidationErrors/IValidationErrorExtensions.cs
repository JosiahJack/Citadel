#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

using UnityEditor;

namespace DTValidator.Internal {
	public static class IValidationErrorExtensions {
		// PRAGMA MARK - Public Interface
		public static Texture2D GetContextIcon(this IValidationError validationError) {
			if (validationError.ContextObject is GameObject) {
				return DTValidatorIcons.PrefabIcon;
			} else if (validationError.ContextObject is SceneAsset) {
				return DTValidatorIcons.SceneIcon;
			} else if (validationError.ContextObject is ScriptableObject) {
				return DTValidatorIcons.ScriptableObjectIcon;
			} else {
				Debug.LogWarning("Failed to get image because context object is not recognized type: " + validationError.ContextObject + "!");
				return Texture2DUtil.ClearTexture;
			}
		}

		public static string GetContextObjectName(this IValidationError validationError) {
			object context = validationError.ContextObject;
			if (context == null) {
				Debug.LogWarning("Cannot get name for null context! Error: " + validationError);
				return null;
			}

			UnityEngine.Object contextObject = context as UnityEngine.Object;
			if (contextObject == null) {
				Debug.LogWarning("Cannot get name of null UnityEngine.Object context: " + contextObject);
				return null;
			}

			string path = AssetDatabase.GetAssetPath(contextObject);
			return Path.GetFileName(path);
		}

		public static void SelectInEditor(this IValidationError validationError) {
			bool selected = SelectObjectInEditor(validationError);
			if (!selected) {
				SelectContextInEditor(validationError);
			}
		}


		// PRAGMA MARK - Internal
		private static bool SelectObjectInEditor(IValidationError validationError) {
			if (!typeof(UnityEngine.Component).IsAssignableFrom(validationError.ObjectType) && !typeof(GameObject).IsAssignableFrom(validationError.ObjectType)) {
				return false;
			}

			SceneAsset sceneAsset = validationError.ContextObject as SceneAsset;
			if (sceneAsset == null) {
				return false;
			}

			string contextScenePath = AssetDatabase.GetAssetPath(sceneAsset);
			Scene loadedScene = default(Scene);
			for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
				var scene = EditorSceneManager.GetSceneAt(i);
				if (scene.path == contextScenePath) {
					loadedScene = scene;
				}
			}

			if (!loadedScene.IsValid()) {
				return false;
			}

			HashSet<GameObject> rootGameObjects = new HashSet<GameObject>(loadedScene.GetRootGameObjects());

			UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(validationError.ObjectType);
			if (objects.Length <= 0) {
				return false;
			}

			IEnumerable<UnityEngine.Object> objectsInScene;
			if (validationError.ObjectType.Equals(typeof(UnityEngine.GameObject))) {
				objectsInScene = objects.Where(o => rootGameObjects.Contains((o as UnityEngine.GameObject).GetRoot()));
			} else {
				objectsInScene = objects.Where(o => rootGameObjects.Contains((o as UnityEngine.Component).gameObject.GetRoot()));
			}

			UnityEngine.Object obj = objectsInScene.FirstOrDefault(o => o.GetLocalId() == validationError.ObjectLocalId);
			Transform targetTransform = null;

			UnityEngine.Component objectAsComponent = obj as UnityEngine.Component;
			if (objectAsComponent != null) {
				targetTransform = objectAsComponent.transform;
			}

			GameObject objectAsGameObject = obj as GameObject;
			if (objectAsGameObject != null) {
				targetTransform = objectAsGameObject.transform;
			}

			if (targetTransform == null) {
				return false;
			}

			Selection.activeTransform = targetTransform;
			EditorGUIUtility.PingObject(targetTransform);
			return true;
		}

		private static void SelectContextInEditor(IValidationError validationError) {
			object context = validationError.ContextObject;
			if (context == null) {
				Debug.LogWarning("Cannot select context for null context! Error: " + validationError);
				return;
			}

			UnityEngine.Object contextObject = context as UnityEngine.Object;
			if (contextObject == null) {
				Debug.LogWarning("Cannot select null UnityEngine.Object context: " + contextObject);
				return;
			}

			Selection.activeObject = contextObject;
			EditorGUIUtility.PingObject(contextObject);
		}
	}
}
#endif
