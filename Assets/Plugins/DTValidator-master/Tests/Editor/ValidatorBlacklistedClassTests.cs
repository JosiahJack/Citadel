using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class BlacklistedOutletComponent : MonoBehaviour {
    public GameObject Outlet;
}


namespace DTValidator.Internal {
    public static class ValidatorBlacklistedClassTests {
        private static IList<ValidatorBlacklistedClass> BlacklistedOutletComponentClassProvider() {
            var blacklistedClass = ScriptableObject.CreateInstance<ValidatorBlacklistedClass>();
            blacklistedClass.Class = "BlacklistedOutletComponent";

            return new ValidatorBlacklistedClass[] { blacklistedClass };
        }

        [Test]
        public static void BlacklistedMissingOutlet_ReturnsNoErrors() {
            ValidatorBlacklistedClassProvider.SetCurrentProvider(BlacklistedOutletComponentClassProvider);
            ValidatorIgnoredNamespaceProvider.SetCurrentProvider(() => new ValidatorIgnoredNamespace[0]);
            ValidatorWhitelistedNamespaceProvider.SetCurrentProvider(() => new ValidatorWhitelistedNamespace[0]);

            GameObject gameObject = new GameObject();

            var outletComponent = gameObject.AddComponent<BlacklistedOutletComponent>();
            outletComponent.Outlet = null;

            IList<IValidationError> errors = Validator.Validate(gameObject);
            Assert.That(errors, Is.Null);

            ValidatorBlacklistedClassProvider.ClearCurrentProvider();
            ValidatorIgnoredNamespaceProvider.ClearCurrentProvider();
            ValidatorWhitelistedNamespaceProvider.ClearCurrentProvider();
        }
    }
}
