using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;

namespace DTValidator.TestWhitelisted {
    public class WhitelistedOutletComponent : MonoBehaviour {
        public GameObject Outlet;
    }
}

namespace DTValidator.Internal {
    using DTValidator.TestWhitelisted;

    public static class ValidatorWhitelistedNamespaceTests {
        private static IList<ValidatorWhitelistedNamespace> WhitelistedOutletComponentNamespaceProvider() {
            var whitelistedNamespace = ScriptableObject.CreateInstance<ValidatorWhitelistedNamespace>();
            whitelistedNamespace.Namespace = "DTValidator.TestWhitelisted";

            return new ValidatorWhitelistedNamespace[] { whitelistedNamespace };
        }

        private static IList<ValidatorIgnoredNamespace> IgnoredOutletComponentNamespaceProvider() {
            var ignoredNamespace = ScriptableObject.CreateInstance<ValidatorIgnoredNamespace>();
            ignoredNamespace.Namespace = "DTValidator.TestWhitelisted";

            return new ValidatorIgnoredNamespace[] { ignoredNamespace };
        }

        private static IList<ValidatorBlacklistedClass> BlacklistedOutletComponentClassProvider() {
            var blacklistedClass = ScriptableObject.CreateInstance<ValidatorBlacklistedClass>();
            blacklistedClass.Class = "BlacklistedOutletComponent";

            return new ValidatorBlacklistedClass[] { blacklistedClass };
        }

        [Test]
        public static void WhitelistedMissingOutlet_ReturnsNoErrors() {
            ValidatorWhitelistedNamespaceProvider.SetCurrentProvider(WhitelistedOutletComponentNamespaceProvider);
            ValidatorIgnoredNamespaceProvider.SetCurrentProvider(() => new ValidatorIgnoredNamespace[0]);
            ValidatorBlacklistedClassProvider.SetCurrentProvider(() => new ValidatorBlacklistedClass[0]);

            GameObject gameObject = new GameObject();

            var outletComponent = gameObject.AddComponent<WhitelistedOutletComponent>();
            outletComponent.Outlet = null;

            IList<IValidationError> errors = Validator.Validate(gameObject);
            Assert.That(errors, Is.Not.Null);

            ValidatorWhitelistedNamespaceProvider.ClearCurrentProvider();
            ValidatorIgnoredNamespaceProvider.ClearCurrentProvider();
            ValidatorBlacklistedClassProvider.ClearCurrentProvider();
        }

        [Test]
        public static void WhitelistedMissingOutlet_AndIgnored_DefaultsToWhitelisted() {
            ValidatorWhitelistedNamespaceProvider.SetCurrentProvider(WhitelistedOutletComponentNamespaceProvider);
            ValidatorIgnoredNamespaceProvider.SetCurrentProvider(IgnoredOutletComponentNamespaceProvider);
            ValidatorBlacklistedClassProvider.SetCurrentProvider(() => new ValidatorBlacklistedClass[0]);

            Debug.unityLogger.logEnabled = false;

            GameObject gameObject = new GameObject();

            var outletComponent = gameObject.AddComponent<WhitelistedOutletComponent>();
            outletComponent.Outlet = null;

            IList<IValidationError> errors = Validator.Validate(gameObject);
            Assert.That(errors, Is.Not.Null);

            Debug.unityLogger.logEnabled = true;
            ValidatorWhitelistedNamespaceProvider.ClearCurrentProvider();
            ValidatorIgnoredNamespaceProvider.ClearCurrentProvider();
            ValidatorBlacklistedClassProvider.ClearCurrentProvider();
        }

        [Test]
        public static void WhitelistedMissingOutlet_AndBlacklisted_DefaultsToWhitelisted() {
            ValidatorWhitelistedNamespaceProvider.SetCurrentProvider(WhitelistedOutletComponentNamespaceProvider);
            ValidatorIgnoredNamespaceProvider.SetCurrentProvider(() => new ValidatorIgnoredNamespace[0]);
            ValidatorBlacklistedClassProvider.SetCurrentProvider(BlacklistedOutletComponentClassProvider);

            Debug.unityLogger.logEnabled = false;

            GameObject gameObject = new GameObject();

            var outletComponent = gameObject.AddComponent<WhitelistedOutletComponent>();
            outletComponent.Outlet = null;

            IList<IValidationError> errors = Validator.Validate(gameObject);
            Assert.That(errors, Is.Not.Null);

            Debug.unityLogger.logEnabled = true;
            ValidatorWhitelistedNamespaceProvider.ClearCurrentProvider();
            ValidatorIgnoredNamespaceProvider.ClearCurrentProvider();
            ValidatorBlacklistedClassProvider.ClearCurrentProvider();
        }
    }
}
