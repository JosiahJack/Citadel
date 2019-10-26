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
	public static class ListValidationTests {
		private class ListOutletComponent : MonoBehaviour {
			public List<GameObject> Outlets;
		}

		[Test]
		public static void NullList_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();
			ListOutletComponent outletComponent = gameObject.AddComponent<ListOutletComponent>();
			outletComponent.Outlets = null;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void EmptyList_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();
			ListOutletComponent outletComponent = gameObject.AddComponent<ListOutletComponent>();
			outletComponent.Outlets = new List<GameObject>();

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void FilledList_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();
			ListOutletComponent outletComponent = gameObject.AddComponent<ListOutletComponent>();
			outletComponent.Outlets = new List<GameObject>();
			outletComponent.Outlets.Add(gameObject);

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void MissingOutletInList_ReturnsErrors() {
			GameObject gameObject = new GameObject();
			ListOutletComponent outletComponent = gameObject.AddComponent<ListOutletComponent>();
			outletComponent.Outlets = new List<GameObject>();
			outletComponent.Outlets.Add(null);

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		[Test]
		public static void MixedOutletInList_ReturnsErrors() {
			GameObject gameObject = new GameObject();
			ListOutletComponent outletComponent = gameObject.AddComponent<ListOutletComponent>();
			outletComponent.Outlets = new List<GameObject>();
			outletComponent.Outlets.Add(gameObject);
			outletComponent.Outlets.Add(null);
			outletComponent.Outlets.Add(null);

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(2));
		}

		[Test]
		public static void MissingOutletValidationError_ReturnsExpected() {
			GameObject gameObject = new GameObject();
			ListOutletComponent outletComponent = gameObject.AddComponent<ListOutletComponent>();
			outletComponent.Outlets = new List<GameObject>();
			outletComponent.Outlets.Add(null);

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));

			IValidationError error = errors[0];
			Assert.That(error.ObjectLocalId, Is.EqualTo(outletComponent.GetLocalId()));
			Assert.That(error.ObjectType, Is.EqualTo(typeof(ListOutletComponent)));
			Assert.That(error.MemberInfo, Is.EqualTo(typeof(ListOutletComponent).GetField("Outlets")));
			Assert.That(error.ContextObject, Is.EqualTo(gameObject));
		}
	}
}
