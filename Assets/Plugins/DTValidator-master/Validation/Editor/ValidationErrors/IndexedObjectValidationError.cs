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
	public class IndexedObjectValidationError : IValidationError {
		// PRAGMA MARK - Public Interface
		public readonly int ObjectLocalId;
		public readonly Type ObjectType;
		public readonly MemberInfo MemberInfo;
		public readonly object ContextObject;
		public readonly int Index;

		public IndexedObjectValidationError(object obj, Type objectType, MemberInfo memberInfo, object contextObject, int index) {
			ObjectLocalId = (obj as UnityEngine.Object).GetLocalId();
			ObjectType = objectType;
			MemberInfo = memberInfo;
			ContextObject = contextObject;
			Index = index;
		}

		public override string ToString() {
			return string.Format("IOVE ({0}->{1}[{2}]) context: {3}", MemberInfo.DeclaringType.Name, MemberInfo.Name, Index, ContextObject);
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
