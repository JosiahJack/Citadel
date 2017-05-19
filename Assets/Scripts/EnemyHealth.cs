using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
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
	public int index;

	void Awake () {
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource>();
		rbody = GetComponent<Rigidbody>();
		if (index < 0 || index > 23) {
			Debug.Log("BUG: Enemy with EnemyHealth not assigned an index (0 through 23), disabled.");
			this.enabled = false;
		}
		health = Const.a.healthForNPC[index]; // initialize health from the Const tables
	}

	public void TakeDamage (DamageData dd) {
		if (health < 0)
			return;

		// Update damage data with this entities information
		dd.other = gameObject;
		dd.armorvalue = Const.a.armorvalueForNPC[index];
		dd.defense = Const.a.defenseForNPC[index];
		dd.indexNPC = index;

		float take = Const.a.GetDamageTakeAmount(dd);
		health -= take;
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
