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
	public static class WhitelistTypeRegistrationTests {
		private static readonly MemberInfo kCanvasWorldCamera = ValidatorUnityWhitelist.GetPropertyFrom(typeof(Canvas), "worldCamera");

		[SetUp]
		public static void Setup() {
			ValidatorUnityWhitelist.RegisterWhitelistedTypeMember(typeof(Canvas), kCanvasWorldCamera);
		}

		[TearDown]
		public static void Cleanup() {
			ValidatorUnityWhitelist.UnregisterWhitelistedTypeMember(typeof(Canvas), kCanvasWorldCamera);
		}

		[Test]
		public static void TestWorldCamera_ReturnsError() {
			GameObject gameObject = new GameObject();

			var canvas = gameObject.AddComponent<Canvas>();
			canvas.worldCamera = null;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
