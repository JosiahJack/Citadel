using System;
using UnityEngine;

namespace DTValidator {
	// NOTE (darren): ValidatorWhitelistedNamespace and ValidatorIgnoredNamespace are mutually exclusive
	// ValidatorIgnoredNamespace is blacklisting - all scripts are valid except for those that fall in ignored namespace
	// ValidatorWhitelistedNamespace is whitelisting - all scripts are ignored except for those that fall in whitelisted namespace
	//
	// If any ValidatorWhitelistedNamespace exists in the project - validator will use whitelisting instead of blacklisting
	[CreateAssetMenu(fileName = "ValidatorWhitelistedNamespace", menuName = "DTValidator/ValidatorWhitelistedNamespace")]
	public class ValidatorWhitelistedNamespace : ScriptableObject {
		public string Namespace;
	}
}