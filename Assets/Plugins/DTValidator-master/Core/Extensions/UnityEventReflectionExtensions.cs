#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace DTValidator.Internal {
	public static class UnityEventReflectionExtensions {
		// PRAGMA MARK - Static Public Interface
		public static MethodInfo GetMethodInfoForIndex(this UnityEventBase unityEvent, int index) {
			object persistentCall = unityEvent.GetPersistentCallForIndex(index);
			return (MethodInfo)kFindMethodMethod.Invoke(unityEvent, new object[] { persistentCall });
		}

		public static void AddVoidPersistentListener(this UnityEventBase unityEvent, UnityEngine.Object targetObj, string methodName) {
			var persistentCalls = kPersistentCallsField.GetValue(unityEvent);
			kAddListenerMethod.Invoke(persistentCalls, null);
			kRegisterVoidListenerMethod.Invoke(persistentCalls, new object[] { 0, targetObj, methodName });
			kDirtyPersistentCallsMethod.Invoke(unityEvent, null);
		}


		// PRAGMA MARK - Internal
		private static readonly Type kPersistentCallType = Assembly.GetAssembly(typeof(UnityEventBase)).GetTypes().First(t => t.Name == "PersistentCall");

		private static readonly FieldInfo kPersistentCallsField = typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly MethodInfo kFindMethodMethod = typeof(UnityEventBase).GetMethod("FindMethod", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { kPersistentCallType }, null);
		private static readonly MethodInfo kDirtyPersistentCallsMethod = typeof(UnityEventBase).GetMethod("DirtyPersistentCalls", BindingFlags.NonPublic | BindingFlags.Instance);

		private static readonly Type kPersistentCallGroupType = Assembly.GetAssembly(typeof(UnityEventBase)).GetTypes().First(t => t.Name == "PersistentCallGroup");
		private static readonly MethodInfo kRegisterVoidListenerMethod = kPersistentCallGroupType.GetMethod("RegisterVoidPersistentListener");
		private static readonly MethodInfo kAddListenerMethod = kPersistentCallGroupType.GetMethod("AddListener", types: new Type[0]);
		private static readonly MethodInfo kGetListenerMethod = kPersistentCallGroupType.GetMethod("GetListener");

		private static object GetPersistentCallForIndex(this UnityEventBase unityEvent, int index) {
			var persistentCalls = kPersistentCallsField.GetValue(unityEvent);
			return kGetListenerMethod.Invoke(persistentCalls, new object[] { index });
		}
	}
}
#endif
