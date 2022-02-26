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
	public static class MissingMonoScriptTests {
		[Test]
		public static void RenamedComponentMissingMonoScript_ReturnsErrors() {
			GameObject renamedComponentPrefab = Resources.Load<GameObject>("DTValidatorTests/TestRenamedComponentPrefab");

			IList<IValidationError> errors = Validator.Validate(renamedComponentPrefab);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
