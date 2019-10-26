#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DTValidator.Internal {
	public class ClearCachedAssetsOnPostProcess : AssetPostprocessor {
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
			AssetDatabaseUtil.ClearCachedAssets();
		}
	}

	public static class AssetDatabaseUtil {
		public static void ClearCachedAssets() {
			cachedAssets_.Clear();
		}

		public static T LoadAssetAtPath<T>(string assetPath) where T : class {
			return AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
		}

		public static string FindSpecificAsset(string findAssetsInput, bool required = true) {
			string[] guids = AssetDatabase.FindAssets(findAssetsInput);

			if (guids.Length <= 0) {
				if (required) {
					Debug.LogError(string.Format("FindSpecificAsset: Can't find anything matching ({0}) anywhere in the project", findAssetsInput));
				}

				return "";
			}

			if (guids.Length > 2) {
				if (required) {
					Debug.LogError(string.Format("FindSpecificAsset: More than one file found for ({0}) in the project!", findAssetsInput));
				}

				return "";
			}

			return guids[0];
		}

		public static List<T> AllAssetsOfType<T>() where T : UnityEngine.Object {
			var type = typeof(T);
			if (!cachedAssets_.ContainsKey(type)) {
				List<T> assets = new List<T>();

				var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
				foreach (string guid in guids) {
					var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
					if (asset == null) {
						continue;
					}

					assets.Add(asset);
				}

				cachedAssets_[type] = assets;
			}

			return (List<T>)cachedAssets_[type];
		}


		private static Dictionary<Type, object> cachedAssets_ = new Dictionary<Type, object>();
	}
}
#endif