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

using DTValidator.Internal;

namespace DTValidator {
	public static class ValidatorUnityWhitelist {
		// PRAGMA MARK - Static Public Interface
		public static bool IsTypeWhitelisted(Type type) {
			return kTypes.ContainsKey(type);
		}

		public static HashSet<MemberInfo> GetWhitelistedMembersFor(Type type) {
			return kTypes[type];
		}

		public static void RegisterWhitelistedTypeMember(Type type, MemberInfo memberInfo) {
			HashSet<MemberInfo> members = kTypes.GetAndCreateIfNotFound(type);
			members.Add(memberInfo);
		}

		public static void UnregisterWhitelistedTypeMember(Type type, MemberInfo memberInfo) {
			HashSet<MemberInfo> members = kTypes.GetAndCreateIfNotFound(type);
			members.Remove(memberInfo);
		}

		public static PropertyInfo GetPropertyFrom(Type type, string propertyName) {
			return type.GetRequiredProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}

		public static FieldInfo GetFieldFrom(Type type, string fieldName) {
			return type.GetRequiredField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}


		// PRAGMA MARK - Internal
		private static readonly Dictionary<Type, HashSet<MemberInfo>> kTypes = new Dictionary<Type, HashSet<MemberInfo>>();
	}
}
#endif
