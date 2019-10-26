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
	public static class ValidatorPredicates {
		// PRAGMA MARK - Static Public Interface
		public static IEnumerable<Predicate<object>> GetOptionalPredicatesFor(MemberInfo memberInfo) {
			return kMemberInfoPredicates.GetValueOrDefault(memberInfo);
		}

		public static void RegisterPredicateFor(MemberInfo memberInfo, Predicate<object> predicate) {
			var predicates = kMemberInfoPredicates.GetAndCreateIfNotFound(memberInfo);
			predicates.Add(predicate);
		}

		public static void UnregisterPredicateFor(MemberInfo memberInfo, Predicate<object> predicate) {
			var predicates = kMemberInfoPredicates.GetValueOrDefault(memberInfo);
			if (predicates == null) {
				Debug.LogWarning("UnregisterPredicateFor - failed because no predicates registered for: " + memberInfo);
				return;
			}

			predicates.Remove(predicate);
		}


		// PRAGMA MARK - Internal
		private static readonly Dictionary<MemberInfo, HashSet<Predicate<object>>> kMemberInfoPredicates = new Dictionary<MemberInfo, HashSet<Predicate<object>>>();
	}
}
#endif
