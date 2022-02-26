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
	public static class WhitelistValidationTests {
		[Test]
		public static void MeshFilterMissingMesh_ReturnsError() {
			GameObject gameObject = new GameObject();

			var meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = null;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		[Test]
		public static void BrokenMeshFilterPrefab_ReturnsErrors() {
			GameObject brokenMeshFilterPrefab = Resources.Load<GameObject>("DTValidatorTests/BrokenMeshFilterPrefab");

			IList<IValidationError> errors = Validator.Validate(brokenMeshFilterPrefab, recursive: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
