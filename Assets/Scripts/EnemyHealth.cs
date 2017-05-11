using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
	public float startingHealth = 2f;
	public float health;
	public float deathTime = 0.1f;
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
	private Rigidbody rbody;
	//private MeshCollider meshCollider;
	public SkinnedMeshRenderer enemySkinRenderer;

	void Awake () {
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource>();
		rbody = GetComponent<Rigidbody>();
		health = startingHealth;
		//meshCollider = GetComponent<MeshCollider>();
		//meshCollider.enabled = false;
	}

	public void TakeDamage (float take) {
		if (health < 0)
			return;
		
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

	public void TakeDamage (int index) {

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
		//float lerp = Mathf.PingPong(Time.time, deathTime) / deathTime;
		//Color color = enemySkinRenderer.material.color;
		//color.a += lerp;
		//enemySkinRenderer.material.color = color;
		if (enemySkinRenderer != null && deathMaterial != null) {
			enemySkinRenderer.material.mainTexture = deathMaterial.mainTexture;
		}
			
		if (deathTime < Time.time) {
			Death();
			return;
		}
		anim.SetBool("Dead",true);
	}

	void Death () {
		isDead = true;
		isDying = false;
		SFX.PlayOneShot(SXFDeathClip);
		gameObject.tag = "Searchable";
		rbody.isKinematic = true;
	}
}
