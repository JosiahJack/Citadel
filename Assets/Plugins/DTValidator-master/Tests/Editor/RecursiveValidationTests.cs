using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using NUnit.Framework;

namespace DTValidator.Internal {
	public static class RecursiveValidationTests {
		private class OutletComponent : MonoBehaviour {
			public GameObject Outlet;
		}

		private class OutletScriptableObjectComponent : MonoBehaviour {
			public OutletScriptableObject Outlet;
		}

		[Test]
		public static void RecursivePrefabWithMissingOutlet_ReturnsErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<OutletComponent>();

			GameObject missingOutletPrefab = Resources.Load<GameObject>("DTValidatorTests/TestMissingOutletPrefab");
			outletComponent.Outlet = missingOutletPrefab;

			IList<IValidationError> errors = Validator.Validate(gameObject, recursive: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		[Test]
		public static void RecursivePrefabValidationError_ReturnsExpected() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<OutletComponent>();

			GameObject missingOutletPrefab = Resources.Load<GameObject>("DTValidatorTests/TestMissingOutletPrefab");
			outletComponent.Outlet = missingOutletPrefab;

			IList<IValidationError> errors = Validator.Validate(gameObject, recursive: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));

			IValidationError error = errors[0];
			Assert.That(error.ObjectLocalId, Is.EqualTo(missingOutletPrefab.GetComponentInChildren<TestOutletComponent>().GetLocalId()));
			Assert.That(error.ObjectType, Is.EqualTo(typeof(TestOutletComponent)));
			Assert.That(error.MemberInfo, Is.EqualTo(typeof(TestOutletComponent).GetField("Outlet")));
			Assert.That(error.ContextObject, Is.EqualTo(missingOutletPrefab));
		}

		[Test]
		public static void InfiniteLoopPrefab_DoesNotGoIndefinitely() {
			GameObject infiniteLoopPrefab = Resources.Load<GameObject>("DTValidatorTests/TestInfiniteLoopOutletPrefab");

			IList<IValidationError> errors = Validator.Validate(infiniteLoopPrefab, recursive: true);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void MissingScriptableObjectInnerOutlet_ReturnsErrors() {
			GameObject gameObject = new GameObject();
			OutletScriptableObject outletScriptableObject = ScriptableObject.CreateInstance<OutletScriptableObject>();
			outletScriptableObject.Outlet = null;

			var outletComponent = gameObject.AddComponent<OutletScriptableObjectComponent>();
			outletComponent.Outlet = outletScriptableObject;

			IList<IValidationError> errors = Validator.Validate(gameObject, recursive: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		[Test]
		public static void MissingScriptableObjectValidationError_ReturnsExpected() {
			GameObject gameObject = new GameObject();
			OutletScriptableObject outletScriptableObject = ScriptableObject.CreateInstance<OutletScriptableObject>();
			outletScriptableObject.Outlet = null;

			var outletComponent = gameObject.AddComponent<OutletScriptableObjectComponent>();
			outletComponent.Outlet = outletScriptableObject;

			IList<IValidationError> errors = Validator.Validate(gameObject, recursive: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));

			IValidationError error = errors[0];
			Assert.That(error.ObjectLocalId, Is.EqualTo(outletScriptableObject.GetLocalId()));
			Assert.That(error.ObjectType, Is.EqualTo(typeof(OutletScriptableObject)));
			Assert.That(error.MemberInfo, Is.EqualTo(typeof(OutletScriptableObject).GetField("Outlet")));
			Assert.That(error.ContextObject, Is.EqualTo(outletScriptableObject));
		}
	}
}
