#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DTValidator.Internal {
	public static class ScriptableObjectEditorUtil {
		public static Dictionary<Type, string> _cachedPaths = new Dictionary<Type, string>();

		public static string DirectoryPathForScriptableObjectType<T>() where T : ScriptableObject {
			Type type = typeof(T);
			if (_cachedPaths.ContainsKey(type)) {
				return _cachedPaths[type];
			}

			T instance = ScriptableObject.CreateInstance<T>();
			MonoScript script = MonoScript.FromScriptableObject(instance);
			string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));
			ScriptableObject.DestroyImmediate(instance);

			_cachedPaths[type] = path;
			return path;
		}
	}
}
#endif