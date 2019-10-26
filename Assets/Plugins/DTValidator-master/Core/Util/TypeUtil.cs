using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DTValidator.Internal {
	public static class TypeUtil {
		// PRAGMA MARK - Static Public Interface
		public static FieldInfo[] GetInspectorFields(Type type) {
			if (!inspectorFieldMapping_.ContainsKey(type)) {
				List<FieldInfo> fieldInfos = new List<FieldInfo>();
				Type iterType = type;
				while (iterType != null) {
					fieldInfos.AddRange(iterType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(f => f.IsPublic || Attribute.IsDefined(f, typeof(SerializeField))));
					iterType = iterType.BaseType;
				}

				inspectorFieldMapping_[type] = fieldInfos.ToArray();
			}

			return inspectorFieldMapping_[type];
		}

		public static Type[] GetImplementationTypes(Type inputType) {
			if (!implementationTypeMapping_.ContainsKey(inputType)) {
				implementationTypeMapping_[inputType] = AllAssemblyTypes
				.Where(t => inputType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType)
				.ToArray();
			}

			return implementationTypeMapping_[inputType];
		}

		public static string[] GetImplementationTypeNames(Type type) {
			if (!implementationTypeNameMapping_.ContainsKey(type)) {
				implementationTypeNameMapping_[type] = GetImplementationTypes(type).Select(t => t.Name).ToArray();
			}

			return implementationTypeNameMapping_[type];
		}

		public static Type[] AllAssemblyTypes {
			get {
				return allAssemblyTypes_ ?? (allAssemblyTypes_ = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
																  from type in assembly.GetTypes()
																  select type).ToArray());
			}
		}


		// PRAGMA MARK - Static Internal
		private static Dictionary<Type, FieldInfo[]> inspectorFieldMapping_ = new Dictionary<Type, FieldInfo[]>();
		private static Dictionary<Type, Type[]> implementationTypeMapping_ = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, string[]> implementationTypeNameMapping_ = new Dictionary<Type, string[]>();

		private static Type[] allAssemblyTypes_ = null;
	}
}
