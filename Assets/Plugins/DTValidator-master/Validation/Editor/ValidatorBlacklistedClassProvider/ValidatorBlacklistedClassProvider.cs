#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace DTValidator.Internal {
    // NOTE (darren): provider is used for Unit Tests
    public static class ValidatorBlacklistedClassProvider {
        public static IList<ValidatorBlacklistedClass> GetBlacklistedClasses() {
            if (currentProvider_ == null) {
                return AssetDatabaseUtil.AllAssetsOfType<ValidatorBlacklistedClass>();
            }

            return currentProvider_.Invoke();
        }

        public static void SetCurrentProvider(Func<IList<ValidatorBlacklistedClass>> provider) {
            currentProvider_ = provider;
        }

        public static void ClearCurrentProvider() {
            currentProvider_ = null;
        }


        // PRAGMA MARK - Internal
        private static Func<IList<ValidatorBlacklistedClass>> currentProvider_;
    }
}
#endif
