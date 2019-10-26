#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

using DTValidator.Internal;
using DTValidator.ValidationErrors;

namespace DTValidator {
	public class ValidatorWindow : EditorWindow {
		// PRAGMA MARK - Static Public Interface
		[MenuItem("Window/DTValidator Window", priority=100)]
		public static void ShowWindow() {
			var window = EditorWindow.GetWindow<ValidatorWindow>(utility: false, title: "Validator Window");
			window.minSize = new Vector2(kMinWindowWidth, kMinWindowHeight);
		}


		// PRAGMA MARK - Internal
		private const float kMinWindowWidth = 410.0f;
		private const float kMinWindowHeight = 120.0f;

		private const float kTopBarSize = 20.0f;

		private static Color? kErrorEvenColor_ = null;
		private static Color? kErrorEvenColor {
			get { return kErrorEvenColor_ ?? (kErrorEvenColor_ = EditorGUIUtility.isProSkin ? ColorUtil.HexStringToColor("#4a1515") : ColorUtil.HexStringToColor("#ff6969")); }
		}

		private static Color? kErrorOddColor_ = null;
		private static Color? kErrorOddColor {
			get { return kErrorOddColor_ ?? (kErrorOddColor_ = EditorGUIUtility.isProSkin ? ColorUtil.HexStringToColor("#3b1a1a") : ColorUtil.HexStringToColor("#f05555")); }
		}

		private readonly List<IValidationError> validationErrors_ = new List<IValidationError>();
		private float? validationTimeInMS_ = null;

        private static Vector2 ScrollPosition_ {
            get {
                if (!EditorPrefs.HasKey("ValidatorWindow::scrollPosition_")) {
                    return Vector2.zero;
                }

                return JsonUtility.FromJson<Vector2>(EditorPrefs.GetString("ValidatorWindow::scrollPosition_", ""));
            }
            set { EditorPrefs.SetString("ValidatorWindow::scrollPosition_", JsonUtility.ToJson(value)); }
        }

		private static bool ValidateScriptableObjects_ {
			get { return EditorPrefs.GetBool("ValidatorWindow::ValidateScriptableObjects_", defaultValue: true); }
			set { EditorPrefs.SetBool("ValidatorWindow::ValidateScriptableObjects_", value); }
		}

        private static bool ValidateOpenScenes_
        {
            get { return EditorPrefs.GetBool("ValidatorWindow::ValidateOpenScenes_", defaultValue: true); }
            set {
                if (value)
                {
                    ValidateSavedScenes_ = false;
                    ValidateBuildScenes_ = false;
                }
                EditorPrefs.SetBool("ValidatorWindow::ValidateOpenScenes_", value);
            }
        }

        private static bool ValidateBuildScenes_
        {
            get { return EditorPrefs.GetBool("ValidatorWindow::ValidateBuildScenes_", defaultValue: true); }
            set
            {
                if (value)
                {
                    ValidateSavedScenes_ = false;
                    ValidateOpenScenes_ = false;
                }
                EditorPrefs.SetBool("ValidatorWindow::ValidateBuildScenes_", value);
            }
        }

        private static bool ValidateSavedScenes_ {
			get { return EditorPrefs.GetBool("ValidatorWindow::ValidateSavedScenes_", defaultValue: true); }
			set {
                if (value)
                {
                    ValidateBuildScenes_ = false;
                    ValidateOpenScenes_ = false;
                }
                EditorPrefs.SetBool("ValidatorWindow::ValidateSavedScenes_", value);
            }
		}

		private static bool ValidatePrefabsInResources_ {
			get { return EditorPrefs.GetBool("ValidatorWindow::ValidatePrefabsInResources_", defaultValue: true); }
			set { EditorPrefs.SetBool("ValidatorWindow::ValidatePrefabsInResources_", value); }
		}

        private void OnGUI() {
            EditorGUILayout.BeginVertical(GUILayout.Height(kTopBarSize));
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Validate:", GUILayout.Width(50.0f));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                    ValidateScriptableObjects_ = GUILayout.Toggle(ValidateScriptableObjects_, "Scriptable Objects", GUILayout.ExpandWidth(false));
                    ValidatePrefabsInResources_ = GUILayout.Toggle(ValidatePrefabsInResources_, "Prefabs in Resources", GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                    ValidateSavedScenes_ = GUILayout.Toggle(ValidateSavedScenes_, "Saved Scenes", GUILayout.ExpandWidth(false));
                    ValidateBuildScenes_ = GUILayout.Toggle(ValidateBuildScenes_, "Build Scenes", GUILayout.ExpandWidth(false));
                    ValidateOpenScenes_ = GUILayout.Toggle(ValidateOpenScenes_, "Open Scenes", GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
					string buttonTitle = "Validate!";
					if (validationTimeInMS_ != null) {
						buttonTitle += string.Format(" ({0:0.00}s)", validationTimeInMS_.Value / 1000.0f);
					}
					if (GUILayout.Button(buttonTitle)) {
						Validate();
					}
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            ScrollPosition_ = EditorGUILayout.BeginScrollView(ScrollPosition_);
				int index = 0;
				foreach (IValidationError error in validationErrors_.OrderBy(error => error.GetContextObjectName())) {
					Color color = index % 2 == 0 ? kErrorEvenColor.Value : kErrorOddColor.Value;

					EditorGUILayout.BeginVertical(EditorGUIStyleUtil.StyleWithBackgroundColor(color));
						var horizontalStyle = EditorGUIStyleUtil.CachedStyle("DTValidatorWindow::HorizontalMarginStyle", GUIStyle.none, (style) => {
							style.padding.top = 5;
							style.padding.left = 5;
							style.padding.bottom = 2;
							style.margin.bottom = 3;
						});
						EditorGUILayout.BeginHorizontal(horizontalStyle);
							var boxStyle = EditorGUIStyleUtil.StyleWithTexture(error.GetContextIcon(), (GUIStyle style) => {
								style.margin.top = 8;
							});
							GUILayout.Box("", boxStyle, GUILayout.Height(15.0f), GUILayout.Width(15.0f));
							EditorGUILayout.BeginVertical();
								EditorGUILayout.LabelField(error.GetContextObjectName(), EditorGUIStyleUtil.CachedLabelTitleStyle(), EditorGUIStyleUtil.TitleHeight);

								var componentError = error as ComponentValidationError;
								if (componentError != null) {
									EditorGUILayout.LabelField(componentError.ComponentPath);
								}

								EditorGUILayout.BeginHorizontal();
									var missingMonoScriptError = error as MissingMonoScriptValidationError;
									if (missingMonoScriptError != null && error.MemberInfo == null) {
										EditorGUILayout.LabelField(string.Format("    >Missing script on '{0}'!", missingMonoScriptError.GameObjectPath));
									} else {
										EditorGUILayout.LabelField(string.Format("    >Missing '{1}' on script '{0}'", error.MemberInfo.DeclaringType.Name, error.MemberInfo.Name));
									}
									if (GUILayout.Button("Select In Editor", EditorGUIStyleUtil.CachedAlignedButtonStyle(), GUILayout.ExpandWidth(false))) {
										error.SelectInEditor();
									}
								EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();

					index++;
				}
            EditorGUILayout.EndScrollView();
        }

		private void Validate() {
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			validationErrors_.Clear();

			if (ValidateScriptableObjects_) {
				IList<IValidationError> errors = ValidationUtil.ValidateAllSavedScriptableObjects(earlyExitOnError: false);
				if (errors != null) {
					validationErrors_.AddRange(errors);
				}
			}

            if (ValidateOpenScenes_)
            {
                IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInOpenScenes(earlyExitOnError: false);
                if (errors != null)
                {
                    validationErrors_.AddRange(errors);
                }
            }

            if (ValidateBuildScenes_)
            {
                IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInBuildSettingScenes(earlyExitOnError: false);
                if (errors != null)
                {
                    validationErrors_.AddRange(errors);
                }
            }

            if (ValidateSavedScenes_) {
				IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInSavedScenes(earlyExitOnError: false);
				if (errors != null) {
					validationErrors_.AddRange(errors);
				}
			}

			if (ValidatePrefabsInResources_) {
				IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInResources(earlyExitOnError: false);
				if (errors != null) {
					validationErrors_.AddRange(errors);
				}
			}

			stopwatch.Stop();
			validationTimeInMS_ = stopwatch.ElapsedMilliseconds;
		}
	}
}
#endif
