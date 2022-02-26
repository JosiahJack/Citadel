using UnityEngine;

namespace DTValidator {
    [CreateAssetMenu(fileName = "ValidatorBlacklistedClass", menuName = "DTValidator/ValidatorBlacklistedClass")]
    public class ValidatorBlacklistedClass : ScriptableObject {
        public string Class;
    }
}
