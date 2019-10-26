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
	public class ComponentValidationError : IComponentValidationError {
		// PRAGMA MARK - Public Interface
		public readonly int ComponentLocalId;
		public readonly Type ComponentType;
		public readonly MemberInfo MemberInfo;
		public readonly object ContextObject;

		public readonly string ComponentPath;

		public ComponentValidationError(Component component, Type componentType, MemberInfo memberInfo, object contextObject) {
			component_ = component;

			ComponentLocalId = component.GetLocalId();
			ComponentPath = component.gameObject.FullName();
			ComponentType = componentType;
			MemberInfo = memberInfo;
			ContextObject = contextObject;
		}

		public override string ToString() {
			return string.Format("CVE ({0}=>{1}) context: {2}", MemberInfo.DeclaringType.Name, MemberInfo.Name, ContextObject);
		}


		// PRAGMA MARK - IComponentValidationError Implementation
		Component IComponentValidationError.Component {
			get { return component_; }
		}


		// PRAGMA MARK - IValidationError Implementation
		int IValidationError.ObjectLocalId {
			get { return ComponentLocalId; }
		}

		Type IValidationError.ObjectType {
			get { return ComponentType; }
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
