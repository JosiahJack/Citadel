using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using NUnit.Framework;

namespace DTValidator {
	public static class ValidationTests {
		[Test]
		public static void ValidateSavedScriptableObjects() {
			IList<IValidationError> errors = ValidationUtil.ValidateAllSavedScriptableObjects(earlyExitOnError: true);
			Assert.That(errors, Is.Empty);
		}

		[Test]
		public static void ValidateGameObjectsInResources() {
			IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInResources(earlyExitOnError: true);
			Assert.That(errors, Is.Empty);
		}

		[Test]
		public static void ValidateSavedScenes() {
			IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInSavedScenes(earlyExitOnError: true);
			Assert.That(errors, Is.Empty);
		}

		// [Test]
		public static void ValidateBuildScenes() {
			IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInBuildSettingScenes(earlyExitOnError: true);
			Assert.That(errors, Is.Empty);
		}
	}
}