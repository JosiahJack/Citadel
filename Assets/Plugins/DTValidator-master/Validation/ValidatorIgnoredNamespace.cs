using System;
using UnityEngine;

namespace DTValidator {
	[CreateAssetMenu(fileName = "ValidatorIgnoredNamespace", menuName = "DTValidator/ValidatorIgnoredNamespace")]
	public class ValidatorIgnoredNamespace : ScriptableObject {
		public string Namespace;
	}
}