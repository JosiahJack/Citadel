using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using DTValidator.Internal;

namespace DTValidator.ValidationErrors {
	public class ObjectValidationError : IValidationError {
		// PRAGMA MARK - Public Interface
		public readonly int ObjectLocalId;
		public readonly Type ObjectType;
		public readonly object ContextObject;
		public readonly MemberInfo MemberInfo;

		public ObjectValidationError(object obj, Type objectType, MemberInfo memberInfo, object contextObject) {
			ObjectLocalId = (obj as UnityEngine.Object).GetLocalId();
			ObjectType = objectType;
			MemberInfo = memberInfo;
			ContextObject = contextObject;
		}

		public override string ToString() {
			return string.Format("OVE ({0}->{1}) context: {2}", MemberInfo.DeclaringType.Name, MemberInfo.Name, ContextObject);
		}


		// PRAGMA MARK - IValidationError Implementation
		int IValidationError.ObjectLocalId {
			get { return ObjectLocalId; }
		}

		Type IValidationError.ObjectType {
			get { return ObjectType; }
		}

		MemberInfo IValidationError.MemberInfo {
			get { return MemberInfo; }
		}

		object IValidationError.ContextObject {
			get { return ContextObject; }
		}
	}
}
