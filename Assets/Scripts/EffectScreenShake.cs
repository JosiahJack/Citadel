using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectScreenShake : MonoBehaviour {
	[Tooltip("Distance in world units, set to -1 to use global default")] public float distance = 15f;
	[Tooltip("Force of shaking, higher is more violent, set to -1 to use global default")] public float force = 10f;
	public bool effectIsGlobal = false;
	public void Shake() {
		Const.a.Shake(effectIsGlobal,distance,force);
	}
}