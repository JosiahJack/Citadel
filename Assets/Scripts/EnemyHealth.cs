using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
	public float startingHealth = 2f;
	public float health;
	public float deathTime;
	public AudioClip SXFDeathClip;
	public AudioClip SXFIdleClip;
	public AudioClip SXFPainClip;
	public Material mainMaterial;
	public Material deathMaterial;
	private AudioSource SFX;
	private Animator anim;
	private bool isDead;
	private bool isDying;
	private float idleTime;
	private MeshCollider meshCollider;
	public SkinnedMeshRenderer enemySkinRenderer;

	void Awake () {
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource>();
		health = startingHealth;
		meshCollider = GetComponent<MeshCollider>();
		meshCollider.enabled = false;
	}

	public void TakeDamage (float take) {
		print("Enemy health was: " + health);
		health -= take;
		print("and is now" + health);
		if (health <= 0) {
			deathTime = Time.time + deathTime;
			isDying = true;
			Dying();
		} else {
			SFX.PlayOneShot(SXFPainClip);
		}
	}

	void Update () {
		if (isDying) {
			Dying();
		} else {
			if (!isDead) {
				if (idleTime < Time.time) {
					SFX.PlayOneShot(SXFIdleClip);
					idleTime = Time.time + Random.Range(3f,10f);
				}
			}
		}
	}

	void Dying() {
		float lerp = Mathf.PingPong(Time.time, deathTime) / deathTime;
		enemySkinRenderer.material.Lerp(mainMaterial, deathMaterial, lerp);
		if (deathTime < Time.time) {
			Death();
		}
	}

	void Death () {
		isDead = true;
		isDying = false;
		SFX.PlayOneShot(SXFDeathClip);
		gameObject.tag = "Searchable";
		anim.SetTrigger("Dead");
	}
}
