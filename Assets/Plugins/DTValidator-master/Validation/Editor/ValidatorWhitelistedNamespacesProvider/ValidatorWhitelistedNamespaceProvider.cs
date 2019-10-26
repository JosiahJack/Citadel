#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTValidator.Internal {
	// NOTE (darren): provider is used for Unit Tests
	public static class ValidatorWhitelistedNamespaceProvider {
		public static IList<ValidatorWhitelistedNamespace> GetWhitelistedNamespaces() {
			if (currentProvider_ == null) {
				return AssetDatabaseUtil.AllAssetsOfType<ValidatorWhitelistedNamespace>();
			}

			return currentProvider_.Invoke();
		}

		public static void SetCurrentProvider(Func<IList<ValidatorWhitelistedNamespace>> provider) {
			currentProvider_ = provider;
		}

		public static void ClearCurrentProvider() {
			currentProvider_ = null;
		}


		// PRAGMA MARK - Internal
		private static Func<IList<ValidatorWhitelistedNamespace>> currentProvider_;
	}
}
#endif