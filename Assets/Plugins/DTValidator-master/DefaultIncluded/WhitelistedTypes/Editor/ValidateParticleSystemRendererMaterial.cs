#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DTValidator.Internal {
	[InitializeOnLoad]
	public static class ValidateParticleSystemRendererMaterial {
		// PRAGMA MARK - Public Interface
		static ValidateParticleSystemRendererMaterial() {
			Type particleSystemRendererType = typeof(UnityEngine.ParticleSystemRenderer);
			MemberInfo sharedMaterialMember = ValidatorUnityWhitelist.GetPropertyFrom(particleSystemRendererType, "sharedMaterial");
			ValidatorUnityWhitelist.RegisterWhitelistedTypeMember(particleSystemRendererType, sharedMaterialMember);

			ValidatorPredicates.RegisterPredicateFor(sharedMaterialMember, DontValidateIfRenderModeNone);
		}

		private static bool DontValidateIfRenderModeNone(object obj) {
			ParticleSystemRenderer renderer = obj as ParticleSystemRenderer;
			if (renderer == null) {
				return true; // should validate
			}

			// validate when render mode is Mesh
			return renderer.renderMode != ParticleSystemRenderMode.None;
		}
	}
}
#endif
