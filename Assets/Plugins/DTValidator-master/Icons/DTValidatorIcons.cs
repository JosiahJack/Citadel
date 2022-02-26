#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DTValidator.Internal {
	// NOTE (darren): scriptable object used for getting the path of icons
	public class DTValidatorIcons : ScriptableObject {
		public static Texture2D PrefabIcon {
			get {
				if (prefabIcon_ == null) {
					prefabIcon_ = AssetDatabase.LoadAssetAtPath(Path.Combine(IconDirectory_, "PrefabIcon.png"), typeof(Texture2D)) as Texture2D;
				}
				return prefabIcon_ ?? Texture2DUtil.ClearTexture;
			}
		}

		public static Texture2D SceneIcon {
			get {
				if (sceneIcon_ == null) {
					sceneIcon_ = AssetDatabase.LoadAssetAtPath(Path.Combine(IconDirectory_, "SceneIcon.png"), typeof(Texture2D)) as Texture2D;
				}
				return sceneIcon_ ?? Texture2DUtil.ClearTexture;
			}
		}

		public static Texture2D ScriptableObjectIcon {
			get {
				if (scriptableObjectIcon_ == null) {
					scriptableObjectIcon_ = AssetDatabase.LoadAssetAtPath(Path.Combine(IconDirectory_, "ScriptableObjectIcon.png"), typeof(Texture2D)) as Texture2D;
				}
				return scriptableObjectIcon_ ?? Texture2DUtil.ClearTexture;
			}
		}

		public static Texture2D ErrorIcon {
			get {
				if (errorIcon_ == null) {
					errorIcon_ = AssetDatabase.LoadAssetAtPath(Path.Combine(IconDirectory_, "ErrorIcon.png"), typeof(Texture2D)) as Texture2D;
				}
				return errorIcon_ ?? Texture2DUtil.ClearTexture;
			}
		}


		// PRAGMA MARK - Internal
		private static Texture2D prefabIcon_;
		private static Texture2D sceneIcon_;
		private static Texture2D scriptableObjectIcon_;
		private static Texture2D errorIcon_;

		private static string iconDirectory_ = null;
		private static string IconDirectory_ {
			get { return iconDirectory_ ?? (iconDirectory_ = ScriptableObjectEditorUtil.DirectoryPathForScriptableObjectType<DTValidatorIcons>()); }
		}
	}
}
#endif
