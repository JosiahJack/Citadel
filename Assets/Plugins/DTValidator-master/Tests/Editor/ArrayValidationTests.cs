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
	public static class ArrayValidationTests {
		private class ArrayOutletComponent : MonoBehaviour {
			public GameObject[] Outlets;
		}

		[Test]
		public static void NullArray_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();
			ArrayOutletComponent outletComponent = gameObject.AddComponent<ArrayOutletComponent>();
			outletComponent.Outlets = null;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void EmptyArray_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();
			ArrayOutletComponent outletComponent = gameObject.AddComponent<ArrayOutletComponent>();
			outletComponent.Outlets = new GameObject[0];

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void FilledArray_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();
			ArrayOutletComponent outletComponent = gameObject.AddComponent<ArrayOutletComponent>();
			outletComponent.Outlets = new GameObject[1];
			outletComponent.Outlets[0] = gameObject;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void MissingOutletInArray_ReturnsErrors() {
			GameObject gameObject = new GameObject();
			ArrayOutletComponent outletComponent = gameObject.AddComponent<ArrayOutletComponent>();
			outletComponent.Outlets = new GameObject[1];

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		[Test]
		public static void MixedOutletInArray_ReturnsErrors() {
			GameObject gameObject = new GameObject();
			ArrayOutletComponent outletComponent = gameObject.AddComponent<ArrayOutletComponent>();
			outletComponent.Outlets = new GameObject[3];
			outletComponent.Outlets[0] = gameObject;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(2));
		}

		[Test]
		public static void MissingOutletValidationError_ReturnsExpected() {
			GameObject gameObject = new GameObject();
			ArrayOutletComponent outletComponent = gameObject.AddComponent<ArrayOutletComponent>();
			outletComponent.Outlets = new GameObject[1];

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));

			IValidationError error = errors[0];
			Assert.That(error.ObjectLocalId, Is.EqualTo(outletComponent.GetLocalId()));
			Assert.That(error.ObjectType, Is.EqualTo(typeof(ArrayOutletComponent)));
			Assert.That(error.MemberInfo, Is.EqualTo(typeof(ArrayOutletComponent).GetField("Outlets")));
			Assert.That(error.ContextObject, Is.EqualTo(gameObject));
		}
	}
}
