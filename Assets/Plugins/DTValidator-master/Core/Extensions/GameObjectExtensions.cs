using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTValidator.Internal {
	public static class GameObjectExtensions {
		public static string FullName(this GameObject g) {
			string name = g.name;
			while (g.transform.parent != null) {
				g = g.transform.parent.gameObject;
				name = g.name + "/" + name;
			}
			return name;
		}

		public static IEnumerable<GameObject> GetChildren(this GameObject g) {
			foreach (Transform childTransform in g.transform) {
				yield return childTransform.gameObject;
			}
		}

		public static GameObject GetParent(this GameObject g) {
			if (g.transform.parent == null) {
				return null;
			}

			return g.transform.parent.gameObject;
		}

		public static GameObject GetRoot(this GameObject g) {
			GameObject parent = g.GetParent();
			if (parent == null) {
				return g;
			} else {
				return parent.GetRoot();
			}
		}

		public static IEnumerable<GameObject> GetParents(this GameObject g) {
			while (g != null) {
				yield return g.GetParent();

				g = g.GetParent();
			}
		}
	}
}
