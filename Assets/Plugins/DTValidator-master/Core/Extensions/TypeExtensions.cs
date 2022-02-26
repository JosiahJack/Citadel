using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DTValidator.Internal {
	public static class TypeExtensions {
		public static FieldInfo GetRequiredField(this Type type, string name, BindingFlags bindingFlags) {
			FieldInfo fieldInfo = type.GetField(name, bindingFlags);
			if (fieldInfo == null) {
				Debug.LogError(string.Format("Failed to find field named: '{0}' on type: {1}\nFlags: {2}", name, type.Name, bindingFlags));
			}
			return fieldInfo;
		}

		public static PropertyInfo GetRequiredProperty(this Type type, string name, BindingFlags bindingFlags) {
			PropertyInfo propertyInfo = type.GetProperty(name, bindingFlags);
			if (propertyInfo == null) {
				Debug.LogError(string.Format("Failed to find property named: '{0}' on type: {1}\nFlags: {2}", name, type.Name, bindingFlags));
			}
			return propertyInfo;
		}
	}
}
