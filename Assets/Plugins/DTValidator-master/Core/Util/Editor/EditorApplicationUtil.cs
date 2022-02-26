using System;
using UnityEditor;
using UnityEngine;

namespace DTValidator.Internal {
	public class EditorApplicationUtilAssetModificationProcessor : UnityEditor.AssetModificationProcessor {
		public static string[] OnWillSaveAssets(string[] paths) {
			bool sceneSaved = false;
			foreach (string path in paths) {
				if (PathUtil.IsScene(path)) {
					sceneSaved = true;
					break;
				}
			}

			if (sceneSaved) {
				EditorApplicationUtil.SceneSaved.Invoke();
			}

			return paths;
		}
	}

	public static class EditorApplicationUtil {
		public static Action<SceneView> OnSceneGUIDelegate = delegate (SceneView sceneView) { };
		public static Action SceneSaved = delegate { };
		public static Action SceneDirtied = delegate { };

		static EditorApplicationUtil() {

            #if UNITY_2018_1_OR_NEWER
                EditorApplication.hierarchyChanged += HierarchyChanged;
            #else
                EditorApplication.hierarchyWindowChanged += HierarchyChanged;
            #endif

            SceneView.onSceneGUIDelegate += OnSceneGUI;
			Undo.postprocessModifications += PostProcessModifications;
		}

		private static int previousObjectCount_ = 0;
		private static void HierarchyChanged() {
			int newObjectCount = GameObject.FindObjectsOfType<UnityEngine.Object>().Length;
			if (newObjectCount != previousObjectCount_) {
				previousObjectCount_ = newObjectCount;
				SceneDirtied.Invoke();
			}
		}

		private static UndoPropertyModification[] PostProcessModifications(UndoPropertyModification[] propertyModifications) {
			SceneDirtied.Invoke();
			return propertyModifications;
		}

		private static void OnSceneGUI(SceneView sceneView) {
			OnSceneGUIDelegate.Invoke(sceneView);
		}
	}
}
