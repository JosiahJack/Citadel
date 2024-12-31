using UnityEngine;

namespace CSM.Samples
{
    public class RandomUnitTranslation : MonoBehaviour
    {
        void Awake(){
            this.transform.position += Random.insideUnitSphere;
        }
    }
}