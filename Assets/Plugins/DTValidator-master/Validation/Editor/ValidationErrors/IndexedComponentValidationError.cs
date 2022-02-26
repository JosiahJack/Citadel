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
	public class IndexedComponentValidationError : IComponentValidationError {
		// PRAGMA MARK - Public Interface
		public readonly int ObjectLocalId;
		public readonly Type ObjectType;
		public readonly MemberInfo MemberInfo;
		public readonly object ContextObject;
		public readonly int Index;

		public IndexedComponentValidationError(Component component, Type objectType, MemberInfo memberInfo, object contextObject, int index) {
			component_ = component;

			ObjectLocalId = component.GetLocalId();
			ObjectType = objectType;
			MemberInfo = memberInfo;
			ContextObject = contextObject;
			Index = index;
		}

		public override string ToString() {
			return string.Format("IOVE ({0}->{1}[{2}]) context: {3}", MemberInfo.DeclaringType.Name, MemberInfo.Name, Index, ContextObject);
		}


		// PRAGMA MARK - IComponentValidationError Implementation
		Component IComponentValidationError.Component {
			get { return component_; }
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


		// PRAGMA MARK - Internal
		private readonly Component component_;
	}
}
