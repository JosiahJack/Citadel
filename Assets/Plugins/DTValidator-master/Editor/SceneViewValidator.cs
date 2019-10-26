#if DT_VALIDATOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using DTValidator.Internal;
using DTValidator.ValidationErrors;

namespace DTValidator {
	[ExecuteInEditMode]
	[InitializeOnLoad]
	public static class SceneViewValidator {
		// PRAGMA MARK - Public Interface
		static SceneViewValidator() {
			if (DTValidatorPreferences.ValidateSceneAutomatically) {
				EditorApplication.delayCall += () => {
					StartValidating();
				};
			}
		}

		public static void StartValidating() {
			RefreshValidationErrors();

			EditorApplicationUtil.OnSceneGUIDelegate += OnSceneGUI;
			EditorApplicationUtil.SceneDirtied += RefreshValidationErrors;
			EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
		}

		public static void StopValidating() {
			// cleanup
			cachedValidationErrors_ = null;

			EditorApplicationUtil.OnSceneGUIDelegate -= OnSceneGUI;
			EditorApplicationUtil.SceneDirtied -= RefreshValidationErrors;
			EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
		}

		public static bool RefreshAndCheckValiationErrors() {
			Refresh();
			return cachedValidationErrors_ != null && cachedValidationErrors_.Count > 0;
		}

		public static void Refresh() {
			RefreshValidationErrors();
		}


		// PRAGMA MARK - Internal
		private static readonly Color kErrorColor = ColorUtil.HexStringToColor("#dc4d4d");

		private const float kErrorHeight = 20.0f;
		private const float kErrorSpacingHeight = 2.0f;
		private const float kErrorWidth = 275.0f;
		private const float kLinkWidth = 40.0f;
		private const float kLinkPadding = 3.0f;

		private static GUIStyle kButtonStyle_ = null;
		private static GUIStyle kButtonStyle {
			get {
				// NOTE (darren): sometimes the textures can get dealloc
				// and appear as nothing - we recreate them here
				if (kButtonStyle_ != null && kButtonStyle_.normal.background == null) {
					kButtonStyle_ = null;
				}

				if (kButtonStyle_ == null) {
					kButtonStyle_ = new GUIStyle(GUI.skin.GetStyle("Button"));
					kButtonStyle_.alignment = TextAnchor.MiddleRight;
					kButtonStyle_.padding.right = (int)(kLinkWidth + (2.0f * kLinkPadding) + 2);
					kButtonStyle_.padding.top = 3;
					kButtonStyle_.normal.textColor = kErrorColor;
					kButtonStyle_.normal.background = Texture2DUtil.GetCached1x1TextureWithColor(Color.black.WithAlpha(0.5f));
					kButtonStyle_.active.background = Texture2DUtil.GetCached1x1TextureWithColor(Color.black.WithAlpha(0.3f));
				}
				return kButtonStyle_;
			}
		}

		private const float kErrorIconPadding = 3.0f;

		private static IList<IValidationError> cachedValidationErrors_;
		private static HashSet<GameObject> objectsWithErrors_ = new HashSet<GameObject>();

		private static void OnSceneGUI(SceneView sceneView) {
			if (cachedValidationErrors_ == null) {
				return;
			}

			Handles.BeginGUI();

			// BEGIN SCENE GUI
			float yPosition = 0.0f;
			foreach (IValidationError error in cachedValidationErrors_) {
				var componentError = error as IComponentValidationError;

				// NOTE (darren): it's possible that OnSceneGUI gets called after
				// the prefab is destroyed - don't do anything in that case
				if (componentError == null || componentError.Component == null) {
					continue;
				}

				var linkRect = new Rect(kErrorWidth - kLinkWidth - kLinkPadding, yPosition + kLinkPadding, kLinkWidth, kErrorHeight - kLinkPadding);
				if (GUI.Button(linkRect, "Link")) {
					LinkValidationError(componentError);
				}

				var rect = new Rect(0.0f, yPosition, kErrorWidth, kErrorHeight);
				var errorDescription = string.Format("{0}->{1}.{2}", componentError.Component.gameObject.name, error.ObjectType.Name, error.MemberInfo.Name);

				if (GUI.Button(rect, errorDescription, kButtonStyle)) {
					Selection.activeGameObject = componentError.Component.gameObject;
				}

				if (GUI.Button(linkRect, "Link")) {
					// empty (no-action) button for the visual look
					// NOTE (darren): it appears the order in which GUI.button is
					// called determines the ordering for which button consumes the touch
					// but also the order is used to render :)
				}

				yPosition += kErrorHeight + kErrorSpacingHeight;
			}
			// END SCENE GUI
			Handles.EndGUI();
		}

		private static void OnHierarchyWindowItemOnGUI(int instanceId, Rect selectionRect) {
			if (Event.current.type != EventType.Repaint) {
				return;
			}

			GameObject g = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

			bool gameObjectHasError = objectsWithErrors_.Contains(g);
			if (gameObjectHasError) {
				float edgeLength = selectionRect.height - (2.0f * kErrorIconPadding);
				Rect errorIconRect = new Rect(selectionRect.x + selectionRect.width - kErrorIconPadding - edgeLength, selectionRect.y + kErrorIconPadding, edgeLength, edgeLength);
				GUI.DrawTexture(errorIconRect, DTValidatorIcons.ErrorIcon);
				EditorApplication.RepaintHierarchyWindow();
			}
		}

		private static void RefreshValidationErrors() {
			cachedValidationErrors_ = ValidationUtil.ValidateAllGameObjectsInLoadedScenes();

			objectsWithErrors_.Clear();
			if (cachedValidationErrors_ != null) {
				foreach (IComponentValidationError componentError in cachedValidationErrors_.Where(e => e is IComponentValidationError)) {
					objectsWithErrors_.Add(componentError.Component.gameObject);
				}
			}
		}

		private static void LinkValidationError(IComponentValidationError componentError) {
			var selectedGameObject = Selection.activeGameObject;
			if (selectedGameObject == null) {
				Debug.LogWarning("Cannot link when no selected game object!");
				return;
			}

			var fieldInfo = componentError.MemberInfo as FieldInfo;
			if (fieldInfo != null) {
				Type fieldType = fieldInfo.FieldType;

				Type fieldElementType = fieldType.IsArray ? fieldType.GetElementType() : fieldType;
				object elementValue = null;

				if (fieldElementType == typeof(UnityEngine.GameObject)) {
					elementValue = selectedGameObject;
				} else if (fieldElementType == typeof(UnityEngine.Transform)) {
					elementValue = selectedGameObject.transform;
				} else if (typeof(UnityEngine.Component).IsAssignableFrom(fieldElementType)) {
					var linkedComponent = selectedGameObject.GetComponent(fieldElementType);
					if (linkedComponent == null) {
						Debug.LogWarning("LinkValidationError: Failed to find component of type: " + fieldElementType.Name + " on selected game object, cannot link!");
						return;
					}

					elementValue = linkedComponent;
				} else {
					Debug.LogWarning("LinkValidationError: Field is of unhandled type: " + fieldElementType.Name + ", cannot link!");
					return;
				}

				if (fieldType.IsArray) {
					var indexedComponentValidationError = componentError as IndexedComponentValidationError;
					((IList)fieldInfo.GetValue(componentError.Component))[indexedComponentValidationError.Index] = elementValue;
				} else {
					fieldInfo.SetValue(componentError.Component, elementValue);
				}

				SceneView.RepaintAll();
				EditorUtility.SetDirty(componentError.Component);
				RefreshValidationErrors();
			}
		}
	}
}
#endif
