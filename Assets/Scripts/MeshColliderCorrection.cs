using UnityEngine;
using System.Collections;

public class MeshColliderCorrection : MonoBehaviour {
	[HideInInspector]
	public Collider meshCollider;

	void Awake () {
		DestroyImmediate(gameObject.GetComponent<MeshCollider>());
		meshCollider = gameObject.AddComponent<MeshCollider>();
	}
}
