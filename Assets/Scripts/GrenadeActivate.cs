using UnityEngine;
using System.Collections;

// Used on the physical grenade iteslf.
public class GrenadeActivate : MonoBehaviour {
	public int constIndex = -1; // Useable Item Index (NOT the master index)s
	public float nearforce;
	public float nearradius;
	public float damage = 11f;
	public float penetration = 20f;
	public float offense = 3f;
	public AttackType attackType = AttackType.Projectile;
	public bool proxSensed = false;
	public bool useProx = false; // save
	public PoolType explosionType = PoolType.GrenadeFragExplosions;
	public bool active = false;
	
	[HideInInspector] public float timeFinished; // save
	[HideInInspector] public bool explodeOnContact = false; // save
	[HideInInspector] public bool useTimer = false; // save
	private GameObject explosionEffect;
	private Rigidbody rbody;

	void Awake () {
		rbody = GetComponent<Rigidbody>();
		if (constIndex == 11) {
			GameObject childGO = transform.GetChild(0).gameObject;
			if (childGO != null) {
				childGO.layer = gameObject.layer;
			}
		}
	}

	public void AwakeFromLoad(float health) {
		if (health > 0) {
			rbody.useGravity = true;
			Utils.EnableCollision(gameObject);
		} else {
			rbody.useGravity = false;
			Utils.DisableCollision(gameObject);
		}
	}

	void Update() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;
		if (!active) return;

		// Plastique or other explosive device:
		if (constIndex == 14 && active && gameObject.activeInHierarchy) {
			Explode();
			return;
		}

		// Standard grenade explode route:
		if ((useTimer && timeFinished < PauseScript.a.relativeTime)
			|| (useProx && proxSensed)) {

			Explode();
		}
	}

	// Index = Const.a.useableItemsFrobIcon index.
	public void Activate() {
		switch(constIndex) {
			case 7: explodeOnContact = true; break; // Fragmentation Grenade
			case 8: explodeOnContact = true; break; // Concussion Grenade
			case 9: explodeOnContact = true; break; // EMP Grenade
			case 10: timeFinished = PauseScript.a.relativeTime + Inventory.a.earthShakerTimeSetting;
					 useTimer = true; break;        // Earthshaker Bomb
			case 11: useProx = true; explodeOnContact = false; break; // Land Mine
			case 12: timeFinished = PauseScript.a.relativeTime + Inventory.a.nitroTimeSetting; 
					 useTimer = true; break;        // Nitropack Explosive
			case 13: explodeOnContact = true; break; // Gas Grenade
			default: return;
		}
		active = true;
	}

	void OnCollisionStay(Collision col) {
		if (explodeOnContact) Explode();
	}

	public bool IsNPCMine() {
		if (gameObject.layer == 11) return false; // Bullet
		return true;
	}

	public void Explode() {
		Debug.Log("Grenade exploded");
		Utils.DisableCollision(gameObject);
		DamageData dd = new DamageData();
		dd.damage = damage;
		dd.attackType = attackType;
		dd.penetration = penetration;
		dd.offense = offense;
		dd.impactVelocity = damage * 1.5f;
		if (!IsNPCMine()) {
			dd.owner = Const.a.player1Capsule;
			PlayerHealth.a.makingNoise = true;
			PlayerHealth.a.noiseFinished = PauseScript.a.relativeTime + 2f;
		}
		
		Utils.ApplyImpactForceSphere(dd,transform.position,nearradius,1.0f);
		GameObject explosionEffect = Const.a.GetObjectFromPool(explosionType);
		if (explosionEffect != null) {
			explosionEffect.SetActive(true);
			explosionEffect.transform.position = transform.position;
			int soundIndex = 60; // attack1_explode
			switch(constIndex) {
				case 7:  soundIndex = 64; break; // explosion1
				case 8:  soundIndex = 60; break; // attack1_explode
				case 9:  soundIndex = 67; break; // hit2
				case 10: soundIndex = 60; break; // attack1_explode
				case 11: soundIndex = 64; break; // explosion1
				case 12: soundIndex = 60; break; // attack1_explode
				case 13: soundIndex = 63; break; // explode_minor
			}
			
			Utils.PlayTempAudio(transform.position,Const.a.sounds[soundIndex]);
		}

		Const.a.Shake(true,-1,-1);
		gameObject.SetActive(false);
	}

	// Live grenades - These should only be up in the air or active running timer, but still...or it's a landmine
	public static string Save(GameObject go) {
		string line = System.String.Empty;
		GrenadeActivate ga = go.GetComponent<GrenadeActivate>();
		line = Utils.UintToString(ga.constIndex,"constIndex");
		line += Utils.splitChar + Utils.BoolToString(ga.active,"active");
		line += Utils.splitChar + Utils.BoolToString(ga.useTimer,"useTimer");
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ga.timeFinished,"timeFinished");
		line += Utils.splitChar + Utils.BoolToString(ga.explodeOnContact,"explodeOnContact");
		line += Utils.splitChar + Utils.BoolToString(ga.useProx,"useProx");
		line += Utils.splitChar + Utils.BoolToString(ga.IsNPCMine(),"IsNPCMine");
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		GrenadeActivate ga = go.GetComponent<GrenadeActivate>();
		if (entries == null) {
			Debug.Log("GrenadeActivate.Load failure, entries == null");
			return index + 7;
		}

		ga.constIndex = Utils.GetIntFromString(entries[index],"constIndex"); index++;
		ga.active = Utils.GetBoolFromString(entries[index],"active"); index++;
		ga.useTimer = Utils.GetBoolFromString(entries[index],"useTimer"); index++;
		ga.timeFinished = Utils.LoadRelativeTimeDifferential(entries[index],"timeFinished"); index++;
		ga.explodeOnContact = Utils.GetBoolFromString(entries[index],"explodeOnContact"); index++;
		ga.useProx = Utils.GetBoolFromString(entries[index],"useProx"); index++;
		bool isNPC = Utils.GetBoolFromString(entries[index],"IsNPCMine"); index++;
		if (isNPC) {
			go.layer = 24; // NPCBullet
		} else {
			go.layer = 11; // Bullet (player's)
		}

		if (ga.constIndex == 11) {
			if (isNPC) ga.active = true;
			GameObject childGO = ga.transform.GetChild(0).gameObject;
			if (childGO != null) {
				childGO.layer = go.layer;
			}
		}

		return index;
	}
}
