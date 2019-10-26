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
	public static class BasicGameObjectValidationTests {
		[Test]
		public static void EmptyGameObject_ReturnsNoErrors() {
			GameObject emptyGameObject = new GameObject();
			IList<IValidationError> errors = Validator.Validate(emptyGameObject);
			Assert.That(errors, Is.Null);
		}

		private class OutletComponent : MonoBehaviour {
			public GameObject Outlet;
		}

		[Test]
		public static void FilledOutlet_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<OutletComponent>();
			outletComponent.Outlet = gameObject;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void MissingOutlet_ReturnsErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<OutletComponent>();
			outletComponent.Outlet = null;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		private class PrivateOutletComponent : MonoBehaviour {
			public void SetOutlet(GameObject go) {
				outlet_ = go;
			}

			#pragma warning disable 0414 // fields never used
			[SerializeField]
			private GameObject outlet_;
			#pragma warning restore 0414 // fields never used
		}

		[Test]
		public static void FilledPrivateOutlet_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<PrivateOutletComponent>();
			outletComponent.SetOutlet(gameObject);

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void MissingPrivateOutlet_ReturnsErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<PrivateOutletComponent>();
			outletComponent.SetOutlet(null);

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		private class NonSerializedOutletComponent : MonoBehaviour {
			#pragma warning disable 0649 // fields never used or assigned to
			protected GameObject outlet1_;
			protected readonly GameObject outlet2_;
			private GameObject outlet3_;
			private readonly GameObject outlet4_;
			#pragma warning restore 0649 // fields never used or assigned to
		}

		[Test]
		public static void MissingNonSerializedOutlet_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();

			gameObject.AddComponent<NonSerializedOutletComponent>();

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		private class PrivateNestedOutletComponent : MonoBehaviour {
			public PrivateNestedOutlet NestedOutlet;
		}

		[Serializable]
		private class PrivateNestedOutlet {
			public void SetOutlet(GameObject go) {
				outlet_ = go;
			}

			#pragma warning disable 0414 // fields never used
			[SerializeField]
			private GameObject outlet_;
			#pragma warning restore 0414 // fields never used
		}

		[Test]
		public static void FilledPrivateNestedOutlet_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<PrivateNestedOutletComponent>();
			outletComponent.NestedOutlet = new PrivateNestedOutlet();
			outletComponent.NestedOutlet.SetOutlet(gameObject);

			IList<IValidationError> errors = Validator.Validate(gameObject, recursive: true);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void MissingPrivateNestedOutlet_ReturnsErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<PrivateNestedOutletComponent>();
			outletComponent.NestedOutlet = new PrivateNestedOutlet();
			outletComponent.NestedOutlet.SetOutlet(null);

			IList<IValidationError> errors = Validator.Validate(gameObject, recursive: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
