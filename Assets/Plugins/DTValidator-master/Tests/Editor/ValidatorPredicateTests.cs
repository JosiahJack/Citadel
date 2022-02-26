using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace DTValidator.Internal {
	public static class ValidatorPredicateTests {
		private class OutletComponent : MonoBehaviour {
			public GameObject Outlet;
		}

		private static readonly MemberInfo kMeshFilterSharedMesh = ValidatorUnityWhitelist.GetPropertyFrom(typeof(UnityEngine.MeshFilter), "sharedMesh");
		private static readonly MemberInfo kOutletComponentOutlet = ValidatorUnityWhitelist.GetFieldFrom(typeof(OutletComponent), "Outlet");

		[SetUp]
		public static void Setup() {
			ValidatorPredicates.RegisterPredicateFor(kMeshFilterSharedMesh, DontValidateIfMeshRenderer);
			ValidatorPredicates.RegisterPredicateFor(kOutletComponentOutlet, DontValidateIfMeshRenderer);
		}

		[TearDown]
		public static void Cleanup() {
			ValidatorPredicates.UnregisterPredicateFor(kMeshFilterSharedMesh, DontValidateIfMeshRenderer);
			ValidatorPredicates.UnregisterPredicateFor(kOutletComponentOutlet, DontValidateIfMeshRenderer);
		}

		[Test]
		public static void TestPredicate_WorksAsExpected() {
			GameObject gameObject = new GameObject();

			var meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = null;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));

			gameObject.AddComponent<MeshRenderer>();

			IList<IValidationError> newErrors = Validator.Validate(gameObject);
			Assert.That(newErrors, Is.Null);
		}

		[Test]
		public static void TestNonUnityPredicate_WorksAsExpected() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<OutletComponent>();
			outletComponent.Outlet = null;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));

			gameObject.AddComponent<MeshRenderer>();

			IList<IValidationError> newErrors = Validator.Validate(gameObject);
			Assert.That(newErrors, Is.Null);
		}

		private static bool DontValidateIfMeshRenderer(object obj) {
			UnityEngine.Component component = obj as UnityEngine.Component;
			if (component == null) {
				return true;
			}

			return component.GetComponent<MeshRenderer>() == null;
		}
	}
}
