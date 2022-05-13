using UnityEngine;
using System.Collections;

public class PauseParticleSystem : MonoBehaviour {
	private ParticleSystem psys;
	private Vector3 previousVelocity;
	private bool previousUseGravity;
	private bool previousKinematic;
	private CollisionDetectionMode previouscolDetMode;

	void Awake () {
		Initialize();
	}

	void Initialize() {
		psys = GetComponent<ParticleSystem>();
		if (!Const.a.psys.Contains(this)) Const.a.psys.Add(this);
	}

	void OnEnable () {
		if (psys == null) Initialize();
	}
		
	public void Pause () {
		if (psys != null && this.enabled && gameObject.activeInHierarchy) psys.Pause(true); // Pause the particles.
	}

	public void UnPause () {
		if (psys != null && this.enabled && gameObject.activeInHierarchy) psys.Play(true); // Resume the particles.
	}
}
