using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTValidator.Internal {
	public static class IEnumerableGenericExtensions {
		public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) {
			foreach (T elem in enumerable) {
				if (predicate.Invoke(elem)) {
					return elem;
				}
			}

			return default(T);
		}
	}
}
