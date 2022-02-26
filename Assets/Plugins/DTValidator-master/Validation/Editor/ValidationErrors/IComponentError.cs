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
	public interface IComponentValidationError : IValidationError {
		Component Component { get; }
	}
}
