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
	public static class AttributeValidationTests {
		private class HiddenOutletComponent : MonoBehaviour {
			[HideInInspector]
			public GameObject DynamicSerializedOutlet;
		}

		[Test]
		public static void MissingHiddenOutlets_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<HiddenOutletComponent>();
			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		private class OptionalOutletComponent : MonoBehaviour {
			[Optional]
			public GameObject Outlet;
		}

		[Test]
		public static void OptionalOutlets_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<OptionalOutletComponent>();
			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}
	}
}
