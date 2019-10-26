using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using NUnit.Framework;

namespace DTValidator.Internal {
	public static class PersistentListenerValidationTests {
		private class UnityEventOutletComponent : MonoBehaviour {
			public UnityEvent EventOutlet = new UnityEvent();
		}

		private class SimpleEventHandlerComponent : MonoBehaviour {
			public void ResolveEvent() {}
		}

		[Test]
		public static void ValidPersistentListener_ReturnsNoErrors() {
			GameObject go = new GameObject();
			var simpleEventHandlerComponent = go.AddComponent<SimpleEventHandlerComponent>();
			var unityEventOutletComponent = go.AddComponent<UnityEventOutletComponent>();
			unityEventOutletComponent.EventOutlet.AddVoidPersistentListener(simpleEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void InvalidPersistentListener_ReturnsError() {
			GameObject go = new GameObject();
			var simpleEventHandlerComponent = go.AddComponent<SimpleEventHandlerComponent>();
			var unityEventOutletComponent = go.AddComponent<UnityEventOutletComponent>();
			unityEventOutletComponent.EventOutlet.AddVoidPersistentListener(simpleEventHandlerComponent, "ResolveEvento");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		[Test]
		public static void InvalidUnityEnginePersistentListener_ReturnsError() {
			GameObject go = new GameObject();
			var button = go.AddComponent<UnityEngine.UI.Button>();
			button.onClick.AddVoidPersistentListener(go as UnityEngine.Object, "Invalido");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		private class ComplexEventHandlerComponent : MonoBehaviour {
			public void ResolveEvent() {}
			public void ResolveEvent(int i) {}
		}

		[Test]
		public static void EventWithSameName_ResolvesCorrectly() {
			GameObject go = new GameObject();
			var complexEventHandlerComponent = go.AddComponent<ComplexEventHandlerComponent>();
			var unityEventOutletComponent = go.AddComponent<UnityEventOutletComponent>();
			unityEventOutletComponent.EventOutlet.AddVoidPersistentListener(complexEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Null);
		}

		private class IntUnityEvent : UnityEvent<int> {
		}

		private class IntUnityEventOutletComponent : MonoBehaviour {
			public IntUnityEvent EventOutlet = new IntUnityEvent();
		}

		[Test]
		public static void IntEventWithSameName_ResolvesCorrectly() {
			GameObject go = new GameObject();
			var complexEventHandlerComponent = go.AddComponent<ComplexEventHandlerComponent>();
			var intUnityEventOutletComponent = go.AddComponent<IntUnityEventOutletComponent>();
			intUnityEventOutletComponent.EventOutlet.AddVoidPersistentListener(complexEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Null);
		}

		// NOTE (darren): interestingly enough, it's valid to serialize an IntUnityEvent
		// and point it at any function with any arguments...
		[Test]
		public static void IntEventRegisteredToVoid_ReturnsNoErrors() {
			GameObject go = new GameObject();
			var simpleEventHandlerComponent = go.AddComponent<SimpleEventHandlerComponent>();
			var intUnityEventOutletComponent = go.AddComponent<IntUnityEventOutletComponent>();
			intUnityEventOutletComponent.EventOutlet.AddVoidPersistentListener(simpleEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Null);
		}

		private class SubclassEventHandlerComponent : SimpleEventHandlerComponent {
			// stub - inherits ResolveEvent() method from superclass
		}

		[Test]
		public static void ValidPersistentListener_PointingToSubclass_ReturnsNoErrors() {
			GameObject go = new GameObject();
			var subclassEventHandlerComponent = go.AddComponent<SubclassEventHandlerComponent>();
			var unityEventOutletComponent = go.AddComponent<UnityEventOutletComponent>();
			unityEventOutletComponent.EventOutlet.AddVoidPersistentListener(subclassEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Null);
		}

		private class IntOnlyEventHandlerComponent : MonoBehaviour {
			public void ResolveEvent(int i) {}
		}

		[Test]
		public static void UnityEventRegisteredToVoid_PointingToIntOnly_ReturnsErrors() {
			GameObject go = new GameObject();
			var intOnlyEventHandlerComponent = go.AddComponent<IntOnlyEventHandlerComponent>();
			var intUnityEventOutletComponent = go.AddComponent<IntUnityEventOutletComponent>();
			intUnityEventOutletComponent.EventOutlet.AddVoidPersistentListener(intOnlyEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
