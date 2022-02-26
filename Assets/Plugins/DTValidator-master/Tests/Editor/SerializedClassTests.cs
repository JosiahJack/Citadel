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
	public static class SerializedClassTests {
        [System.Serializable]
        private class Outlet {
            public GameObject outlet;
        }

		private class OutletComponent : MonoBehaviour {
			public Outlet Outlet;
		}

		private class ListOutletComponent : MonoBehaviour {
			public List<Outlet> Outlets;
		}

		[Test]
		public static void MissingInnerOutlet_ReturnsErrors() {
			GameObject gameObject = new GameObject();
			OutletComponent outletComponent = gameObject.AddComponent<OutletComponent>();
			outletComponent.Outlet = new Outlet();

			IList<IValidationError> errors = Validator.Validate(gameObject, recursive: true);
			Assert.That(errors, Is.Not.Null);
		}

		[Test]
		public static void MissingListOfInnerOutlet_ReturnsErrors() {
			GameObject gameObject = new GameObject();
			ListOutletComponent outletComponent = gameObject.AddComponent<ListOutletComponent>();
			outletComponent.Outlets = new List<Outlet>();
			outletComponent.Outlets.Add(new Outlet());

			IList<IValidationError> errors = Validator.Validate(gameObject, recursive: true);
			Assert.That(errors, Is.Not.Null);
		}
    }
}
