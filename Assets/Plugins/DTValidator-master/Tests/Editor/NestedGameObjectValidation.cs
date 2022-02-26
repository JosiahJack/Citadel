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
	public static class NestedGameObjectValidationTests {
		private class OutletComponent : MonoBehaviour {
			public GameObject Outlet;
		}

		[Test]
		public static void MissingNestedOutlet_ReturnsErrors() {
			GameObject gameObjectA = new GameObject("A");
			GameObject gameObjectB = new GameObject("B");

			var outletComponent = gameObjectB.AddComponent<OutletComponent>();
			outletComponent.Outlet = null;

			gameObjectB.transform.SetParent(gameObjectA.transform);

			IList<IValidationError> errors = Validator.Validate(gameObjectA);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		[Test]
		public static void MissingNestedOutletValidationError_ReturnsExpected() {
			GameObject gameObjectA = new GameObject("A");
			GameObject gameObjectB = new GameObject("B");

			var outletComponent = gameObjectB.AddComponent<OutletComponent>();
			outletComponent.Outlet = null;

			gameObjectB.transform.SetParent(gameObjectA.transform);

			IList<IValidationError> errors = Validator.Validate(gameObjectA);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));

			IValidationError error = errors[0];
			Assert.That(error.ObjectLocalId, Is.EqualTo(outletComponent.GetLocalId()));
			Assert.That(error.ObjectType, Is.EqualTo(typeof(OutletComponent)));
			Assert.That(error.MemberInfo, Is.EqualTo(typeof(OutletComponent).GetField("Outlet")));
			Assert.That(error.ContextObject, Is.EqualTo(gameObjectA));
		}
	}
}
