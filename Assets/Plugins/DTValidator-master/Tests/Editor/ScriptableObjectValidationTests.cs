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
	public static class ScriptableObjectValidationTests {
		[Test]
		public static void MissingOutlet_ReturnsErrors() {
			OutletScriptableObject obj = ScriptableObject.CreateInstance<OutletScriptableObject>();
			obj.Outlet = null;

			IList<IValidationError> errors = Validator.Validate(obj);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
