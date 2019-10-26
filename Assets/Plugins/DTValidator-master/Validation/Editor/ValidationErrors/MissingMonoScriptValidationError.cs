using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace DTValidator.Internal {
	public class MissingMonoScriptValidationError : IValidationError {
		// PRAGMA MARK - Public Interface
		public readonly int GameObjectLocalId;
		public readonly object ContextObject;

		public readonly string GameObjectPath;

		public MissingMonoScriptValidationError(GameObject gameObject, object contextObject) {
			GameObjectLocalId = gameObject.GetLocalId();
			GameObjectPath = gameObject.FullName();
			ContextObject = contextObject;
		}

		public override string ToString() {
			return string.Format("MMSVE ({0})", GameObjectPath);
		}


		// PRAGMA MARK - IValidationError Implementation
		int IValidationError.ObjectLocalId {
			get { return GameObjectLocalId; }
		}

		Type IValidationError.ObjectType {
			get { return typeof(GameObject); }
		}

		MemberInfo IValidationError.MemberInfo {
			get { return null; }
		}

		object IValidationError.ContextObject {
			get { return ContextObject; }
		}
	}
}
