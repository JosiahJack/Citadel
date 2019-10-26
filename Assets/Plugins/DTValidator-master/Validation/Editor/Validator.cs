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
	public static class Validator {
		// PRAGMA MARK - Static Public Interface
		public static IList<IValidationError> Validate(GameObject gameObject, object contextObject = null, bool recursive = false, List<IValidationError> validationErrors = null) {
			if (gameObject == null) {
				return null;
			}

			if (contextObject == null) {
				contextObject = gameObject;
			}

			ValidateGameObjectInternal(gameObject, contextObject, recursive, ref validationErrors);
			return validationErrors;
		}

		public static IList<IValidationError> Validate(ScriptableObject scriptableObject, bool recursive = false, List<IValidationError> validationErrors = null) {
			if (scriptableObject == null) {
				return null;
			}

			ValidateInternal(scriptableObject, scriptableObject, recursive, ref validationErrors);
			return validationErrors;
		}


		// PRAGMA MARK - Static Internal
		private static HashSet<Assembly> kUnityAssemblies = new HashSet<Assembly>() {
			Assembly.GetAssembly(typeof(UnityEngine.MonoBehaviour)),
			Assembly.GetAssembly(typeof(UnityEngine.UI.Text)),
			Assembly.GetAssembly(typeof(UnityEditor.Editor))
		};

		private static void ValidateGameObjectInternal(GameObject gameObject, object contextObject, bool recursive, ref List<IValidationError> validationErrors, HashSet<object> validatedObjects = null) {
			Queue<GameObject> queue = new Queue<GameObject>();
			queue.Enqueue(gameObject);

			if (recursive) {
				validatedObjects = validatedObjects ?? new HashSet<object>();
				validatedObjects.Add(gameObject);
			}

			while (queue.Count > 0) {
				GameObject current = queue.Dequeue();

				Component[] components = current.GetComponents<Component>();
				if (components == null) {
					continue;
				}

				foreach (Component c in components) {
					if (c == null) {
						validationErrors = validationErrors ?? new List<IValidationError>();
						validationErrors.Add(ValidationErrorFactory.Create(gameObject, contextObject));
						continue;
					}

					ValidateInternal(c, contextObject, recursive, ref validationErrors, validatedObjects);
				}

				foreach (GameObject child in current.GetChildren()) {
					queue.Enqueue(child);
				}
			}
		}

		private static void ValidateInternal(object obj, object contextObject, bool recursive, ref List<IValidationError> validationErrors, HashSet<object> validatedObjects = null) {
			if (obj == null) {
				return;
			}

			if (validatedObjects != null) {
				if (validatedObjects.Contains(obj)) {
					return;
				}

				validatedObjects.Add(obj);
			}

			Type objectType = obj.GetType();

			var whitelistedNamespaces = ValidatorWhitelistedNamespaceProvider.GetWhitelistedNamespaces();
			// if whitelisted asset exists - use whitelisting instead of blacklisting
			if (whitelistedNamespaces.Count > 0) {
				int ignoredNamespacesCount = ValidatorIgnoredNamespaceProvider.GetIgnoredNamespaces().Count;
				if (ignoredNamespacesCount > 0) {
					Debug.LogWarning("Both ValidatorIgnoredNamespace + ValidatorWhitelistedNamespace exist in project (mutually exclusive) - will only use whitelisted!");
				}

				if (string.IsNullOrEmpty(objectType.Namespace)) {
					// No namespace means no validation in whitelist format
					return;
				}

				foreach (var whitelistedNamespace in whitelistedNamespaces) {
					if (!objectType.Namespace.Contains(whitelistedNamespace.Namespace)) {
						return;
					}
				}
			} else {
				if (!string.IsNullOrEmpty(objectType.Namespace)) {
					// allow user defined ignores for namespaces
					foreach (var validatorIgnoredNamespace in ValidatorIgnoredNamespaceProvider.GetIgnoredNamespaces()) {
						if (validatorIgnoredNamespace == null) {
							Debug.LogWarning("Bad state - validatorIgnoredNamespace is null!");
							continue;
						}

						if (objectType.Namespace.Contains(validatorIgnoredNamespace.Namespace)) {
							return;
						}
					}
				}
			}

			foreach (FieldInfo fieldInfo in TypeUtil.GetInspectorFields(objectType)
			.Where(f => typeof(UnityEventBase).IsAssignableFrom(f.FieldType))
			.Where(f => !Attribute.IsDefined(f, typeof(OptionalAttribute)) && !Attribute.IsDefined(f, typeof(HideInInspector)))) {
				// NOTE (darren): check UnityEvents for all classes
				UnityEventBase unityEvent = (UnityEventBase)fieldInfo.GetValue(obj);
				if (unityEvent == null) {
					Debug.LogError("Unexpected null UnityEvent in GameObjectValidator!");
					continue;
				}

				for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++) {
					MethodInfo methodInfo = unityEvent.GetMethodInfoForIndex(i);
					if (methodInfo == null) {
						validationErrors = validationErrors ?? new List<IValidationError>();
						validationErrors.Add(ValidationErrorFactory.Create(obj, objectType, fieldInfo, contextObject));
						break;
					}
				}
			}

			bool whitelisted = ValidatorUnityWhitelist.IsTypeWhitelisted(objectType);

			if (kUnityAssemblies.Contains(objectType.Assembly) && !whitelisted) {
				return;
			}

			IEnumerable<MemberInfo> membersToCheck = null;
			if (whitelisted) {
				membersToCheck = ValidatorUnityWhitelist.GetWhitelistedMembersFor(objectType);
			} else {
				membersToCheck = TypeUtil.GetInspectorFields(objectType)
								.Where(f => !Attribute.IsDefined(f, typeof(OptionalAttribute)) && !Attribute.IsDefined(f, typeof(HideInInspector)))
								.Where(f => !kUnityAssemblies.Contains(f.DeclaringType.Assembly)).Cast<MemberInfo>(); // NOTE (darren): this is to ignore fields that declared in super-classes out of our control (Unity)
			}

			foreach (MemberInfo memberInfo in membersToCheck) {
				IEnumerable<Predicate<object>> predicates = ValidatorPredicates.GetOptionalPredicatesFor(memberInfo);
				if (predicates != null) {
					bool shouldValidate = predicates.All(p => p.Invoke(obj));
					if (!shouldValidate) {
						continue;
					}
				}

				IList<UnityEngine.Object> unityEngineObjects = GetUnityEngineObjects(memberInfo, obj);
				// TODO (darren): don't alloc memory for List<> if not necessary
				if (unityEngineObjects == null || unityEngineObjects.Count <= 0) {
					// NOTE (darren): if this is not a UnityEngine.Object
					// we might still have to recursively look through its fields
					// which might contain UnityEngine.Objects
					if (recursive) {
						IList<object> memberObjects = GetMemberObjects(memberInfo, obj);
						foreach (object memberObj in memberObjects) {
							// NOTE (darren): the LocalId is broken here because we lost
							// a reference to the original GameObject being validated
							// (as contextObject may be the scene for example).
							// Leaving this as-is as it's an edge case and nice-to-have.
							ValidateInternal(memberObj, contextObject, recursive, ref validationErrors, validatedObjects);
						}
					}
					continue;
				}

				int index = 0;
				foreach (UnityEngine.Object memberObject in unityEngineObjects) {
					if (memberObject == null) {
						validationErrors = validationErrors ?? new List<IValidationError>();
						if (unityEngineObjects.Count > 1) {
							validationErrors.Add(ValidationErrorFactory.Create(obj, objectType, memberInfo, contextObject, index));
						} else {
							validationErrors.Add(ValidationErrorFactory.Create(obj, objectType, memberInfo, contextObject));
						}
						index++;
						continue;
					}

					if (recursive) {
						GameObject memberObjectAsGameObject = memberObject as GameObject;
						if (memberObjectAsGameObject != null) {
							PrefabType prefabType = PrefabUtility.GetPrefabType(memberObjectAsGameObject);
							if (prefabType == PrefabType.Prefab) {
								// switch context to the prefab we just recursed to
								object newContextObject = memberObjectAsGameObject;

								validatedObjects = validatedObjects ?? new HashSet<object>() { obj };
								ValidateGameObjectInternal(memberObjectAsGameObject, newContextObject, recursive, ref validationErrors, validatedObjects);
							}
						}

						ScriptableObject memberObjectAsScriptableObject = memberObject as ScriptableObject;
						if (memberObjectAsScriptableObject != null) {
							// switch context to the scriptable object we just recursed to
							object newContextObject = memberObjectAsScriptableObject;

							validatedObjects = validatedObjects ?? new HashSet<object>() { obj };
							ValidateInternal(memberObjectAsScriptableObject, newContextObject, recursive, ref validationErrors, validatedObjects);
						}
					}
					index++;
				}
			}
		}

		private static List<object> GetMemberObjects(MemberInfo memberInfo, object obj) {
			Type memberType;
			Func<object, object> getter = GetGetterFor(memberInfo, out memberType);

			if (getter == null) {
				Debug.LogWarning("Failed to get getter from memberInfo: " + memberInfo + "!");
				return null;
			}

			List<object> objects = new List<object>();
			if (memberType.IsClass) {
				objects.Add(getter.Invoke(obj));
			} else if (typeof(IEnumerable).IsAssignableFrom(memberType)) {
				var enumerable = (IEnumerable)getter.Invoke(obj);
				if (enumerable == null) {
					// NOTE (darren): it's possible for a serialized enumerable like int[] to be
					// null instead of empty enumerable - there is nothing to iterate over
					return null;
				}

				foreach (var o in enumerable) {
					objects.Add(o);
				}
			}

			return objects;
		}

		private static List<UnityEngine.Object> GetUnityEngineObjects(MemberInfo memberInfo, object obj) {
			Type memberType;
			Func<object, object> getter = GetGetterFor(memberInfo, out memberType);

			if (getter == null) {
				Debug.LogWarning("Failed to get getter from memberInfo: " + memberInfo + "!");
				return null;
			}

			List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
			if (memberType.IsClass && typeof(UnityEngine.Object).IsAssignableFrom(memberType)) {
				objects.Add((UnityEngine.Object)getter.Invoke(obj));
			} else if (typeof(IEnumerable).IsAssignableFrom(memberType)) {
				if (memberType.IsGenericType && (typeof(List<>)).IsAssignableFrom(memberType.GetGenericTypeDefinition())) {
					if (!typeof(UnityEngine.Object).IsAssignableFrom(memberType.GetGenericArguments()[0])) {
						return null;
					}
				} else {
					if (!typeof(UnityEngine.Object).IsAssignableFrom(memberType.GetElementType())) {
						return null;
					}
				}

				var enumerable = (IEnumerable)getter.Invoke(obj);
				if (enumerable == null) {
					// NOTE (darren): it's possible for a serialized enumerable like int[] to be
					// null instead of empty enumerable - there is nothing to iterate over
					return null;
				}

				foreach (var o in enumerable) {
					objects.Add(o as UnityEngine.Object);
				}
			}

			return objects;
		}

		private static Func<object, object> GetGetterFor(MemberInfo memberInfo, out Type memberType) {
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null) {
				memberType = fieldInfo.FieldType;
				return (object o) => fieldInfo.GetValue(o);
			}

			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null) {
				memberType = propertyInfo.PropertyType;
				// NOTE (darren): can't use PropertyType.GetValue(o) because .NET version
				return (object o) => propertyInfo.GetValue(o, BindingFlags.Default, null, null, null);
			}

			memberType = null;
			return null;
		}
	}
}
#endif
