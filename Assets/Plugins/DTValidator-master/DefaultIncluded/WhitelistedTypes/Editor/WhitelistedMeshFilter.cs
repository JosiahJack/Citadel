#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DTValidator.Internal {
	[InitializeOnLoad]
	public static class WhitelistedMeshFilter {
		// PRAGMA MARK - Public Interface
		static WhitelistedMeshFilter() {
			ValidatorUnityWhitelist.RegisterWhitelistedTypeMember(typeof(UnityEngine.MeshFilter), kMeshFilterSharedMesh);
			ValidatorPredicates.RegisterPredicateFor(kMeshFilterSharedMesh, DontValidateIfTextMeshPro);
		}


		// PRAGMA MARK - Internal
		// NOTE (darren): MeshFilter doesn't have mesh as a field, it has it as a property
		private static readonly MemberInfo kMeshFilterSharedMesh = ValidatorUnityWhitelist.GetPropertyFrom(typeof(UnityEngine.MeshFilter), "sharedMesh");

		private static readonly Type kTextMeshProFirstPassType = Type.GetType("TMPro.TextMeshPro, Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
		private static readonly Type kTextMeshProType = Type.GetType("TMPro.TextMeshPro, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

		// NOTE (darren): we want to validate and check MeshFilters, but
		// TextMeshPro dynamically adds a mesh. As a requirement to validation,
		// MeshFilter must not have a TextMeshPro component as well
		private static bool DontValidateIfTextMeshPro(object obj) {
			UnityEngine.Component component = obj as UnityEngine.Component;
			if (component == null) {
				return true;
			}

			Type textMeshProType = kTextMeshProType ?? kTextMeshProFirstPassType;
			if (textMeshProType == null) {
				return true;
			}

			UnityEngine.Component textMeshProComponent = component.GetComponent(textMeshProType);
			// valid (true) when textMeshProComponent does not exist
			return textMeshProComponent == null;
		}
	}
}
#endif
