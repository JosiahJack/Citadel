#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTValidator.Internal {
	// NOTE (darren): provider is used for Unit Tests
	public static class ValidatorIgnoredNamespaceProvider {
		public static IList<ValidatorIgnoredNamespace> GetIgnoredNamespaces() {
			if (currentProvider_ == null) {
				return AssetDatabaseUtil.AllAssetsOfType<ValidatorIgnoredNamespace>();
			}

			return currentProvider_.Invoke();
		}

		public static void SetCurrentProvider(Func<IList<ValidatorIgnoredNamespace>> provider) {
			currentProvider_ = provider;
		}

		public static void ClearCurrentProvider() {
			currentProvider_ = null;
		}


		// PRAGMA MARK - Internal
		private static Func<IList<ValidatorIgnoredNamespace>> currentProvider_;
	}
}
#endif