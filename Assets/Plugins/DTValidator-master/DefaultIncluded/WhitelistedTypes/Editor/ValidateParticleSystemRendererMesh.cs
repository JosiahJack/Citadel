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
	public static class ValidateParticleSystemRendererMesh {
		// PRAGMA MARK - Public Interface
		static ValidateParticleSystemRendererMesh() {
			Type particleSystemRendererType = typeof(UnityEngine.ParticleSystemRenderer);
			MemberInfo meshMember = ValidatorUnityWhitelist.GetPropertyFrom(particleSystemRendererType, "mesh");
			ValidatorUnityWhitelist.RegisterWhitelistedTypeMember(particleSystemRendererType, meshMember);

			ValidatorPredicates.RegisterPredicateFor(meshMember, OnlyValidateIfRenderModeMesh);
		}

		private static bool OnlyValidateIfRenderModeMesh(object obj) {
			ParticleSystemRenderer renderer = obj as ParticleSystemRenderer;
			if (renderer == null) {
				return true; // should validate
			}

			// validate when render mode is Mesh
			return renderer.renderMode == ParticleSystemRenderMode.Mesh;
		}
	}
}
#endif
