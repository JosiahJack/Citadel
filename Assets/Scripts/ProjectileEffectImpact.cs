using UnityEngine;
using System.Collections;

public class ProjectileEffectImpact : MonoBehaviour {
	private bool impact = false;
	public float impactFadeTime;
	public Material impactMaterial;
	public GameObject host;
	//private SphereCollider sphere;
	private bool setOnce = true;

	//void Awake () {
	//	sphere = GetComponent<SphereCollider>();
	//}

	void Update () {
		if (impact) {
			if (impactFadeTime < Time.time) {
				GetComponent<Billboard>().DestroySprite();
			}
		}
	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject != host) {
			if (setOnce) {
				impact = true;
				GetComponent<MeshRenderer>().material = impactMaterial;
				impactFadeTime = Time.time + impactFadeTime;
				setOnce = false;
			}
		}
	}
}
